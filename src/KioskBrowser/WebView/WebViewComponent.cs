using System.Linq;
using KioskBrowser.Extensions;
using LanguageExt;
using Microsoft.Web.WebView2.Core;

using static KioskBrowser.Common.Error;

namespace KioskBrowser.WebView;

public class WebViewComponent
{
    public bool IsInstalled =>
        GetInstallType() != InstallType.NotInstalled;
    
    protected virtual InstallType GetInstallType() =>
        GetInstallVersion(
            TryCatch(() => 
                CoreWebView2Environment.GetAvailableBrowserVersionString()));

    protected static InstallType GetInstallVersion(Option<string> version) =>
        version switch {
            _ when version.Contains("dev") => InstallType.EdgeChromiumDev,
            _ when version.Contains("beta") => InstallType.EdgeChromiumBeta,
            _ when version.Contains("canary") => InstallType.EdgeChromiumCanary,
            _ when version
                .Match(
                    Some: x => !x.IsNullOrEmpty(),
                    None: false) 
                => InstallType.WebView2,
            _ => InstallType.NotInstalled
        };
}