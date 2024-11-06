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
        _viewModel = new MainViewModel(Close);

        InitializeComponent();
        
        DataContext = _viewModel;
        
        _webView = new WebView2();
        _webView.Loaded += async (_, _) => await InitializeWebView();
        
        var browserPage = new BrowserPage(_webView);
        
        _navigationService = new NavigationService(); 
        _navigationService.AddPage(browserPage);
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

        var args = Environment.GetCommandLineArgs();
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                if(!o.EnableTitlebar)
                    Titlebar.Height = 0;

                _viewModel.TitlebarEnabled = o.EnableTitlebar;
                _viewModel.RefreshContentEnabled = o.EnableAutomaticContentRefresh;
                _viewModel.RefreshContentIntervalInSeconds = Math.Max(Math.Min(o.ContentRefreshIntervalInSeconds, 3600), 10);
            });
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        _navigationService.Navigate<BrowserPage>();
    }

    private async Task InitializeWebView()
    {
        var args = Environment.GetCommandLineArgs();
        
        var readmeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "readme.html");

        var url = args.Length < 2 ? readmeFilePath : args[1];
        
        try
        {
            var environment = await CoreWebView2Environment.CreateAsync(null, _viewModel.CacheFolderPath);
            await _webView.EnsureCoreWebView2Async(environment);
            
            if(FileUtils.IsFilePath(url))
            {
                var image = FileUtils.GetFileIcon(url);
                _viewModel.TitlebarIcon = image;
                _viewModel.TaskbarOverlayImage = image;
            }

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

            _webView.Source = new UriBuilder(url).Uri;

            if (_viewModel.RefreshContentEnabled)
                StartAutomaticContentRefresh();
        }
        catch (Exception)
        {
            MessageBox.Show(this, "Error Occurred", "An error occurred when starting the browser. Browser window will close.");
            Application.Current.Shutdown();
        }
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
