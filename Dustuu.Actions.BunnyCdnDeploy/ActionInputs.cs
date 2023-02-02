using CommandLine;

namespace Dustuu.Actions.BunnyCdnDeploy;

public class ActionInputs
{
    [Option('w', "workspace", Required = true)] public string Workspace { get; set; } = null!;

    [Option('d', "dir", Required = true)] public string Directory { get; set; } = null!;

    [Option('u', "bunny-cdn-username", Required = true)] public string BunnyCdnUsername { get; set; } = null!;

    [Option('p', "bunny-cdn-password", Required = true)] public string BunnyCdnPassword { get; set; } = null!;

    [Option('r', "bunny-cdn-region", Required = true)] public string BunnyCdnRegion { get; set; } = null!;

    [Option('a', "bunny-cdn-api-key", Required = true)] public string BunnyCdnApiKey { get; set; } = null!;
}
