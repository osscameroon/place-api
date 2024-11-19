using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Serilog;

class Build : NukeBuild
{
    /// <summary>
    /// It will be run when you run the nuke command without any target
    /// The best practice is to always run it before pushing the changes to source
    /// Run directyly : cmd> nuke
    /// </summary>
    public static int Main() => Execute<Build>(x => x.RunIntegrationTests);

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
                        s.Name.EndsWith("Unit.Tests")
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

    private Target RunIntegrationTests =>
        _ =>
            _.DependsOn(Compile, LintCheck, RunUnitTests)
                .Executes(() =>
                {
                    IEnumerable<Project> testProjects = Solution
                        .AllProjects.Where(s => s.Name.EndsWith("Tests.Integration"))
                        .ToList();

                    Log.Information($"Found {testProjects.Count()} integration test projects");

                    foreach (Project project in testProjects)
                    {
                        Log.Information($"Running tests for project: {project.Name}");

                        AbsolutePath testResultFile = TestResultDirectory / $"{project.Name}.trx";
                        AbsolutePath coverageResultFile =
                            TestResultDirectory / $"{project.Name}.xml";

                        DotNetTasks.DotNetTest(s =>
                            s.SetProjectFile(project)
                                .SetConfiguration(Configuration)
                                .SetNoBuild(true)
                                .SetNoRestore(true)
                                .SetResultsDirectory(TestResultDirectory)
                                .SetLoggers($"trx;LogFileName={project.Name}.trx")
                                .EnableCollectCoverage()
                                .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                                .SetCoverletOutput(coverageResultFile)
                                .SetProcessEnvironmentVariable(
                                    "DOTNET_SYSTEM_GLOBALIZATION_INVARIANT",
                                    "1"
                                )
                        );
                    }

                    // Vérifier le contenu du répertoire des résultats
                    string[] resultFiles = Directory.GetFiles(TestResultDirectory);
                    Log.Information($"Files in result directory: {string.Join(", ", resultFiles)}");
                });
}
