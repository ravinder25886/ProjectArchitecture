using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

using ProjectName.Core;
using ProjectName.DataAccess;
using ProjectName.DataAccess.Constants;
using RS.Dapper.Utility;
using RS.Dapper.Utility.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Set DbSchema for RS_DapperUtility
DbSchema.Initialize(builder.Configuration);
//Inject RS_DapperUtilityDependencyInjections
builder.Services.RS_DapperUtilityDependencyInjections(builder.Configuration);

//End-RS.Dapper.Utility

//****Inject RS_DapperUtilityDependencyInjections
builder.Services.RS_DataAccessDependencyInjections(builder.Configuration);
builder.Services.RS_CoreDependencyInjections(builder.Configuration);
//setup SqlBuilder IMemoryCache
builder.Services.AddMemoryCache();
var app = builder.Build();

//Start-****************Setup SqlBuilder IMemoryCache for RS_DapperUtility **************
var memoryCache = app.Services.GetRequiredService<IMemoryCache>();
SqlBuilder.Initialize(memoryCache);
//End-*****************Setup SqlBuilder IMemoryCache for RS_DapperUtility****************
// Configure global exception handling middleware to catch unhandled exceptions
// This ensures that the API returns a generic error message without exposing sensitive details
// It also allows centralized logging of errors, improving maintainability and security
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        // Optional: log the exception here
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception occurred");

        var response = new
        {
            Message = "An unexpected error occurred. Please try again later.",
            context.Response.StatusCode,
#if DEBUG
            Details = exception?.ToString()
#endif
        };

        await context.Response.WriteAsJsonAsync(response);
    }));
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
