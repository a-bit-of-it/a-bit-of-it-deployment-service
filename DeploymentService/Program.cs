using System.Net.Http.Headers;
using DeploymentService;
using DeploymentService.Services;

var builder = WebApplication.CreateBuilder(args);

var config = 
    builder.Configuration.Get<Config>();

if (config is null)
    throw new Exception("No configuration found.");

builder.Services.AddHttpClient<IGithubService, GithubService>(client =>
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