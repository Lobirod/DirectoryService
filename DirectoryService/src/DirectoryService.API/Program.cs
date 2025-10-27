using DirectoryService.API;
using DirectoryService.API.Middlewares;
using DirectoryService.Application;
using DirectoryService.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq") ?? throw new ArgumentNullException("Seq"))
    .CreateLogger();

builder.Services.AddWebDependencies();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

builder.Services.AddSerilog();

builder.Services.AddHttpLogging();

var app = builder.Build();

app.UseExceptionMiddleware();

app.UseSerilogRequestLogging();

app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("swagger/v1/swagger.json");
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();