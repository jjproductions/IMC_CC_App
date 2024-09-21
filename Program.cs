using IMC_CC_App.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using Serilog;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Components;
using IMC_CC_App.Routes;
using IMC_CC_App.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContext_CC>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDB")));

builder.Host.UseSerilog((context, configuration)
    => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

builder.Services.AddScoped<RouterBase, StatementsAPI>();

builder.Services.AddScoped<RouterBase, ExpenseAPI>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



//*************************************
// Add Routes from all "Router Classes"
//*************************************
using (var scope = app.Services.CreateScope())
{
    // Build collection of all RouterBase classes
    var services = scope.ServiceProvider.GetServices<RouterBase>();

    // Loop through each RouterBase class
    foreach (var item in services)
    {
        // Invoke the AddRoutes() method to add the routes
        item.AddRoutes(app);
    }

    // Make sure this is called within the application scope
    app.Run();
}


