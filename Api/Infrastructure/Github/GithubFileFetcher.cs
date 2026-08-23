using System.Net;
using System.Text;

namespace Api.Infrastructure.Github;

public class GithubFileFetcher(HttpClient client)
{
    public async Task<string> GetComposeFileAsync(string repository, string tag)
    {
        const string fileName = "docker-compose.yml";
        
        var url = $"repos/a-bit-of-it/{repository}/contents/{fileName}?ref={tag}";

        var response = await client.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new FileNotFoundException($"'{fileName}' not found in '{repository}' at ref '{tag}'");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<GitHubContentResponse>();

        if (content is null || content.Encoding != "base64")
            throw new InvalidOperationException($"Unexpected response fetching '{fileName}' from '{repository}'");

        var bytes = Convert.FromBase64String(content.Content.Replace("\n", ""));
        return Encoding.UTF8.GetString(bytes);
    }
}

public class GitHubContentResponse
{
    public string Content { get; set; } = default!;
    public string Encoding { get; set; } = default!;
    public string Sha { get; set; } = default!;
}