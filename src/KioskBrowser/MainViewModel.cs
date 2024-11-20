using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KioskBrowser.Common;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace KioskBrowser;

public partial class MainViewModel(Action close, NavigationService navigationService, ILogger logger, UpdateCheckerService updateCheckerService) : ObservableObject
{
    private readonly DispatcherTimer _refreshContentTimer = new();
    private readonly WebView2 _webView = new();
    
    private string Url { get; set; } = default!;
    private static string CacheFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KioskBrowser");
    private bool RefreshContentEnabled { get; set; }
    private double RefreshContentIntervalInSeconds { get; set; }
    
    [ObservableProperty]
    private string _title = "Kiosk Browser";
    
    [ObservableProperty]
    private int _titlebarHeight;

    [ObservableProperty]
    private BitmapImage _titlebarIcon = new(new Uri("pack://application:,,,/Images/app.png"));
    
    [ObservableProperty] 
    private BitmapImage? _taskbarOverlayImage;

    public bool TitlebarEnabled { get; private set; } = true;

    public bool IsUpdateAvailable => false;//_storeUpdateHelper.IsUpdateAvailableAsync().Result;
    
    [RelayCommand]
    private void Close()
    {
        if (!TitlebarEnabled)
            close();
    }

    [RelayCommand]
    private void ShowAboutPage()
    {
        navigationService.Navigate<AboutPage>();
    }
    
    public void Initialize(Options options)
    {
        Url = options.Url ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "readme.html");;
        TitlebarEnabled = options.Url is not null && options.EnableTitlebar;
        TitlebarHeight = options.EnableTitlebar ? 48 : 0;
        RefreshContentEnabled = options.EnableAutomaticContentRefresh;
        RefreshContentIntervalInSeconds = Math.Max(Math.Min(options.ContentRefreshIntervalInSeconds, 3600), 10);
        
        SetIcons(Url);
        
        RegisterPages();
        
        _webView.Loaded += async (_, _) => await InitializeWebView();
    }

    private async Task InitializeWebView()
    {
        if(_webView.CoreWebView2 != null)
            return;
    
        var environment = await CoreWebView2Environment.CreateAsync(null, CacheFolderPath);
        await _webView.EnsureCoreWebView2Async(environment);
        
        if(_webView.CoreWebView2 == null)
            throw new Exception("Failed to initialize WebView control. Please restart application.");

        _webView.CoreWebView2.DocumentTitleChanged += (_, _) =>
        {
            var title = _webView.CoreWebView2.DocumentTitle;
            if(!string.IsNullOrEmpty(title))
                Title = title;
        };

        _webView.CoreWebView2.FaviconChanged += async (_, _) =>
        {
            var faviconUri = _webView.CoreWebView2.FaviconUri;
            if (faviconUri == null) return;

            var image = await FaviconIcon.DownloadAsync(faviconUri);
            if (image == null) return;
            
            TitlebarIcon = image;
            TaskbarOverlayImage = image;
        };

        _webView.Source = new UriBuilder(Url).Uri;

        if (RefreshContentEnabled)
            StartAutomaticContentRefresh();
    }

    private void RegisterPages()
    {
        var browserPage = new BrowserPage(_webView);
        var aboutPage = new AboutPage(navigationService, logger);

        navigationService.AddPage(browserPage);
        navigationService.AddPage(aboutPage);
    }
    
    private void SetIcons(string url)
    {
        if (!FileUtils.IsFilePath(url)) return;
        
        var image = FileUtils.GetFileIcon(url);
        if(image == null) return;
            
        TitlebarIcon = image;
        TaskbarOverlayImage = image;
    }
    
    private void StartAutomaticContentRefresh()
    {
        _refreshContentTimer.Tick += (_, _) => _webView.Reload();
        _refreshContentTimer.Interval = TimeSpan.FromSeconds(RefreshContentIntervalInSeconds);
        _refreshContentTimer.Start();
    }
}