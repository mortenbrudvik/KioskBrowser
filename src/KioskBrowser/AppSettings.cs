using Windows.ApplicationModel;

namespace KioskBrowser;

public static class AppSettings
{
    public static string MicrosoftStoreLink => "ms-windows-store://pdp/?ProductId=9NT0K2DFKHW8";

    public static string Version
    {
        get
        {
            try
            {
                var version = Package.Current.Id.Version;
                return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
            catch (Exception)
            {
                return "Local Version";
            }
        }
    }
}

