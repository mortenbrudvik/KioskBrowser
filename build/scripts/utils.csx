using System.IO;

public string MSBuildPath = MSBuildResolver.TryLocateMSBuildPath();

public static class MSBuildResolver
{
    private static readonly List<string> _editions = new() {"professional", "enterprise", "community"};

    public static string TryLocateMSBuildPath()
    {
        Console.Out.WriteLine("Trying to locate msbuild.exe");

        foreach (var edition in _editions)
        {
            var path = GetMSBuildPath("2019", edition);
            Console.Out.WriteLine(edition +": " + path);
            if (File.Exists(path))
            {
                Console.Out.WriteLine("Success!! Msbuild path located: path");
                return path;
            }
        }

        throw new Exception("Failed to locate msbuild file path");
    }

    private static string GetMSBuildPath(string year, string edition)
    {
        return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), $@"\Microsoft Visual Studio\{year}\{edition}\MSBuild\Current\Bin\msbuild.exe");
    }
}

public void DeleteDirectory(string path)
{
    if( Directory.Exists(path))
        Directory.Delete(path, recursive: true);
}


