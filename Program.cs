using Serilog;
using System;
using Swashbuckle.AspNetCore;
using Microsoft.EntityFrameworkCore;
using HumanResources;
using HumanResources.Models;


// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/HumanResource")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services
    builder.Services.AddSwaggerGen();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddDbContext<HRContext>();

    // Logging builder settings
    builder.Host.UseSerilog();

    var app = builder.Build();

    // MIDDLEWARE PIPELINE

    app.UseSerilogRequestLogging();

    // Allow only in development stage
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

    // Global Error-Handling middleware
    app.Use(async (context, next) =>
    {
        try
        {
            await next.Invoke();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Global Error");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong. Please try again later");
        }
    });

    // Input Validation Middleware
    app.Use(async (context, next) =>
    {
        try
        {
            var input = context.Request.Query["input"];
            if (!string.IsNullOrEmpty(input))
            {
                if (!isValidInput(input))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid Input detected");
                    return;
                }
            }
            await next.Invoke();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Input Error");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unexpected error occurred.");
        }
    });

    // HTTP METHODS -> Employee

    // GET: Get all employees
    app.MapGet("/employees", async (HRContext db) =>
    {
        try
        {
            var employees = await db.Employees.ToListAsync();
            return Results.Ok(employees);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Get Employees Error");
            return Results.BadRequest();
        }
    });

    // POST: Create a new Employee
    app.MapPost("/employees", async (HRContext db, HumanResources.Models.Employee newEmployee) =>
    {
        try
        {
            db.Employees.Add(newEmployee);
            await db.SaveChangesAsync();
            return Results.Created($"Employee added: {newEmployee.FirstName}", newEmployee);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Create Employee Error");
            return Results.BadRequest();
        }
    });

    // PUT: Update an employee record
    app.MapPut("/employees/{id:int}", async (int id, HRContext db, HumanResources.Models.Employee employee) =>
    {
        try
        {
            var employeetoUpdate = await db.Employees.FindAsync(id);
            if (employeetoUpdate == null)
            {
                return Results.NotFound();
            }
            employeetoUpdate.FirstName = employee.FirstName;
            employeetoUpdate.LastName = employee.LastName;
            employeetoUpdate.DepartmentID = employee.DepartmentID;
            await db.SaveChangesAsync();
            return Results.Accepted();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Update Employee Error");
            return Results.BadRequest();
        }

    });

    // DELETE: Delete an employee record
    app.MapDelete("/employees/{id:int}", async (int id, HRContext db) =>
    {
        try
        {
            var employeeToDelete = await db.Employees.FindAsync(id);
            if (employeeToDelete == null)
            {
                return Results.NotFound();
            }
            db.Employees.Remove(employeeToDelete);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Delete Employee Error");
            return Results.BadRequest();
        }
    });
    // HTTP METHODS -> Department

    //GET: Get All Departments
    app.MapGet("/departments", async (HRContext db) =>
    {
        try
        {
            var departments = await db.Departments.ToListAsync();
            return Results.Ok(departments);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Get Departments Error");
            return Results.BadRequest();
        }
    });

    //POST: Create a new Department
    app.MapPost("/departments", async (HRContext db, HumanResources.Models.Department newDepartment) =>
    {
        try
        {
            db.Departments.Add(newDepartment);
            await db.SaveChangesAsync();
            return Results.Created($"Department Created: {newDepartment.Name}.", newDepartment);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Create Department Error");
            return Results.BadRequest();
        }
    });

    // PUT: Update a department
    app.MapPut("/departments/{id:int}", async (int id, HRContext db, HumanResources.Models.Department department) =>
    {
        try
        {
            var departmentToUpdate = await db.Departments.FindAsync(id);
            if (departmentToUpdate == null)
            {
                return Results.NotFound();
            }
            departmentToUpdate.Name = department.Name;
            await db.SaveChangesAsync();
            return Results.Accepted();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Update Department Error");
            return Results.BadRequest();
        }
    });

    // DELETE: Delete a department
    app.MapDelete("/departments/{id:int}", async (int id, HRContext db) =>
    {
        try
        {
            var departmentToDelete = await db.Departments.FindAsync(id);
            if (departmentToDelete == null)
            {
                return Results.NotFound();
            }
            db.Departments.Remove(departmentToDelete);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Delete Department Error");
            return Results.BadRequest();
        }
    });


    app.Run();

    static bool isValidInput(string input)
    {
        return !input.Contains("<script>", StringComparison.OrdinalIgnoreCase) && input.All(char.IsLetterOrDigit);
    }

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.CloseAndFlush();
}
