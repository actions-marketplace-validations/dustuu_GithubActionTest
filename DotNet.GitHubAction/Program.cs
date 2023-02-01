using IHost host = Host.CreateDefaultBuilder(args)
    // .ConfigureServices((_, services) => services.AddGitHubActionServices())
    .Build();

static TService Get<TService>(IHost host)
    where TService : notnull =>
    host.Services.GetRequiredService<TService>();

static async Task TestGithubStuff(ActionInputs inputs, IHost host)
{
    using CancellationTokenSource tokenSource = new();

    Console.CancelKeyPress += delegate
    {
        tokenSource.Cancel();
    };

    await Task.Delay(500);

    /*// https://docs.github.com/actions/reference/workflow-commands-for-github-actions#setting-an-output-parameter
    // ::set-output deprecated as mentioned in https://github.blog/changelog/2022-10-11-github-actions-deprecating-save-state-and-set-output-commands/
    var githubOutputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT", EnvironmentVariableTarget.Process);
    if (!string.IsNullOrWhiteSpace(githubOutputFile))
    {
        using (var textWriter = new StreamWriter(githubOutputFile!, true, Encoding.UTF8))
        {
            textWriter.WriteLine($"updated-metrics={updatedMetrics}");
            textWriter.WriteLine($"summary-title={title}");
            textWriter.WriteLine($"summary-details={summary}");
        }
    }
    else
    {
        Console.WriteLine($"::set-output name=updated-metrics::{updatedMetrics}");
        Console.WriteLine($"::set-output name=summary-title::{title}");
        Console.WriteLine($"::set-output name=summary-details::{summary}");
    }*/

    Console.WriteLine($"Owner: {inputs.Owner}");
    Console.WriteLine($"Name: {inputs.Name}");
    Console.WriteLine($"Branch: {inputs.Branch}");
    Console.WriteLine($"Directory: {inputs.Directory}");
    Console.WriteLine($"WorkspaceDirectory: {inputs.WorkspaceDirectory}");

    Environment.Exit(0);
}

var parser = Default.ParseArguments<ActionInputs>(() => new(), args);
parser.WithNotParsed(
    errors =>
    {
        Get<ILoggerFactory>(host)
            .CreateLogger("DotNet.GitHubAction.Program")
            .LogError(
                string.Join(Environment.NewLine, errors.Select(error => error.ToString())));

        Environment.Exit(2);
    });

await parser.WithParsedAsync(options => TestGithubStuff(options, host));
await host.RunAsync();
