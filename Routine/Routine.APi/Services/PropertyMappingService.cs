﻿using Routine.APi.Entities;
using Routine.APi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Services
{
    //排序使用的属性映射服务（视频P37）
    public class PropertyMappingService : IPropertyMappingService
    {
        //映射关系字典
        //一个属性可以映射到多个属性

        //Employee 的映射关系字典
        //StringComparer.OrdinalIgnoreCase 表明这个字典的 Key 大小写不敏感
        private readonly Dictionary<string, PropertyMappingValue> _employeePropertyMapping
            = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id",new PropertyMappingValue(new List<string>{"Id"}) },
                {"CompanyId",new PropertyMappingValue(new List<string>{"CompanyId"}) },
                {"EmployeeNo",new PropertyMappingValue(new List<string>{"EmployeeNo"}) },
                {"Name",new PropertyMappingValue(new List<string>{"FirstName","LastName"}) },
                {"GenderDisplay",new PropertyMappingValue(new List<string>{"Gender"}) },
                {"Age",new PropertyMappingValue(new List<string>{"DateOfBirth"}, true) } //Age 和 DateOfBirth 排序顺序应该是反转的
            };

        //映射关系列表
        //不能在 IList<T> 中直接使用泛型了，无法解析 IList<PropertyMapping<TSource, TDestination>>
        //因此需要写一个接口 IPropertyMapping，让 PropertyMapping 实现这个接口
        //使用 IList<IPropertyMapping> 来实现
        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            //添加 Employee 的映射关系
            _propertyMappings.Add(new PropertyMapping<EmployeeDto, Employee>(_employeePropertyMapping));
        }

        /// <summary>
        /// 如果 TSource 与 TDestination 存在映射关系，返回映射关系字典
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            //获得 _propertyMappings 对应泛型的映射关系
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();
            var propertyMapping = matchingMapping.ToList();
            if (propertyMapping.Count == 1)
            {
                //如果 TSource 与 TDestination 存在映射关系，返回他们的映射关系字典
                return propertyMapping.First().MappingDictionary;
            }
            throw new Exception($"无法找到唯一的映射关系：{typeof(TSource)},{typeof(TDestination)}");
        }
    }
}
