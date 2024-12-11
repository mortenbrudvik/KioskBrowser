using System.Windows.Controls;

namespace KioskBrowser;

public partial class AboutPage : Page
{
    private readonly AboutViewModel _viewModel;

    public AboutPage(NavigationService navigationService, StoreService storeService)
    {
        _viewModel = new AboutViewModel(navigationService, storeService);
        DataContext = _viewModel;
            
        InitializeComponent();
    }

    protected override async void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        
        await _viewModel.InitializeAsync();
    }
}