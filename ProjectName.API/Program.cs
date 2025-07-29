using ProjectName.Core;
using ProjectName.DataAccess;

using RS.Dapper.Utility;
using RS.Dapper.Utility.Connections;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DapperContext>(); // Or Scoped/Transient based on your design
builder.Services.AddScoped<IDapperRepository, DapperRepository>();
builder.Services.RS_DataAccessDependencyInjections(builder.Configuration);
builder.Services.RS_CoreDependencyInjections(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
