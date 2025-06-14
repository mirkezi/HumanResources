using Serilog;
using System;
using Swashbuckle.AspNetCore;
using Microsoft.EntityFrameworkCore;
using HumanResources;
using HumanResources.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSerilog();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<HRContext>();

// Logging builder settings
builder.Host.UseSerilog();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/HumanResource")
    .CreateLogger();

// MIDDLEWARE PIPELINE

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
        Console.WriteLine($"Global Error: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Something went wront. Please try again later");
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
        Console.WriteLine($"Input error: {ex.Message}");
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
        Console.WriteLine($"Can't GET Employees: {ex.Message}");
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
        Console.WriteLine($"Can't create employee: {ex.Message}");
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
        Console.WriteLine($"Couldn't retrieve departments: {ex.Message}");
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
        Console.WriteLine($"Couldn't create Department: {ex.Message}");
        return Results.BadRequest();
    }
});
app.Run();

static bool isValidInput(string input)
{
    return !input.Contains("<script>", StringComparison.OrdinalIgnoreCase) && input.All(char.IsLetterOrDigit);
}