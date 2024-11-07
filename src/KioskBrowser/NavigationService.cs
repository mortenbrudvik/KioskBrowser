using System.Windows.Controls;
using System.Windows.Navigation;

namespace KioskBrowser;

public class NavigationService
{
    private Frame _navigationFrame = null!;
    private readonly Dictionary<Type, object> _pages = new();
    
    public void AddPage<TPage>(TPage page) where TPage : Page
    {
        _pages.Add(typeof(TPage), page);
    }
    
    public void SetNavigationFrame(Frame frame)
    {
        _navigationFrame = frame;
        _navigationFrame.Navigated += OnNavigated;
    }
    
    public void Navigate<TPage>() where TPage : Page
    {
        if (_pages.TryGetValue(typeof(TPage), out object? page))
        {
            _navigationFrame.Navigate(page);
        }
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        _navigationFrame.NavigationService.RemoveBackEntry();
    }
    
    public void Dispose()
    {
        _navigationFrame.Navigated -= OnNavigated;
    }
}
