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

    private void OnGoToMainPage(object sender, RoutedEventArgs e)
    {
        _navigationService.Navigate<BrowserPage>();
    }

    public string CurrentVersionText => "Swift Kiosk Browser " + AppSettings.Version;

    public string UpdateAvailableText => _storeUpdateHelper.IsUpdateAvailableAsync().Result ? "An update is available" : "You are up to date";
}