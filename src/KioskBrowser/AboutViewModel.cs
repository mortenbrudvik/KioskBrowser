using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace KioskBrowser;

public partial class AboutViewModel(
    NavigationService navigationService,
    ILogger logger
    ) : ObservableObject
{
    public async Task InitializeAsync()
    {
        var storeUpdateHelper = new StoreUpdateHelper(logger);
        IsUpdateAvailable = await storeUpdateHelper.IsUpdateAvailableAsync();
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
        var storeUpdateHelper = new StoreUpdateHelper(logger);
        IsUpdateAvailable = await storeUpdateHelper.IsUpdateAvailableAsync();
        
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
    
    [RelayCommand]
    private async Task DownloadInstallerAndUpdate()
    {
        try
        {
            var storeUpdateHelper = new StoreUpdateHelper(logger);
            var success = await storeUpdateHelper.DownloadAndInstallAllUpdatesAsync();
            if (success)
            {
                ShowUpdateInfoBox("Update installed successfully. Restart the application to apply changes.", "Success");
            }
            else
            {
                ShowUpdateInfoBox("Failed to install update. Please try again later.", "Success");
            }
        }
        catch (Exception e)
        {
            ShowUpdateInfoBox("An error occurred while trying to install update", "Error");
        }
    }
    
    private void ShowUpdateInfoBox(string message, string severity)
    {
        UpdateCheckMessage = message;
        UpdateCheckSeverity = severity;
        IsUpdateCheck = "True";
    }
}