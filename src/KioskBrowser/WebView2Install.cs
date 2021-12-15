using Microsoft.Web.WebView2.Core;

namespace KioskBrowser;

public static class WebView2Install
{
    public static InstallInfo GetInfo()
    {
        var version = GetWebView2Version();

        return new InstallInfo(version);
    }

    private static string GetWebView2Version()
    {
        try
        {
            return CoreWebView2Environment.GetAvailableBrowserVersionString();
        }
        catch (Exception) { return ""; }
    }
}

public class InstallInfo
{
    public InstallInfo(string version) => (Version) = (version);

    public string Version { get; }

    public InstallType InstallType => Version switch
    {
        var version when version.Contains("dev") => InstallType.EdgeChromiumDev,
        var version when version.Contains("beta") => InstallType.EdgeChromiumBeta,
        var version when version.Contains("canary") => InstallType.EdgeChromiumCanary,
        var version when !string.IsNullOrEmpty(version) => InstallType.WebView2,
        _ => InstallType.NotInstalled
    };
}

public enum InstallType
{
    WebView2, EdgeChromiumBeta, EdgeChromiumCanary, EdgeChromiumDev, NotInstalled
}