using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KioskBrowser.Common;

namespace KioskBrowser;

public partial class MainViewModel(Action close, NavigationService navigationService) : ObservableObject
{
    private readonly StoreUpdateHelper _storeUpdateHelper = new();
    
    [ObservableProperty]
    private string _title = "Kiosk Browser";
    
    [ObservableProperty]
    private int _titlebarHeight;

    [ObservableProperty]
    private BitmapImage _titlebarIcon = new(new Uri("pack://application:,,,/Images/app.png"));
    
    [ObservableProperty] 
    private BitmapImage? _taskbarOverlayImage;
    
    public string CacheFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KioskBrowser");

    public bool IsUpdateAvailable => false;//_storeUpdateHelper.IsUpdateAvailableAsync().Result;
    
    [RelayCommand]
    private void Close() => close();

    [RelayCommand]
    private void ShowAboutPage()
    {
        navigationService.Navigate<AboutPage>();
    }
    
    public void Initialize(Options options)
    {
        Url = options.Url ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "readme.html");;
        TitlebarEnabled = options.Url is not null && options.EnableTitlebar;
        TitlebarHeight = options.EnableTitlebar ? 48 : 0;
        RefreshContentEnabled = options.EnableAutomaticContentRefresh;
        RefreshContentIntervalInSeconds = Math.Max(Math.Min(options.ContentRefreshIntervalInSeconds, 3600), 10);
        
        SetIcons(Url);
    }
    
    private void SetIcons(string url)
    {
        if (!FileUtils.IsFilePath(url)) return;
        
        var image = FileUtils.GetFileIcon(url);
        if(image == null) return;
            
        TitlebarIcon = image;
        TaskbarOverlayImage = image;
    }

    public bool RefreshContentEnabled { get; private set; }
    public double RefreshContentIntervalInSeconds { get; private set; }
    public bool TitlebarEnabled { get; private set; } = true;
    
    public string Url { get; set; }
}