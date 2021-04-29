#r "nuget: SimpleExec, 7.0.0"
#r "nuget: CommandLineParser, 2.8.0"
#r "nuget: Bullseye, 3.7.0"

using System.IO;
using CommandLine;
using static SimpleExec.Command;
using static Bullseye.Targets;
using static System.Console;

////////////////////////////////////////////////////////////////////////////////
// PREPARATION
////////////////////////////////////////////////////////////////////////////////

// Versioning (major.minor.patch.build)
var assemblyVersion = "1.0.0"; // Internal to the CLR, is not exposed
var assemblyFileVersion = "1.0.0.0"; // Important for the deployment and windows to differenciate the files
var assemblyInformationalVersion = "1.0 Alpha"; // Product version - the version that you would use on your website etc.

var artifactsDir = "artifacts";
var publishDir = $"{artifactsDir}/publish_winx86";
var msbuildPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe";


////////////////////////////////////////////////////////////////////////////////
// OPTIONS
////////////////////////////////////////////////////////////////////////////////

public sealed class Options
{
    [Option('t', "target", Required = false, Default = "build-msi")]
    public string Target { get; set; }
}

Options options;

////////////////////////////////////////////////////////////////////////////////
// TARGETS
////////////////////////////////////////////////////////////////////////////////

Target("clean-solution", () => {
    if( Directory.Exists(artifactsDir))
        Directory.Delete(artifactsDir, recursive: true);
});

Target("build-solution", DependsOn("clean-solution"), () => {
    Run("dotnet", $@"publish ..\ -c Release -r win-x86 -o {publishDir} /p:FileVersion={assemblyFileVersion} /p:AssemblyVersion={assemblyVersion} /p:InformationalVersion=""{assemblyInformationalVersion}"" ");
});

Target("build-msi", 
    DependsOn("build-solution"), () => {
    Run(msbuildPath, @"..\Installer\");});

////////////////////////////////////////////////////////////////////////////////
// EXECUTION
////////////////////////////////////////////////////////////////////////////////

Parser.Default.ParseArguments<Options>(Args).WithParsed<Options>(o =>
{
    options = o;
    WriteLine("Target: " + options.Target);

    RunTargetsAndExit(new List<string>(){options.Target});
});