using Microsoft.AspNetCore.HttpOverrides;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
var envName = string.IsNullOrEmpty(builder.Environment.EnvironmentName) ? "production" : builder.Environment.EnvironmentName.ToLower();
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("ocelot.json", false, true)
    .AddJsonFile($"ocelot.{envName}.json", true, true)
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{envName}.json", true, true)
    .AddEnvironmentVariables().Build();

builder.Services.AddOcelot(configuration);
Console.WriteLine("Add Ocelot");



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("This is Development");
}
app.UseOcelot().Wait();
Console.WriteLine("PassOcelot");

app.Run();
