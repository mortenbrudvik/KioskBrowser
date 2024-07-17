using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Input;

namespace KioskBrowser;

public class MainViewModel(Action close) : ViewModelBase
{
    private string _title = "Kiosk Browser";
    private BitmapImage? _titlebarIcon = new(new Uri("pack://application:,,,/Images/app.png"));
    private BitmapImage? _taskbarOverlayImage;

    public RelayCommand CloseCommand { get; set; } = new(close);

    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }    
    
    public BitmapImage? TitlebarIcon
    {
        get => _titlebarIcon;
        set => SetField(ref _titlebarIcon, value);
    }
    
    public BitmapImage? TaskbarOverlayImage
    {
        get => _taskbarOverlayImage;
        set => SetField(ref _taskbarOverlayImage, value);
    }

    public string CacheFolderPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KioskBrowser");

    public bool RefreshContentEnabled { get; set; }
    public double RefreshContentIntervalInSeconds { get; set; }
    public bool TitlebarEnabled { get; set; }
}