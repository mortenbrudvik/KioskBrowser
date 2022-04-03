using KioskBrowser.WebView;

namespace KioskBrowser.Tests;

public class WebViewComponentFake : WebViewComponent
{
    private readonly string _installType;
    public WebViewComponentFake(string installType) => _installType = installType;
    protected override InstallType GetInstallType() => GetInstallVersion(_installType);
}