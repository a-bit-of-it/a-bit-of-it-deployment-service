using System.Net.Http.Headers;
using DeploymentService;
using DeploymentService.Application;
using DeploymentService.Application.Services;
using DeploymentService.Infrastructure.Database;
using DeploymentService.Infrastructure.Github;
using DeploymentService.Infrastructure.Server;

var builder = WebApplication.CreateBuilder(args);

var config = 
    builder.Configuration.Get<Config>();

if (config is null)
    throw new Exception("No configuration found.");

builder.Services.AddSingleton(config);

builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<IServerConnection, SshConnection>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<DeploymentService.Application.Services.DeploymentService>();

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

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();