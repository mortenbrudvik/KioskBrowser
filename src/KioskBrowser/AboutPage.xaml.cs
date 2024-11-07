using System.Windows;
using System.Windows.Controls;

namespace KioskBrowser;

public partial class AboutPage : Page
{
    private readonly NavigationService _navigationService;
    private readonly StoreUpdateHelper _storeUpdateHelper = new();

    public AboutPage(NavigationService navigationService)
    {
        _navigationService = navigationService;
        DataContext = this;
        InitializeComponent();
    }
    
    public string CurrentVersionText => "Swift Kiosk Browser " + AppSettings.Version;
    public bool IsUpdateAvailable => _storeUpdateHelper.IsUpdateAvailable();
    public string UpdateAvailableText => IsUpdateAvailable ? "An update is available" : "You are up to date";
    
    private void OnGoToMainPage(object sender, RoutedEventArgs e)
    {
        _navigationService.Navigate<BrowserPage>();
    }

    private async void OnUpdate(object sender, RoutedEventArgs e)
    {
        await _storeUpdateHelper.DownloadAndInstallAllUpdatesAsync();
    }
}
