using System;
using System.Diagnostics;
using System.Windows;
using KioskBrowser;

namespace BrowserWindow
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext =  new MainViewModel();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            var args = Environment.GetCommandLineArgs();

            if (args.Length != 2)
            {
                Shutdown("Missing url parameter. Browser window will close.");
                return;
            }
            var url = args[1];
            if (!IsUriValid(url))
            {
                Shutdown("Url is not on a valid format. Browser window will close.");
                return;
            }

            try
            {
                slimBrowser.Source = new Uri(url);
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
