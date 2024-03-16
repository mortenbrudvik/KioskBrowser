using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Input;

namespace KioskBrowser;

public class MainViewModel : ViewModelBase
{
    private string _title = "Kiosk Browser";
    private BitmapImage? _favicon;

    public MainViewModel(Action close)
    {
        CloseCommand = new RelayCommand(close);
        _favicon = LoadIcon("pack://application:,,,/Images/app.png");
    }

    public RelayCommand CloseCommand { get; set; }

    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }    
    
    public BitmapImage? TitlebarIcon
    {
        get => _favicon;
        set => SetField(ref _favicon, value);
    }
    
    public BitmapImage? TaskbarOverlayImage => null;
    
    public string CacheFolderPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KioskBrowser");

    public bool RefreshContentEnabled { get; set; }
    public double RefreshContentIntervalInSeconds { get; set; }
    
    
    private BitmapImage LoadIcon(string iconPath)
    {
        var iconUri = new Uri(iconPath);
        var image = new BitmapImage(iconUri);
        return image;
    }
}