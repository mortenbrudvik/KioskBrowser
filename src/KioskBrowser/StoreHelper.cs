using Windows.ApplicationModel;

namespace KioskBrowser;

public static class StoreHelper
{
    public static async Task<PackageInstallerInfo?> CheckForUpdates()
    {
        try
        {
            var result = await CheckForUpdateAvailability();
            var isUpdateAvailable = result is PackageUpdateAvailability.Available or PackageUpdateAvailability.Required;
            var installerPath = Package.Current.GetAppInstallerInfo().Uri.ToString();
            var storeLink = new Uri("ms-windows-store://pdp/?ProductId=" + Package.Current.Id.ProductId);
            var installedVersion = GetInstalledVersion();

            return new PackageInstallerInfo(
                installedVersion,
                isUpdateAvailable,
                new Uri(installerPath),
                storeLink);
        }
        catch (Exception)
        {
            // ignored
        }

        return null;
    }

    private static string GetInstalledVersion()
    {
        var version = Package.Current.Id.Version;
        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
    
    private static async Task<PackageUpdateAvailability> CheckForUpdateAvailability()
    {
        var result = await Package.Current.CheckUpdateAvailabilityAsync();
        return result.Availability;
    }
}

public record PackageInstallerInfo(string InstalledVersion, bool IsUpdateAvailable, Uri InstallerLink, Uri? StoreLink);

