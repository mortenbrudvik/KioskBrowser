using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using CommandLine;
using Microsoft.Web.WebView2.Core;

namespace KioskBrowser
{
    public partial class MainWindow : Window
    {
        private readonly string _cacheFolderPath;
        private DispatcherTimer _refreshContentTimer;
        private double _contentRefreshIntervalInSeconds = 60;

        public MainWindow()
        {
            InitializeComponent();

            DataContext =  new MainViewModel(CloseWindow);
            _cacheFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KioskBrowser");
        }

        private bool RefreshContentEnabled { get; set; }

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
                });
            
            SetButtonStates();
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            
            if (WebView2Install.GetInfo().InstallType == InstallType.NotInstalled)
                return;
            
            var args = Environment.GetCommandLineArgs();

            if (args.Length < 2)
            {
                Shutdown("No parameters. Browser window will close.");
                return;
            }
            var url = args[1];

            try
            {
                var webView2Environment = await CoreWebView2Environment.CreateAsync(null, _cacheFolderPath);
                await kioskBrowser.EnsureCoreWebView2Async(webView2Environment);
                
                kioskBrowser.Source = new UriBuilder(url).Uri;
                
                if(RefreshContentEnabled)
                    StartAutomaticContentRefresh();
            }
            catch (Exception)
            {
                Shutdown("An error occurred when starting the browser. Browser window will close.", "Error Occurred");
            }
        }

        private void StartAutomaticContentRefresh()
        {
            _refreshContentTimer = new DispatcherTimer();
            _refreshContentTimer.Tick += (_, _) => kioskBrowser.Reload();
            _refreshContentTimer.Interval = TimeSpan.FromSeconds(_contentRefreshIntervalInSeconds);
            _refreshContentTimer.Start();
        }

        private void CloseWindow()
        {
            if( Titlebar.Visibility != Visibility.Visible)
                Application.Current.Shutdown();
        }

        private void Shutdown(string message, string caption = "Information")
        {
            MessageBox.Show(this, message, caption);
            Application.Current.Shutdown();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://go.microsoft.com/fwlink/p/?LinkId=2124703",
                UseShellExecute = true
            });
        }

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

        private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e) => Close();
    }
    
    public class Options
    {
        [Option('t', "enable-titlebar",
            Required = false, Default = false, HelpText = "Enable Title bar")]
        public bool EnableTitlebar { get; set; }
        
        [Option('r', "enable-content-refresh",
            Required = false, Default = false, HelpText = "Enable automatic content refresh every 60 seconds")]
        public bool EnableAutomaticContentRefresh { get; set; }

    }
}
