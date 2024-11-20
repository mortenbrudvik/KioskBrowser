using System.Windows.Controls;

namespace KioskBrowser;

public partial class AboutPage : Page
{
    private readonly AboutViewModel _viewModel;

    public AboutPage(NavigationService navigationService, ILogger logger)
    {
        _viewModel = new AboutViewModel(navigationService, logger);
        DataContext = _viewModel;
            
        InitializeComponent();
    }

    protected override async void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        
        await _viewModel.InitializeAsync();
    }
}