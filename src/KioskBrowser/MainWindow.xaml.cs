using System.Windows;
using System.Windows.Interop;
using CommandLine;
using static KioskBrowser.Native.ShellHelper;

namespace KioskBrowser;

public partial class MainWindow
{
    private readonly MainViewModel _viewModel;
    private readonly NavigationService _navigationService;
    private readonly UpdateCheckerService _updateCheckerService;

    public MainWindow()
    {
        _navigationService = new NavigationService();
        var logger = new LoggerService();
        _updateCheckerService = new UpdateCheckerService(logger);
        _updateCheckerService.AppUpdated += UpdateCheckerService_AppUpdated;
        _updateCheckerService.Start(10);
        
        _viewModel = new MainViewModel(Close, _navigationService, logger, _updateCheckerService);
        DataContext = _viewModel;
        
        InitializeComponent();
    }
    
    private void UpdateCheckerService_AppUpdated(object? sender, EventArgs e)
    {
        Dispatcher.Invoke( () =>
        {
            var result = MessageBox.Show(
                "A new version of the application has been installed. Do you want to restart now to apply updates?",
                "Update Available",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                AppRestartHelper.RestartApplication();
            }
        });
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        _navigationService.SetNavigationFrame(MainFrame);
        
        ParseCommandLine();
    }

    private void ParseCommandLine()
    {
        var args = Environment.GetCommandLineArgs().Skip(1);
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o => _viewModel.Initialize(o));
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        _navigationService.Navigate<BrowserPage>();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var hwnd = new WindowInteropHelper(this).Handle;

        SetAppUserModelId(hwnd, "KioskBrowser" + Guid.NewGuid());
    }
}