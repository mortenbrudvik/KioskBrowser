using System.Windows;

namespace KioskBrowser
{
    public class MainViewModel
    {
        public DelegateCommand CloseWindowCommand => new DelegateCommand(x => { Application.Current.Shutdown(); });

        public bool IsInstalled => WebView2Install.GetInfo().InstallType != InstallType.NotInstalled;
        public bool IsNotInstalled => !IsInstalled;
    }
}