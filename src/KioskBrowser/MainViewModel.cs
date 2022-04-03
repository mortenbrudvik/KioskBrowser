using KioskBrowser.WebView;

namespace KioskBrowser;

public class MainViewModel
{
    private readonly WebViewComponent _webViewComponent;
    private readonly Action _close;

    public MainViewModel(WebViewComponent webViewComponent, Action close)
    {
        _webViewComponent = webViewComponent;
        _close = close;
    }
    public DelegateCommand CloseWindowCommand => new(_ => { _close(); });

    public string Title => "Kiosk Browser";
    public bool IsInstalled => _webViewComponent.IsInstalled;
    public bool IsNotInstalled => !IsInstalled;
}