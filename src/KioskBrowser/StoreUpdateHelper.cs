using System.Windows;
using System.Windows.Interop;
using Windows.Services.Store;

namespace KioskBrowser;

     /// <summary>
    /// Handles downloading and installing app updates from the Microsoft Store.
    /// </summary>
    public class StoreUpdateHelper
    {
        private readonly StoreContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreUpdateHelper"/> class.
        /// </summary>
        /// <param name="context">The store context.</param>
        /// <param name="logger">The logger instance.</param>
        public StoreUpdateHelper(ILogger logger)
        {
            _context = StoreContext.GetDefault();
            var hwnd = new WindowInteropHelper(Application.Current.MainWindow!).Handle;
            WinRT.Interop.InitializeWithWindow.Initialize(_context, hwnd); 

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Checks if an update is available for the app.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean indicating whether an update is available.
        /// </returns>
        public async Task<bool> IsUpdateAvailableAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var updates = await _context.GetAppAndOptionalStorePackageUpdatesAsync().AsTask(cancellationToken);
                return updates.Any();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to check for updates.");
                return false;
            }
        }

        /// <summary>
        /// Downloads and installs all available package updates.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean indicating whether updates were successfully installed.
        /// </returns>
        public async Task<bool> DownloadAndInstallAllUpdatesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInfo("Checking for available updates.");

            // Get the updates that are available.
            var updates = await _context.GetAppAndOptionalStorePackageUpdatesAsync().AsTask(cancellationToken);

            if (updates == null || !updates.Any())
            {
                _logger.LogInfo("No updates are available.");
                return false;
            }

            _logger.LogInfo("Downloading updates.");

            var downloaded = await DownloadPackageUpdatesAsync(updates, cancellationToken);

            if (downloaded)
            {
                _logger.LogInfo("Installing updates.");
                var installed = await InstallPackageUpdatesAsync(updates, cancellationToken);

                if (installed)
                {
                    _logger.LogInfo("Updates installed successfully.");
                    return true;
                }
                else
                {
                    _logger.LogError("Failed to install updates.");
                    return false;
                }
            }

            _logger.LogError("Failed to download updates.");
            return false;
        }

        /// <summary>
        /// Downloads package updates.
        /// </summary>
        /// <param name="updates">The package updates to download.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean indicating success.
        /// </returns>
        private async Task<bool> DownloadPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates, CancellationToken cancellationToken)
        {
            if (updates == null) throw new ArgumentNullException(nameof(updates));
            if (!updates.Any()) return false;

            try
            {
                var progressHandler = new Progress<StorePackageUpdateStatus>(status =>
                {
                    _logger.LogInfo($"Downloading update for package: {status.PackageFamilyName}, State: {status.PackageUpdateState}, Progress: {status.PackageDownloadProgress * 100}%");
                });

                var downloadOperation = _context.RequestDownloadStorePackageUpdatesAsync(updates).AsTask(cancellationToken, progressHandler);

                var result = await downloadOperation;

                if (result.OverallState == StorePackageUpdateState.Completed)
                {
                    _logger.LogInfo("All updates downloaded successfully.");
                    return true;
                }

                HandleFailedUpdates(updates, result.StorePackageUpdateStatuses);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download updates.");
                throw;
            }
        }

        /// <summary>
        /// Installs package updates.
        /// </summary>
        /// <param name="updates">The package updates to install.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean indicating success.
        /// </returns>
        private async Task<bool> InstallPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates, CancellationToken cancellationToken)
        {
            if (updates == null) throw new ArgumentNullException(nameof(updates));
            if (!updates.Any()) return false;

            try
            {
                var progressHandler = new Progress<StorePackageUpdateStatus>(status =>
                {
                    _logger.LogInfo($"Installing update for package: {status.PackageFamilyName}, State: {status.PackageUpdateState}");
                });

                var installOperation = _context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates).AsTask(cancellationToken, progressHandler);

                var result = await installOperation;

                if (result.OverallState == StorePackageUpdateState.Completed)
                {
                    _logger.LogInfo("All updates installed successfully.");
                    return true;
                }

                HandleFailedUpdates(updates, result.StorePackageUpdateStatuses);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to install updates.");
                throw;
            }
        }

        /// <summary>
        /// Handles failed package updates by checking for mandatory updates and taking appropriate actions.
        /// </summary>
        /// <param name="updates">The original list of updates requested.</param>
        /// <param name="updateStatuses">The statuses of the updates after the operation.</param>
        private void HandleFailedUpdates(IEnumerable<StorePackageUpdate> updates, IEnumerable<StorePackageUpdateStatus> updateStatuses)
        {
            _logger.LogInfo("Handling failed updates.");

            var failedUpdates = updateStatuses
                .Where(status => status.PackageUpdateState != StorePackageUpdateState.Completed)
                .ToList();

            if (!failedUpdates.Any())
            {
                _logger.LogInfo("No failed updates to handle.");
                return;
            }

            foreach (var status in failedUpdates)
            {
                _logger.LogError($"Update failed for package: {status.PackageFamilyName}");
            }

            var failedMandatoryUpdates = updates
                .Where(u => u.Mandatory)
                .Join(failedUpdates, u => u.Package.Id.FamilyName, f => f.PackageFamilyName, (u, f) => u);

            if (failedMandatoryUpdates.Any())
            {
                _logger.LogError("Mandatory updates failed to install.");
                // Implement retry logic or notify the user
                NotifyUserOfMandatoryUpdateFailure();
            }
            else
            {
                _logger.LogWarning("Non-mandatory updates failed to install.");
            }
        }

        /// <summary>
        /// Notifies the user that mandatory updates failed to install.
        /// </summary>
        private void NotifyUserOfMandatoryUpdateFailure()
        {
            // Implement notification logic here
            _logger.LogError("Mandatory updates failed to install. Please try again or contact support.");
        }
    }
    
    /// <summary>
    /// Logger interface for logging messages.
    /// </summary>
    public interface ILogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogError(Exception exception, string message);
    }
    
    public class LoggerService : ILogger
    {
        public void LogError(Exception exception, string message)
        {
            SimpleLogger.LogError(exception, message);
        }

        public void LogError(string message)
        {
        }

        public void LogInfo(string message)
        {
            SimpleLogger.LogInfo(message);
        }

        public void LogWarning(string message)
        {
        }
    }