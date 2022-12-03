using Microsoft.AspNetCore.HttpOverrides;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Net;
using System.Text;

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

try
{
    string url = "https://localhost:8001/api/booking/locations/cites";

    WebRequest myReq = WebRequest.Create(url);
    myReq.Method = "GET";

    UTF8Encoding enc = new UTF8Encoding();

    WebResponse wr = myReq.GetResponse();
    Stream receiveStream = wr.GetResponseStream();
    StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
    string content = reader.ReadToEnd();
    Console.WriteLine(content);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message.ToString());
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("This is Development");
}
app.UseOcelot().Wait();
Console.WriteLine("PassOcelot");

app.Run();
