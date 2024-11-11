using System.Windows.Controls;

namespace KioskBrowser;

public partial class AboutPage : Page
{
    public AboutPage(NavigationService navigationService)
    {
        DataContext = new AboutViewModel(new StoreUpdateHelper(), navigationService);
        InitializeComponent();
    }
}