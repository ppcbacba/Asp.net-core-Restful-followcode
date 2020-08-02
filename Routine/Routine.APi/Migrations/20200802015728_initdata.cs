﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Routine.APi.Migrations
{
    public partial class initdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Introduction = table.Column<string>(maxLength: 500, nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Industry = table.Column<string>(nullable: true),
                    Product = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    EmployeeNo = table.Column<string>(maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Country", "Industry", "Introduction", "Name", "Product" },
                values: new object[,]
                {
                    { new Guid("bbdee09c-089b-4d30-bece-44df5923716c"), "USA", "Software", "Great Company", "Microsoft", "Software" },
                    { new Guid("bbdee09c-089b-4d30-bece-44df59237144"), "USA", "Internet", "Not Exists?", "AOL", "Website" },
                    { new Guid("5efc910b-2f45-43df-afae-620d40542833"), "USA", "ECommerce", "Store", "Amazon", "Books" },
                    { new Guid("6fb600c1-9011-4fd7-9234-881379716433"), "China", "Internet", "Music?", "NetEase", "Songs" },
                    { new Guid("bbdee09c-089b-4d30-bece-44df59237133"), "China", "ECommerce", "Brothers", "Jingdong", "Goods" },
                    { new Guid("5efc910b-2f45-43df-afae-620d40542822"), "China", "Security", "- -", "360", "Security Product" },
                    { new Guid("6fb600c1-9011-4fd7-9234-881379716422"), "USA", "Internet", "Blocked", "Youtube", "Videos" },
                    { new Guid("bbdee09c-089b-4d30-bece-44df59237122"), "USA", "Internet", "Blocked", "Twitter", "Tweets" },
                    { new Guid("5efc910b-2f45-43df-afae-620d40542811"), "China", "ECommerce", "From Jiangsu", "Suning", "Goods" },
                    { new Guid("6fb600c1-9011-4fd7-9234-881379716411"), "Italy", "Football", "Football Club", "AC Milan", "Football Match" },
                    { new Guid("bbdee09c-089b-4d30-bece-44df59237111"), "USA", "Technology", "Wow", "SpaceX", "Rocket" },
                    { new Guid("5efc910b-2f45-43df-afae-620d40542800"), "USA", "Software", "Photoshop?", "Adobe", "Software" },
                    { new Guid("6fb600c1-9011-4fd7-9234-881379716400"), "China", "Internet", "From Beijing", "Baidu", "Software" },
                    { new Guid("bbdee09c-089b-4d30-bece-44df59237100"), "China", "ECommerce", "From Shenzhen", "Tencent", "Software" },
                    { new Guid("5efc910b-2f45-43df-afae-620d40542853"), "China", "Internet", "Fubao Company", "Alipapa", "Software" },
                    { new Guid("6fb600c1-9011-4fd7-9234-881379716440"), "USA", "Internet", "Don't be evil", "Google", "Software" },
                    { new Guid("6fb600c1-9011-4fd7-9234-881379716444"), "USA", "Internet", "Who?", "Yahoo", "Mail" },
                    { new Guid("5efc910b-2f45-43df-afae-620d40542844"), "USA", "Internet", "Is it a company?", "Firefox", "Browser" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[,]
                {
                    { new Guid("4b501cb3-d168-4cc0-b375-48fb33f318a4"), new Guid("bbdee09c-089b-4d30-bece-44df5923716c"), new DateTime(1976, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "MSFT231", "Nick", 1, "Carter" },
                    { new Guid("7eaa532c-1be5-472c-a738-94fd26e5fad6"), new Guid("bbdee09c-089b-4d30-bece-44df5923716c"), new DateTime(1981, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "MSFT245", "Vince", 1, "Carter" },
                    { new Guid("72457e73-ea34-4e02-b575-8d384e82a481"), new Guid("6fb600c1-9011-4fd7-9234-881379716440"), new DateTime(1986, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "G003", "Mary", 0, "King" },
                    { new Guid("7644b71d-d74e-43e2-ac32-8cbadd7b1c3a"), new Guid("6fb600c1-9011-4fd7-9234-881379716440"), new DateTime(1977, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "G097", "Kevin", 1, "Richardson" },
                    { new Guid("679dfd33-32e4-4393-b061-f7abb8956f53"), new Guid("5efc910b-2f45-43df-afae-620d40542853"), new DateTime(1967, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "A009", "卡", 0, "里" },
                    { new Guid("1861341e-b42b-410c-ae21-cf11f36fc574"), new Guid("5efc910b-2f45-43df-afae-620d40542853"), new DateTime(1957, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "A404", "Not", 1, "Man" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
