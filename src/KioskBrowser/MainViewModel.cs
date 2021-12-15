namespace KioskBrowser;

public class MainViewModel
{
    private readonly Action _close;

    public MainViewModel(Action close)
    {
        _close = close;
    }
    public DelegateCommand CloseWindowCommand => new(x => { _close(); });

    public string Title => "Kiosk Browser";
    public bool IsInstalled => WebView2Install.GetInfo().InstallType != InstallType.NotInstalled;
    public bool IsNotInstalled => !IsInstalled;
}