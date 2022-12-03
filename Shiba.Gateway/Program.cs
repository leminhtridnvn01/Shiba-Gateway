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

builder.Services.AddHealthChecks();
Console.WriteLine(envName);
builder.Services.AddOcelot(configuration);
Console.WriteLine("Add Ocelot");
// Add services to the container.
builder.Services.AddMvc(option =>
{
    option.RespectBrowserAcceptHeader = true;
});
builder.Services.Configure<ForwardedHeadersOptions>(option =>
{
    option.ForwardedHeaders = ForwardedHeaders.All;
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("This is Development");
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseOcelot().Wait();
Console.WriteLine("PassOcelot");
app.UseHttpsRedirection();
Console.WriteLine("PassHttpsRedirection");
app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                                                    //.AllowCredentials()
                .WithExposedHeaders("*")
                .SetPreflightMaxAge(TimeSpan.FromSeconds(600)));
Console.WriteLine("PassCors");

app.UseHealthChecks("/healthcheck");
Console.WriteLine("PassHealthChecks");
app.UseRouting();
Console.WriteLine("PassRouting");
app.UseAuthorization();
Console.WriteLine("PassAuthorization");
app.MapControllers();
Console.WriteLine("PassMapControllers");

app.Run();
