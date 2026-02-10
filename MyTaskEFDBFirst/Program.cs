using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MyTaskEFDBFirst.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Task Managment API",
        Description = "Task Managment API"
    });
    //To include xml comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddDbContext<TestdbContext>();

Serilog.Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).
                WriteTo.File(Directory.GetCurrentDirectory()+"/logger/logs.txt", rollingInterval: RollingInterval.Day).
                CreateLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Log.Information("My API Is Running");
app.Run();
