using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommandLine;
using Microsoft.Web.WebView2.Core;

namespace KioskBrowser;

public partial class MainWindow
{
    private readonly DispatcherTimer _refreshContentTimer = new();
    public MainWindow()
    {
        InitializeComponent();

        DataContext =  new MainViewModel(CloseWindow);
    }

    private static string CacheFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KioskBrowser");
    private bool RefreshContentEnabled { get; set; }
    private double RefreshContentIntervalInSeconds { get; set; }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        var args = Environment.GetCommandLineArgs();
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                if (!o.EnableTitlebar)
                    Titlebar.Visibility = Visibility.Collapsed;

                RefreshContentEnabled = o.EnableAutomaticContentRefresh;
                RefreshContentIntervalInSeconds = Math.Max(Math.Min(o.ContentRefreshIntervalInSeconds, 3600), 10);
            });
            
        SetButtonStates();
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        KeyDown += (_, eventArgs) => {
            if (eventArgs.Key == Key.Escape && Titlebar.Visibility != Visibility.Visible)
                CloseWindow(); };
            
        if (WebView2Install.GetInfo().InstallType == InstallType.NotInstalled)
            return;
            
        var args = Environment.GetCommandLineArgs();

        if (args.Length < 2)
        {
            Shutdown("Information","No parameters. Browser window will close.");
            return;
        }
        var url = args[1];

        try
        {
            var environment = await CoreWebView2Environment.CreateAsync(null, CacheFolderPath);
            await WebView.EnsureCoreWebView2Async(environment);
                
            WebView.Source = new UriBuilder(url).Uri;
                
            if(RefreshContentEnabled)
                StartAutomaticContentRefresh();
        }
        catch (Exception)
        {
            Shutdown("Error Occurred", "An error occurred when starting the browser. Browser window will close.");
        }
    }

    private void StartAutomaticContentRefresh()
    {
        _refreshContentTimer.Tick += (_, _) => WebView.Reload();
        _refreshContentTimer.Interval = TimeSpan.FromSeconds(RefreshContentIntervalInSeconds);
        _refreshContentTimer.Start();
    }

    private void CloseWindow()
    {
        if( Titlebar.Visibility != Visibility.Visible)
            Application.Current.Shutdown();
    }

    private void Shutdown(string caption, string message)
    {
        MessageBox.Show(this, message, caption);
        Application.Current.Shutdown();
    }

    private void Hyperlink_OnClick(object sender, RoutedEventArgs e) =>
        Process.Start(new ProcessStartInfo {
            FileName = "https://go.microsoft.com/fwlink/p/?LinkId=2124703",
            UseShellExecute = true});

    private void OnMinimizeButtonClick(object sender, RoutedEventArgs e) =>
        WindowState = WindowState.Minimized;

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);

        SetButtonStates();
    }

    private void SetButtonStates()
    {
        restoreButton.Visibility = WindowState == WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
        maximizeButton.Visibility = WindowState == WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
    }

    private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e) =>
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void OnCloseButtonClick(object sender, RoutedEventArgs e) => Close();
}
    
public class Options
{

    [Option('t', "enable-titlebar", Required = false, Default = false, HelpText = "Enable Title bar")]
    public bool EnableTitlebar { get; set; }
        
    [Option('r', "enable-content-refresh", Required = false, Default = false, HelpText = "(default: 60 seconds) Enable automatic refresh of content")]
    public bool EnableAutomaticContentRefresh { get; set; }
        
    [Option("content-refresh-interval", Required = false, Default = 60, HelpText = "(min: 10, max: 3600) Content refresh interval in seconds")]
    public int ContentRefreshIntervalInSeconds { get; set; }
}