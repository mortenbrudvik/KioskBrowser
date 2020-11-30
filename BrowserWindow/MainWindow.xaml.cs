using System;
using System.Windows;
using System.Windows.Input;

namespace BrowserWindow
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

        private void DockPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Application.Current.Shutdown();
        }
    }
}
