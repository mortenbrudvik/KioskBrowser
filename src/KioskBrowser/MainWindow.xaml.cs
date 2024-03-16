using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommandLine;
using Microsoft.Web.WebView2.Core;

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

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        var args = Environment.GetCommandLineArgs();
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                if(!o.EnableTitlebar) 
                    Titlebar.Height = 0; 
                     
                _viewModel.RefreshContentEnabled = o.EnableAutomaticContentRefresh;
                _viewModel.RefreshContentIntervalInSeconds = Math.Max(Math.Min(o.ContentRefreshIntervalInSeconds, 3600), 10);
            });
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        //Close the window when the escape key is pressed (if the title bar is hidden)
        KeyDown += (_, eventArgs) =>
        {
            if (eventArgs.Key == Key.Escape && Titlebar.Height == 0)
                Close();
        };

        var args = Environment.GetCommandLineArgs();

        if (args.Length < 2)
        {
            MessageBox.Show(this, "Information", "No parameters. Browser window will close.");
            Application.Current.Shutdown();
            return;
        }

        var url = args[1];

        try
        {
            var environment = await CoreWebView2Environment.CreateAsync(null, _viewModel.CacheFolderPath);
            await WebView.EnsureCoreWebView2Async(environment);
                
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

    private void StartAutomaticContentRefresh()
    {
        _refreshContentTimer.Tick += (_, _) => WebView.Reload();
        _refreshContentTimer.Interval = TimeSpan.FromSeconds(_viewModel.RefreshContentIntervalInSeconds);
        _refreshContentTimer.Start();
    }
}