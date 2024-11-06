using Windows.Foundation;
using Windows.Services.Store;

namespace KioskBrowser;

/// <summary>
/// Handles downloading and installing app updates from the Microsoft Store.
/// </summary>
public class StoreUpdateHelper
{
    private readonly StoreContext _context = StoreContext.GetDefault();

    public async Task<bool> IsUpdateAvailableAsync()
    {
        try
        {
            IReadOnlyList<StorePackageUpdate> updates = await _context.GetAppAndOptionalStorePackageUpdatesAsync();
            return updates.Any();
        }
        catch (Exception e)
        {
            SimpleLogger.LogError(e, "Failed to check for updates.");
        }

        return false;
    } 
    
    /// <summary>
    /// Downloads and installs all available package updates in separate steps.
    /// </summary>
    public async Task<bool> DownloadAndInstallAllUpdatesAsync()
    {
        Log("Get any available updates.");

        // Get the updates that are available.
        IReadOnlyList<StorePackageUpdate> updates = await _context.GetAppAndOptionalStorePackageUpdatesAsync();
        
        if (updates.Any())
        {
            Log("Download Updates.");
            var downloaded = await DownloadPackageUpdatesAsync(updates);

            if (downloaded)
            {
                Log("Install Updates.");
                var installed = await InstallPackageUpdatesAsync(updates);

                if (!installed)
                {
                    Log("Failed to install update.");
                }
                
                return !installed;
            }

            Log("Failed to download updates.");
        }

        return false;
    }

    /// <summary>
    /// Downloads package updates.
    /// </summary>
    /// <param name="updates">The package updates to download.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    private async Task<bool> DownloadPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
    {
        try
        {
            IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation =
                _context.RequestDownloadStorePackageUpdatesAsync(updates);

            // downloadOperation.Progress = (asyncInfo, progress) =>
            // {
            //
            // };

            var result = await downloadOperation.AsTask();

            if (result.OverallState == StorePackageUpdateState.Completed)
            {
                return true;
            }

            HandleFailedUpdates(updates, result.StorePackageUpdateStatuses);
            return false;
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to download updates.");
            return false;
        }
    }

    /// <summary>
    /// Installs package updates.
    /// </summary>
    /// <param name="updates">The package updates to install.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    private async Task<bool> InstallPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
    {
        try
        {
            IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> installOperation =
                _context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

            var result = await installOperation.AsTask();

            if (result.OverallState == StorePackageUpdateState.Completed)
            {
                return true;
            }

            HandleFailedUpdates(updates, result.StorePackageUpdateStatuses);
            return false;
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to install updates.");
            return false;
        }
    }

    /// <summary>
    /// Handles failed package updates by checking for mandatory updates and taking appropriate actions.
    /// </summary>
    /// <param name="updates">The original list of updates requested.</param>
    /// <param name="updateStatuses">The statuses of the updates after the operation.</param>
    private void HandleFailedUpdates(IEnumerable<StorePackageUpdate> updates, IEnumerable<StorePackageUpdateStatus> updateStatuses)
    {
        var failedUpdates = updateStatuses
            .Where(status => status.PackageUpdateState != StorePackageUpdateState.Completed)
            .ToList();

        if (failedUpdates.Any())
        {
            var failedMandatoryUpdates = updates
                .Where(u => u.Mandatory)
                .Join(failedUpdates, u => u.Package.Id.FamilyName, f => f.PackageFamilyName, (u, f) => u);

            if (failedMandatoryUpdates.Any())
            {
                Log("Mandatory updates failed to install.");
            }
            else
            {
                Log("Non-mandatory updates failed to install.");
            }
        }
    }
    
    private static void Log(string message) => SimpleLogger.LogInfo(message);

    private static void LogError(Exception ex, string message) => SimpleLogger.LogError(ex, message);
}