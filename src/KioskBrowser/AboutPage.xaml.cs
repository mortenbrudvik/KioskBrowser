using System.Windows;
using System.Windows.Controls;

namespace KioskBrowser;

public partial class AboutPage : Page
{
    private readonly NavigationService _navigationService;

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
}