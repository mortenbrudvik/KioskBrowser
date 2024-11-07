using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using CommandLine;
using KioskBrowser.Common;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using static KioskBrowser.Native.ShellHelper;

namespace KioskBrowser;

public partial class MainWindow
{
    private readonly DispatcherTimer _refreshContentTimer = new();
    private readonly MainViewModel _viewModel;
    private readonly WebView2 _webView;
    private readonly NavigationService _navigationService;

    public MainWindow()
    {
        _navigationService = new NavigationService(); 
        _webView = new WebView2();
        
        _webView.Loaded += async (_, _) => await InitializeWebView();
        
        var browserPage = new BrowserPage(_webView);
        var aboutPage = new AboutPage(_navigationService);

        _navigationService.AddPage(browserPage);
        _navigationService.AddPage(aboutPage);
        
        _viewModel = new MainViewModel(Close, _navigationService);

        InitializeComponent();
        
        DataContext = _viewModel;
        
        _navigationService.SetNavigationFrame(MainFrame);
    }
    
    private new void Close()
    {
        if(!_viewModel.TitlebarEnabled)
            base.Close();
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        var args = Environment.GetCommandLineArgs().Skip(1);
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                if(!o.EnableTitlebar)
                    Titlebar.Height = 0;
                
                _viewModel.Initialize(o);
            });
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        _navigationService.Navigate<BrowserPage>();
    }

    private async Task InitializeWebView()
    {
        if(_webView.CoreWebView2 != null)
            return;
    
        var environment = await CoreWebView2Environment.CreateAsync(null, _viewModel.CacheFolderPath);
        await _webView.EnsureCoreWebView2Async(environment);
        
        if(_webView.CoreWebView2 == null)
            throw new Exception("Failed to initialize WebView control. Please restart application.");

        _webView.CoreWebView2.DocumentTitleChanged += (_, _) =>
        {
            var title = _webView.CoreWebView2.DocumentTitle;
            if(!string.IsNullOrEmpty(title))
                _viewModel.Title = title;
        };

        _webView.CoreWebView2.FaviconChanged += async (_, _) =>
        {
            var faviconUri = _webView.CoreWebView2.FaviconUri;
            if (faviconUri == null) return;

            var image = await FaviconIcon.DownloadAsync(faviconUri);
            if (image == null) return;
            
            _viewModel.TitlebarIcon = image;
            _viewModel.TaskbarOverlayImage = image;
        };

        _webView.Source = new UriBuilder(_viewModel.Url).Uri;

        if (_viewModel.RefreshContentEnabled)
            StartAutomaticContentRefresh();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        
        var hwnd = new WindowInteropHelper(this).Handle;
        
        SetAppUserModelId(hwnd, "KioskBrowser" + Guid.NewGuid());
    }

    private void StartAutomaticContentRefresh()
    {
        _refreshContentTimer.Tick += (_, _) => _webView.Reload();
        _refreshContentTimer.Interval = TimeSpan.FromSeconds(_viewModel.RefreshContentIntervalInSeconds);
        _refreshContentTimer.Start();
    }
}
