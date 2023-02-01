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

    DirectoryInfo directory = new(inputs.Directory);
    Console.WriteLine($"Testing: {directory.FullName}");
    WalkDirectoryTree(directory);

    DirectoryInfo workspaceDirectory = new(inputs.WorkspaceDirectory);
    Console.WriteLine($"Testing: {workspaceDirectory.FullName}");
    WalkDirectoryTree(workspaceDirectory);

    Console.WriteLine("GOODBYE! :D");

    Environment.Exit(0);
}

// from https://learn.microsoft.com/en-US/dotnet/csharp/programming-guide/file-system/how-to-iterate-through-a-directory-tree
static void WalkDirectoryTree(System.IO.DirectoryInfo root)
{
    Console.WriteLine($"WALKING DIR: {root.FullName}");

    System.IO.FileInfo[] files = null!;
    System.IO.DirectoryInfo[] subDirs = null!;

    // First, process all the files directly under this folder
    try
    {
        files = root.GetFiles("*.*");
    }
    // This is thrown if even one of the files requires permissions greater
    // than the application provides.
    catch (UnauthorizedAccessException e)
    {
        // This code just writes out the message and continues to recurse.
        // You may decide to do something different here. For example, you
        // can try to elevate your privileges and access the file again.
        Console.WriteLine(e.Message);
    }

    catch (System.IO.DirectoryNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }

    if (files != null)
    {
        foreach (System.IO.FileInfo fi in files)
        {
            // In this example, we only access the existing FileInfo object. If we
            // want to open, delete or modify the file, then
            // a try-catch block is required here to handle the case
            // where the file has been deleted since the call to TraverseTree().
            Console.WriteLine(fi.FullName);
        }

        // Now find all the subdirectories under this directory.
        subDirs = root.GetDirectories();

        foreach (System.IO.DirectoryInfo dirInfo in subDirs)
        {
            // Resursive call for each subdirectory.
            WalkDirectoryTree(dirInfo);
        }
    }
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
