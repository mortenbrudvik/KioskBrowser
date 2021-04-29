using System;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace KioskBrowser
{
    public partial class WebWindow : Window
    {
        private string _url;
        private string _html;

        public WebWindow()
        {
            InitializeComponent();
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            try
            {
                var webView2Environment = await CoreWebView2Environment.CreateAsync();
                await WpfBrowser.EnsureCoreWebView2Async(webView2Environment);

                if(!string.IsNullOrEmpty(_url))
                    WpfBrowser.Source = new Uri(_url);
                else if(!string.IsNullOrEmpty(_html))
                    WpfBrowser.NavigateToString(_html);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void ShowFromUrl(string url)
        {
            _url = url;
            Show();
        }

        public void ShowFromHtml(string html)
        {
            _html = html;
            Show();
        }
    }
}