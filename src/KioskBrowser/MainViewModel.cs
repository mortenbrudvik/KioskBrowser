using System.IO;
using System.Windows.Media.Imaging;
using Windows.Services.Store;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace KioskBrowser;

public partial class MainViewModel(Action close) : ObservableObject
{
    private readonly StoreUpdateHelper _storeUpdateHelper = new();
    
    [ObservableProperty]
    private string _title = "Kiosk Browser";
    
    [ObservableProperty]
    private BitmapImage _titlebarIcon = new(new Uri("pack://application:,,,/Images/app.png"));
    
    [ObservableProperty] 
    private BitmapImage? _taskbarOverlayImage;
    
    public string CacheFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KioskBrowser");

    public bool IsUpdateAvailable => true;//_storeUpdateHelper.IsUpdateAvailableAsync().Result;
    
    [RelayCommand]
    private void Close() => close();

    [RelayCommand]
    private void ShowAboutPage()
    {
        
    }

    public bool RefreshContentEnabled { get; set; }
    public double RefreshContentIntervalInSeconds { get; set; }
    public bool TitlebarEnabled { get; set; }
}