using System.Windows;

namespace KioskBrowser
{
    public class MainViewModel
    {
        public DelegateCommand CloseWindowCommand => new DelegateCommand(x => { Application.Current.Shutdown(); });

        public string Title => "Kiosk Browser";
        public bool IsInstalled => WebView2Install.GetInfo().InstallType != InstallType.NotInstalled;
        public bool IsNotInstalled => !IsInstalled;
    }
}