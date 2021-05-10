using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace KioskBrowser
{
    public partial class MainWindow : Window
    {
        private readonly string _cacheFolderPath;

        public MainWindow()
        {
            InitializeComponent();

            DataContext =  new MainViewModel();
            _cacheFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KioskBrowser");
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            var args = Environment.GetCommandLineArgs();

            if (args.Length != 2)
            {
                Shutdown("Missing url parameter. Browser window will close.");
                return;
            }
            var url = args[1];

            try
            {
                var webView2Environment = await CoreWebView2Environment.CreateAsync(null, _cacheFolderPath);
                await kioskBrowser.EnsureCoreWebView2Async(webView2Environment);
                
                kioskBrowser.Source = new UriBuilder(url).Uri;
            }
            catch (Exception)
            {
                Shutdown("An error occurred when starting the browser. Browser window will close.", "Error Occurred");
            }
        }

        private void Shutdown(string message, string caption = "Information")
        {
            MessageBox.Show(this, message, caption);
            Application.Current.Shutdown();
        }

        private static bool IsUriValid(string uriName) =>
            Uri.TryCreate(uriName, UriKind.Absolute, out var uriResult) 
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://go.microsoft.com/fwlink/p/?LinkId=2124703",
                UseShellExecute = true
            });
        }
    }
}
