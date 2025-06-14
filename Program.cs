using Serilog;
using System;
using Swashbuckle.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSerilog();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

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
        if (isValidInput(input))
        {
            await next.Invoke();
        }
        else
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid Input detected");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Input error: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred.");
    }
});

// HTTP METHODS -> Employee

// HTTP METHODS -> Department

app.Run();

static bool isValidInput(string input)
{
    return !string.IsNullOrWhiteSpace(input) && !input.Contains("<script>", StringComparison.OrdinalIgnoreCase) && input.All(char.IsLetterOrDigit);
}