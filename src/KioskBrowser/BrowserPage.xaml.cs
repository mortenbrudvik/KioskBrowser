using System.Windows.Controls;
using Microsoft.Web.WebView2.Wpf;

namespace KioskBrowser;

public partial class BrowserPage : Page
{
    public BrowserPage(WebView2 webView)
    {
        InitializeComponent();
        
        DataContext = this;
        WebView = webView;
    }

    public WebView2 WebView { get; set; }
}