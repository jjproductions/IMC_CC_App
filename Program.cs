using IMC_CC_App.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using System;
using Serilog;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Components;
using IMC_CC_App.Routes;
using IMC_CC_App.Repositories;
using Asp.Versioning;
using IMC_CC_App.Security;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
});
////

//Postgres & DBContext
builder.Services.AddDbContext<DbContext_CC>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDB")));
////

//Serilog
builder.Host.UseSerilog((context, configuration)
    => configuration.ReadFrom.Configuration(context.Configuration));
////

//API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});
////

//Setting up APIs
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

builder.Services.AddScoped<RouterBase, StatementsAPI>();
builder.Services.AddScoped<RouterBase, ExpenseAPI>();
builder.Services.AddScoped<RouterBase, UserAPI>();
////

//CORS setup
var MyAllowedSpecificOrigins = builder.Configuration.GetSection("AllowedOrigins");
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowedOrigins",
        policy =>
        {
            policy.WithOrigins(MyAllowedSpecificOrigins.Value).AllowAnyHeader();
        }
        );
});
////




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseMiddleware<AuthorizationMW>();



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


