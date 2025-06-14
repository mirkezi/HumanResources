# HumanResources
Hi!
This repo is a personal project to showcase achieved knowledge in .NET backend development.

It comprehends:
1. Logging with Serilog
2. Use of middlewares
3. Swagger / OpenAPI
4. HTTP methods for CRUD operations.
5. Use of EntityFrameworkcore with Sqlite database

### Install EF tool for migrations and commands:
1. dotnet tool install --global dotnet-ef
2. dotnet-ef migrations add InitialCreate 
3. dotnet-ef database update

### Required Packages (commands):
1. dotnet add package Microsoft.AspNetCore.OpenApi
2. dotnet add package Swashbuckle.AspNetCore
3. dotnet add package Microsoft.EntityFrameworkCore
4. dotnet add package Microsoft.EntityFrameworkCore.Sqlite
5. dotnet add package Microsoft.EntityFrameworkCore.Design
6. dotnet add package Serilog.AspNetCore
7. dotnet add package Serilog.Sinks.File
8. dotnet add package Serilog.Sinks.Console