using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace KioskBrowser;

public partial class AboutViewModel(NavigationService navigationService, StoreService storeService) : ObservableObject
{
    public async Task InitializeAsync()
    {
        IsUpdateAvailable = await storeService.IsUpdateAvailableAsync();
        UpdateAvailableText = IsUpdateAvailable ? "An update is available" : "You are up to date";
    }
    
    public string CurrentVersionText => "Swift Kiosk Browser " + AppSettings.Version;

    [ObservableProperty]
    private bool _isUpdateAvailable = false;
    
    [ObservableProperty]
    private string _updateAvailableText = "You are up to date";

    [ObservableProperty] private string _updateCheckMessage = "";
    [ObservableProperty] private string _updateCheckSeverity = "Success";
    [ObservableProperty] private string _isUpdateCheck = "False";
    
    [RelayCommand]
    private async Task CheckForUpdate()
    {
        IsUpdateAvailable = await storeService.IsUpdateAvailableAsync();
        
        if(IsUpdateAvailable)
        {
            ShowUpdateInfoBox("An update is available", "Success");
        }
        else
        {
            ShowUpdateInfoBox("No new updates available", "Success");
        }
    }
    
    [RelayCommand]
    private void NavigateToMainPage()
    {
        navigationService.Navigate<BrowserPage>();
    }
    
    private void ShowUpdateInfoBox(string message, string severity)
    {
        UpdateCheckMessage = message;
        UpdateCheckSeverity = severity;
        IsUpdateCheck = "True";
    }
}