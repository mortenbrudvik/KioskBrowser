#load "scripts\utils.csx"

#r "nuget: SimpleExec, 7.0.0"
#r "nuget: CommandLineParser, 2.8.0"
#r "nuget: Bullseye, 3.7.0"

using System;
using System.IO;
using CommandLine;
using static SimpleExec.Command;
using static Bullseye.Targets;
using static System.Console;

////////////////////////////////////////////////////////////////////////////////
// PREPARATION
////////////////////////////////////////////////////////////////////////////////

// Versioning (major.minor.patch.build)
var productVersion = "1.1";
var patchNumber = "0";
var buildNumber = "0"; 
var assemblyVersion = $"{productVersion}.{patchNumber}"; // Internal to the CLR, is not exposed
var assemblyFileVersion = $"{productVersion}.{patchNumber}.{buildNumber}"; // Important for the deployment and windows to differentiate the files
var assemblyInformationalVersion = $"{productVersion} Release"; // Product version - the version that you would use on your website etc.

var artifactsDir = "artifacts";
var publishDir = $"{artifactsDir}/publish";

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

Target("clean-solution", () => DeleteDirectory(artifactsDir));

Target("build-solution", DependsOn("clean-solution"), () => {
    Run("dotnet", $@"publish ..\ -c Release -r win-x64 -o {publishDir} /p:FileVersion={assemblyFileVersion} /p:AssemblyVersion={assemblyVersion} /p:InformationalVersion=""{assemblyInformationalVersion}"" ");
});

Target("build-msi", 
    DependsOn("build-solution"), () => {
    Run(MSBuildPath, $@"..\Installer\ /p:Configuration=Release ");});

////////////////////////////////////////////////////////////////////////////////
// EXECUTION
////////////////////////////////////////////////////////////////////////////////

Parser.Default.ParseArguments<Options>(Args).WithParsed<Options>(o =>
{
    options = o;
    WriteLine("Target: " + options.Target);

    RunTargetsAndExit(new List<string>(){options.Target});
});