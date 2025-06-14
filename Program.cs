using Serilog;
using System;
using Swashbuckle.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSerilog();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/HumanResource")
    .CreateLogger();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}


app.Run();
