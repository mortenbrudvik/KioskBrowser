using System.Windows.Interop;
using CommandLine;
using static KioskBrowser.Native.ShellHelper;

namespace KioskBrowser;

public partial class MainWindow
{
    private readonly MainViewModel _viewModel;
    private readonly NavigationService _navigationService;

    public MainWindow()
    {
        _navigationService = new NavigationService();
        _viewModel = new MainViewModel(Close, _navigationService);
        DataContext = _viewModel;
        
        InitializeComponent();
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