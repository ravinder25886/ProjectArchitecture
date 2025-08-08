using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

using ProjectName.Core;
using ProjectName.DataAccess;

using RS.Dapper.Utility.Connections;
using RS.Dapper.Utility.Constants;
using RS.Dapper.Utility.Repositories.DapperExecutor;
using RS.Dapper.Utility.Repositories.DapperRepository;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Start-RS.Dapper.Utility
DbSchema.Initialize(builder.Configuration);
builder.Services.AddSingleton<DapperContext>(); // Or Scoped/Transient based on your design
builder.Services.AddScoped<IDapperRepository, DapperRepository>();
builder.Services.AddScoped<IDapperExecutor, DapperExecutor>();
//End-RS.Dapper.Utility

builder.Services.RS_DataAccessDependencyInjections(builder.Configuration);
builder.Services.RS_CoreDependencyInjections(builder.Configuration);

//setup SqlBuilder IMemoryCache
builder.Services.AddMemoryCache();
var app = builder.Build();

//setup SqlBuilder IMemoryCache

var memoryCache = app.Services.GetRequiredService<IMemoryCache>();
SqlBuilder.Initialize(memoryCache);

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
