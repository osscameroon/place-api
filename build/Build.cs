using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Serilog;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    FetchDepth = 0,
    On = new[]
    {
        GitHubActionsTrigger.Push,
        GitHubActionsTrigger.PullRequest,
        GitHubActionsTrigger.WorkflowDispatch,
    },
    InvokedTargets = new[] { nameof(Compile) }
)]
class Build : NukeBuild
{
    /// <summary>
    /// It will be run when you run the nuke command without any target
    /// The best practice is to always run it before pushing the changes to source
    /// Run directyly : cmd> nuke
    /// </summary>
    public static int Main() => Execute<Build>(x => x.RunUnitTests);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild
        ? Configuration.Debug
        : Configuration.Release;

    [Solution]
    private readonly Solution Solution;

    [Parameter]
    readonly AbsolutePath TestResultDirectory = RootDirectory + "/.nuke/Artifacts/Test-Results/";

    /// <summary>
    /// I just logs some information
    /// Run directyly : cmd> nuke LogInformation
    /// </summary>
    Target LogInformation =>
        _ =>
            _.Executes(() =>
            {
                Log.Information($"Solution path: {Solution}");
                Log.Information($"Solution directory: {Solution.Directory}");
                Log.Information($"configuration: {Configuration}");
                Log.Information($"TestResultDirectory: {TestResultDirectory}");
            });

    /// <summary>
    /// I prepare the build artifacts
    /// Run directyly : cmd> nuke Preparation
    /// </summary>
    Target Preparation =>
        _ =>
            _.DependsOn(LogInformation)
                .Executes(() =>
                {
                    TestResultDirectory.CreateOrCleanDirectory();
                });

    /// <summary>
    /// It will restore all the dotnet tools mentioned in ./.config/dotnet-tools.json
    /// We use those tools in the following (like stryker and csharpier)
    /// Run directyly : cmd> nuke RestoreDotNetTools
    /// </summary>
    Target RestoreDotNetTools =>
        _ =>
            _.DependsOn(LogInformation)
                .Executes(() =>
                {
                    DotNetTasks.DotNet(arguments: "tool restore");
                });

    /// <summary>
    /// It will clean the solution
    /// Run directyly : cmd> nuke Clean
    /// </summary>
    Target Clean =>
        _ =>
            _.DependsOn(Preparation)
                .Executes(() =>
                {
                    DotNetTasks.DotNetClean();
                });

    Target InstallWorkload =>
        _ =>
            _.DependsOn(Preparation)
                .Executes(() =>
                {
                    DotNetTasks.DotNetWorkloadRestore();
                });

    /// <summary>
    /// It will restore all projects workload
    /// Run directyly : cmd> nuke Restore
    /// </summary>
    Target Restore =>
        _ =>
            _.DependsOn(Clean, InstallWorkload)
                .Executes(() =>
                {
                    DotNetTasks.DotNetRestore(a => a.SetProjectFile(Solution));
                });

    Target Lint =>
        _ =>
            _.DependsOn(Compile, RestoreDotNetTools)
                .Executes(() =>
                {
                    // Only on local we want to apply linting changes to the source code
                    if (!IsLocalBuild)
                        return;

                    DotNetTasks.DotNet("csharpier .");
                });

    /// <summary>
    /// It is almost the same as Lint but in this step, it only checks if there is still any rule violation or not.
    /// It doesn't apply any change to the source code.
    /// If there is any violation, it will break the build and log the reason
    /// Run directyly : cmd> nuke LintCheck
    /// </summary>
    Target LintCheck =>
        _ =>
            _.DependsOn(Lint)
                .Executes(() =>
                {
                    DotNetTasks.DotNet("csharpier --check .");
                });

    /// <summary>
    /// It will Compile the solution
    /// Run directyly : cmd> nuke Compile
    /// </summary>
    Target Compile =>
        _ =>
            _.DependsOn(Restore)
                .Executes(() =>
                {
                    DotNetTasks.DotNetBuild(a =>
                        a.SetProjectFile(Solution)
                            .SetNoRestore(true)
                            .SetConfiguration(Configuration)
                    );
                });

    /// <summary>
    /// It will run all the unit tests
    /// Run directyly : cmd> nuke RunUnitTests
    /// </summary>
    private Target RunUnitTests =>
        _ =>
            _.DependsOn(LintCheck)
                .Executes(() =>
                {
                    IEnumerable<Project> testProjects = Solution.AllProjects.Where(s =>
                        s.Name.EndsWith("Tests.Unit")
                    );

                    DotNetTasks.DotNetTest(a =>
                        a.SetConfiguration(Configuration)
                            .SetNoBuild(true)
                            .SetNoRestore(true)
                            .ResetVerbosity()
                            .SetResultsDirectory(TestResultDirectory)
                            .EnableCollectCoverage()
                            .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                            .SetExcludeByFile("*.Generated.cs")
                            .EnableUseSourceLink()
                            .CombineWith(
                                testProjects,
                                (b, z) =>
                                    b.SetProjectFile(z)
                                        .AddLoggers($"trx;LogFileName={z.Name}.trx")
                                        .SetCoverletOutput(TestResultDirectory + $"{z.Name}.xml")
                            )
                    );
                });
}
