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
var productVersion = "1.4";
var patchNumber = "0";
var buildNumber = "0"; 
var assemblyVersion = $"{productVersion}.{patchNumber}"; // Internal to the CLR, is not exposed
var assemblyFileVersion = $"{productVersion}.{patchNumber}.{buildNumber}"; // Important for the deployment and windows to differentiate the files
var assemblyInformationalVersion = $"{productVersion} Release"; // Product version - the version that you would use on your website etc.

string currentDir = Directory.GetCurrentDirectory();
string artifactsDir = Path.Combine(currentDir, "artifacts");
string assetsDir = Path.Combine(currentDir, "assets");
string buildDir = Path.Combine(artifactsDir, "KioskBrowser");

string packageFileName = $"SwiftKioskBrowser_{assemblyFileVersion}.msix";
string packageFilePath = Path.Combine(artifactsDir, packageFileName);

////////////////////////////////////////////////////////////////////////////////
// OPTIONS
////////////////////////////////////////////////////////////////////////////////

public sealed class Options
{
    [Option('t', "target", Required = false, Default = "build-msix", HelpText = "The target to run")]
    public string Target { get; set; }
    
    [Option("identityName", Required = false, Default = "MortenBrudvik.KioskBrowser", HelpText = "Unique Id of the application")]
    public string IdentityName { get; set; }
    
    [Option("identityPublisher", Required = false, Default = "CN=MortenBrudvik", HelpText = "Publisher of the application")]
    public string IdentityPublisher { get; set; }
}


Options options;

////////////////////////////////////////////////////////////////////////////////
// TARGETS
////////////////////////////////////////////////////////////////////////////////

Target("clean-solution", () => {
    DeleteDirectory(buildDir);
    DeleteDirectory(artifactsDir);
});

Target("build-binaries", DependsOn("clean-solution"), () => {
    Run("dotnet", $@"publish ..\ -c Release -r win-x64 --self-contained true -o {buildDir} /p:FileVersion={assemblyFileVersion} /p:AssemblyVersion={assemblyVersion} /p:InformationalVersion=""{assemblyInformationalVersion}"" ");
});

Target("configure-package",
    DependsOn("build-binaries"),
    () => {
        WriteLine("Copy package assets to artifacts");
        Run("xcopy", $"\"{assetsDir}\" \"{artifactsDir}\" /E /I /Y");
        
        WriteLine("Update package manifest"); 
        string manifestPath = Path.Combine(artifactsDir,"AppxManifest.xml");
        Run("powershell.exe", $"\".\\scripts\\update-package-manifest.ps1\" -manifestPath \"{manifestPath}\" -newVersion \"{assemblyFileVersion}\" -identityName \"{options.IdentityName}\" -identityPublisher \"{options.IdentityPublisher}\" " );
    }
); 

Target("build-msix", DependsOn("configure-package"), () => {
    Run("MakeAppx.exe", $"pack /d \"{artifactsDir}\"  /p \"{packageFilePath}\"");
});

Target("sign-package", DependsOn("build-msix"), () => {
    Run("signtool.exe", $"sign /fd SHA256 /a /n MortenBrudvik {packageFilePath}");
});

////////////////////////////////////////////////////////////////////////////////
// EXECUTION
////////////////////////////////////////////////////////////////////////////////

Parser.Default.ParseArguments<Options>(Args).WithParsed<Options>(o =>
{
    options = o;
    WriteLine("Target: " + options.Target);

    RunTargetsAndExit(new List<string>(){options.Target});
});