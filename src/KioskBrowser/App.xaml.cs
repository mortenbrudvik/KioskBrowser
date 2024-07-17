using System.Runtime.InteropServices;
using System.Windows;

namespace KioskBrowser;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var appId = "KioskBrowser_" + Guid.NewGuid();
        SetCurrentProcessExplicitAppUserModelID(appId);

        var window = new MainWindow();
        window.Show();
    }
    
    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int SetCurrentProcessExplicitAppUserModelID(string AppID);
}

