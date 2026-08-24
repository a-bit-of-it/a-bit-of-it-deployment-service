using System.Net;
using System.Text;
using Api.Infrastructure.Github.DTOs;

namespace Api.Infrastructure.Github;

public class GithubFileFetcher(HttpClient client)
{
    public async Task<string> GetComposeFileAsync(string repository, string tag)
    {
        const string fileName = "docker-compose.yml";
        
        var url = $"repos/{Config.Organization}/{repository}/contents/{fileName}?ref={tag}";

        var response = await client.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new FileNotFoundException($"'{fileName}' not found in '{repository}' at ref '{tag}'");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<GithubFile>();

        if (content is null || content.Encoding != "base64")
            throw new InvalidOperationException($"Unexpected response fetching '{fileName}' from '{repository}'");

        var bytes = Convert.FromBase64String(content.Content);
        return Encoding.UTF8.GetString(bytes);
    }
}

