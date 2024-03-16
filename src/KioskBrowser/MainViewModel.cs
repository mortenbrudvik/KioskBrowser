using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Input;

namespace KioskBrowser;

public class MainViewModel(Action close) : ViewModelBase
{
    public RelayCommand CloseCommand { get; set; } = new(close);

    public string Title => "Kiosk Browser";
    public BitmapImage? TaskbarOverlayImage => null;
    
    public string CacheFolderPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KioskBrowser");

    public bool RefreshContentEnabled { get; set; }
    public double RefreshContentIntervalInSeconds { get; set; }
}