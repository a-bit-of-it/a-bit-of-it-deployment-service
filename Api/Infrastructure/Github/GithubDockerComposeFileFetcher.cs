using System.Net;
using System.Text;
using Api.Infrastructure.Github.DTOs;

namespace Api.Infrastructure.Github;

public class GithubDockerComposeFileFetcher(HttpClient client)
{
    private const string FileName = "docker-compose.deployment.yml";

    public async Task<string> GetComposeFileAsync(string repository, string tag)
    {
        var url = $"repos/{Config.Organization}/{repository}/contents/{FileName}?ref={tag}";

        var response = await client.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new FileNotFoundException($"'{FileName}' not found in '{repository}' at ref '{tag}'");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GithubFile>();

        if (content is null || content.Encoding != "base64")
            throw new InvalidOperationException($"Unexpected response fetching '{FileName}' from '{repository}'");

        var bytes = Convert.FromBase64String(content.Content);
        return Encoding.UTF8.GetString(bytes);
    }
}

