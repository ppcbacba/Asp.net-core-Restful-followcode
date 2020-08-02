using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Routine.APi.Data;
using Routine.APi.Services;
using System;
using System.Linq;

namespace Routine.APi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // 注册服务 This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加缓存服务（视频P46）
            services.AddResponseCaching();

            //支持高级 CacheHeaders，并进行全局配置（视频P48）
            services.AddHttpCacheHeaders(expires =>
            {
                expires.MaxAge = 60;
                expires.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
            }, validation =>
            {
                //如果响应过期，必须重新验证
                validation.MustRevalidate = true;
            });

            /*
            * 内容协商：
            * 针对一个响应，当有多种表述格式的时候，选取最佳的一个表述，例如 application/json、application/xml
            * 
            * Accept Header 指明服务器输出格式，对应 ASP.NET Core 里的 Output Formatters
            * 如果服务器不支持客户端请求的媒体类型（Media Type），返回状态码406
            * 
            * Content-Type Header 指明服务器输入格式，对应 ASP.NET Core 里的 Input Formatters
            */

            /*
             * .Net Core 默认使用 Problem details for HTTP APIs RFC (7807) 标准
             * - 为所需错误信息的应用，定义了通用的错误格式
             * - 可以识别问题属于哪个 API
             */

            //以下是一种添加序列化工具的写法，在本项目中未采用（视频P8）
            //services.AddControllers(options =>
            //{
            //    //OutputFormatters 默认有且只有 Json 格式
            //    //添加对输出 XML 格式的支持
            //    //此时默认输出格式依然是 Json ,因为 Json 格式位于第一位置
            //    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            //    //如果在 Index 0 位置插入对 XML 格式的支持，那么默认输出格式是 XML
            //    //options.OutputFormatters.Insert(0, new XmlDataContractSerializerOutputFormatter());
            //});

            services.AddControllers(options =>
            {
                //启用406状态码（视频P7）
                options.ReturnHttpNotAcceptable = true;

                //配置缓存字典（视频P46）
                //options.CacheProfiles.Add("120sCacheProfile", new CacheProfile
                //{
                //    Duration = 120
                //});
            })
                //默认格式取决于序列化工具的添加顺序
                .AddNewtonsoftJson(options =>  //第三方 JSON 序列化和反序列化工具（会替换掉原本默认的 JSON 序列化工具）（视频P32）
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters() //XML 序列化和反序列化工具（视频P8）
                .ConfigureApiBehaviorOptions(options =>   //自定义错误报告（视频P29）
                {
                    //创建一个委托 context，在 IsValid == false 时执行
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "https://www.bilibili.com/video/av77957694",
                            Title = "出现错误",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "请看详细信息",
                            Instance = context.HttpContext.Request.Path
                        };
                        problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });

            services.Configure<MvcOptions>(options =>
            {
                //全局设置 NewtonsoftJsonOutputFormatter（视频P43）
                var newtonSoftJsonOutputFormatter = options.OutputFormatters
                                                    .OfType<NewtonsoftJsonOutputFormatter>()
                                                    ?.FirstOrDefault();
                if (newtonSoftJsonOutputFormatter != null)
                {
                    //将 NewtonsoftJsonOutputFormatter 设为 "application/vnd.company.hateoas+json" 等 Media type 的输出格式化器
                    newtonSoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");
                    newtonSoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.company.friendly+json");
                    newtonSoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.company.friendly.hateoas+json");
                    newtonSoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.company.full+json");
                    newtonSoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.company.full.hateoas+json");
                }
            });

            //使用 AutoMapper，扫描当前应用域的所有 Assemblies 寻找 AutoMapper 的配置文件（视频P12）
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //AddScoped 针对每一次 HTTP 请求都会建立一个新的实例（视频P1）
            services.AddScoped<ICompanyRepository, CompanyRepository>();

            services.AddDbContext<RoutineDbContext>(options =>
            {
//                options.UseSqlServer("Data Source=localhost;DataBase=routine;Integrated Security=SSPI");
                options.UseSqlServer("Data Source=(localdb)\\MSSQLLOCALDB;Initial Catalog=routine;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            });

            //轻量服务可以使用 Transient，每次从容器（IServiceProvider）中获取的都是一个新的实例
            //深入理解依赖注入、Singleton、Scoped、Transient 参见 https://www.cnblogs.com/gdsblog/p/8465101.html
            //排序使用的属性映射服务（视频P37）
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            //判断 Uri query 字符串中的 fields 是否合法（视频P39）
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();
        }

        //路由中间件 This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //添加中间件的顺序非常重要，如果你把授权中间件放在了Controller的后边，
            //那么即使需要授权，那么请求也会先到达Controller并执行里面的代码，这样的话授权就没有意义了。（视频P1）

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //500 错误信息
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Unexpected Error!");
                    });
                });
            }

            //缓存中间件
            //app.UseResponseCaching();  //（视频P46）
            app.UseHttpCacheHeaders(); //（视频P48）

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
