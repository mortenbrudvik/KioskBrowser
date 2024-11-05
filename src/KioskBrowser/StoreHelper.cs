using Windows.ApplicationModel;

namespace KioskBrowser;

public static class StoreHelper
{
    public static async Task<PackageInstallerInfo> CheckForUpdates()
    {
        var result = await CheckForUpdateAvailability();
        var isUpdateAvailable = result is PackageUpdateAvailability.Available or PackageUpdateAvailability.Required;
        var installerPath = GetInstallerLink();
        var storeLink = GetStoreLink();
        var installedVersion = GetInstalledVersion();

        return new PackageInstallerInfo(
            installedVersion,
            isUpdateAvailable,
            installerPath,
            storeLink);
    }

    private static Uri? GetStoreLink()
    {
        try
        {
            return new Uri("ms-windows-store://pdp/?ProductId=" + Package.Current.Id.ProductId);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    private static Uri? GetInstallerLink()
    {
        try
        {
            return Package.Current.GetAppInstallerInfo().Uri;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static string GetInstalledVersion()
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
    
    private static async Task<PackageUpdateAvailability> CheckForUpdateAvailability()
    {
        try
        {
            var result = await Package.Current.CheckUpdateAvailabilityAsync();
            return result.Availability;
        }
        catch (Exception)
        {
            return PackageUpdateAvailability.NoUpdates;
        }
    }
}

public record PackageInstallerInfo(string InstalledVersion, bool IsUpdateAvailable, Uri? InstallerLink, Uri? StoreLink);

