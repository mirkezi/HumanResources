# HumanResources
Hi!
This repo is a personal project to showcase achieved knowledge in .NET backend development.

HumanResources API Web Application allow users to retrieve, create, update or delete Employees or Departments. All data is saved and made accessible on a SQLite Database.

![image](https://github.com/user-attachments/assets/7d943ac7-3fff-4911-b611-dfbbd733c796)

## It has implementation for:
1. Logging with Serilog
2. Use of middlewares
3. Swagger / OpenAPI
4. HTTP methods for CRUD operations.
5. Use of EntityFrameworkcore with Sqlite database

## Future Implementation:
1. Authentication / Authorization
2. JWT Token for API Security
3. Encryption / Decryption
4. Caching

### Install EF tool for migrations and commands:
1. dotnet tool install --global dotnet-ef

### Database commands:
1. dotnet-ef migrations add InitialCreate 
2. dotnet-ef database update

### Required Packages (commands):
1. dotnet add package Microsoft.AspNetCore.OpenApi
2. dotnet add package Swashbuckle.AspNetCore
3. dotnet add package Microsoft.EntityFrameworkCore
4. dotnet add package Microsoft.EntityFrameworkCore.Sqlite
5. dotnet add package Microsoft.EntityFrameworkCore.Design
6. dotnet add package Serilog.AspNetCore
7. dotnet add package Serilog.Sinks.File
8. dotnet add package Serilog.Sinks.Console
