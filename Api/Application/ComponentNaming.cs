using System.Text.RegularExpressions;
using Humanizer;

namespace Api.Application;

public static class ComponentNaming
{
    private static readonly Regex TrailingIndexPattern = new(@"-\d+$", RegexOptions.Compiled);

    public static string GetContainerNamePrefix(string customerName, string applicationName) =>
        $"{customerName.Kebaberize()}-{applicationName.Kebaberize()}-";

    public static string GetShortComponentName(string containerName, string prefix)
    {
        var shortName = containerName[prefix.Length..];
        return TrailingIndexPattern.Replace(shortName, "");
    }
}
