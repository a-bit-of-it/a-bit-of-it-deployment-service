using System.Net.Http.Headers;
using Api;
using Api.Application;
using Api.Application.Services;
using Api.Infrastructure.Database;
using Api.Infrastructure.Github;
using Api.Infrastructure.Server;
using Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

var config = 
    builder.Configuration.Get<Config>();

if (config is null)
    throw new Exception("No configuration found.");

builder.Services.AddSingleton(config);

builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<ServerService>();
builder.Services.AddSingleton<IServerConnection, SshConnection>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<DeploymentService>();

builder.Services.AddHttpClient<IImageRepository, GithubImageRepository>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2026-03-10");
    client.DefaultRequestHeaders.Add("User-Agent", "deployment-service");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.GitHub.PackagesToken);
});

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(policy => policy
    .WithOrigins("http://localhost:5094")
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();