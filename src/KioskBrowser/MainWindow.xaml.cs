using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using CommandLine;
using KioskBrowser.Common;
using Microsoft.Web.WebView2.Core;
using static KioskBrowser.Native.ShellHelper;

namespace KioskBrowser;

public partial class MainWindow
{
    private readonly DispatcherTimer _refreshContentTimer = new();
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        _viewModel = new MainViewModel(Close);

        InitializeComponent();
        
        DataContext = _viewModel;
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

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        var args = Environment.GetCommandLineArgs();

        
        var readmeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "readme.html");

        var url = args.Length < 2 ? readmeFilePath : args[1];
        
        try
        {
            var environment = await CoreWebView2Environment.CreateAsync(null, _viewModel.CacheFolderPath);
            await WebView.EnsureCoreWebView2Async(environment);
            
            if(FileUtils.IsFilePath(url))
            {
                var image = FileUtils.GetFileIcon(url);
                _viewModel.TitlebarIcon = image;
                _viewModel.TaskbarOverlayImage = image;
            }

            WebView.CoreWebView2.DocumentTitleChanged += (_, _) =>
            {
                var title = WebView.CoreWebView2.DocumentTitle;
                if(!string.IsNullOrEmpty(title))
                    _viewModel.Title = title;
            };

            WebView.CoreWebView2.FaviconChanged += async (_, _) =>
            {
                 var faviconUri = WebView.CoreWebView2.FaviconUri;
                 if (faviconUri == null) return;

                var image = await FaviconIcon.DownloadAsync(faviconUri);
                 if (image == null) return;
                
                 _viewModel.TitlebarIcon = image;
                 _viewModel.TaskbarOverlayImage = image;
            };

            WebView.Source = new UriBuilder(url).Uri;

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
        _refreshContentTimer.Tick += (_, _) => WebView.Reload();
        _refreshContentTimer.Interval = TimeSpan.FromSeconds(_viewModel.RefreshContentIntervalInSeconds);
        _refreshContentTimer.Start();
    }
}
