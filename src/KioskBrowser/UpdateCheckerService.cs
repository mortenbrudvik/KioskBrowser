using System.Reflection;
using Windows.ApplicationModel;

namespace KioskBrowser;

    /// <summary>
    /// Service that checks for app updates and notifies when an update has been installed.
    /// </summary>
    public class UpdateCheckerService
    {
        private readonly ILogger _logger;
        private CancellationTokenSource _cts;
        private Task _updateCheckTask;

        /// <summary>
        /// Occurs when the application has been updated in the background.
        /// </summary>
        public event EventHandler AppUpdated;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCheckerService"/> class.
        /// </summary>
        /// <param name="logger">An instance of a logger for logging messages.</param>
        public UpdateCheckerService(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Starts periodic checks for app updates.
        /// </summary>
        /// <param name="checkIntervalInSeconds"></param>
        public void Start(uint checkIntervalInSeconds)
        {
            if (_updateCheckTask != null)
            {
                throw new InvalidOperationException("Update checking is already started.");
            }

            _cts = new CancellationTokenSource();
            _updateCheckTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(System.TimeSpan.FromSeconds(checkIntervalInSeconds), _cts.Token);

                        _logger.LogInfo("Checking for app updates...");
                        if (!IsAppUpdated()) continue;
                        
                        _logger.LogInfo("A new version of the app has been installed.");
                        OnAppUpdated();
                        break;
                    }
                    catch (TaskCanceledException)
                    {
                        // Task was canceled, exit gracefully
                        break;
                    }
                    catch (Exception ex)
                    {
                        // ignored
#if !DEBUG
                            _logger.LogError(ex, "Error while checking for app updates.");
#endif
                    }
                }
            }, _cts.Token);
        }

        /// <summary>
        /// Stops periodic checks for app updates.
        /// </summary>
        public void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
                _updateCheckTask = null;
            }
        }

        /// <summary>
        /// Checks if the application has been updated in the background.
        /// </summary>
        /// <returns>True if the app has been updated; otherwise, false.</returns>
        public bool IsAppUpdated()
        {
            var runningVersion = GetRunningAppVersion();
            _logger.LogInfo($"Running version: {runningVersion}");
            var installedVersion = GetInstalledPackageVersion();

            _logger.LogInfo($"Running version: {runningVersion}, Installed version: {installedVersion}");

            return installedVersion > runningVersion;
        }

        /// <summary>
        /// Gets the version of the running application.
        /// </summary>
        /// <returns>The version of the running application.</returns>
        private Version GetRunningAppVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Gets the version of the installed package.
        /// </summary>
        private Version GetInstalledPackageVersion()
        {
            var packageVersion = Package.Current.Id.Version;
            return new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }

        /// <summary>
        /// Raises the <see cref="AppUpdated"/> event.
        /// </summary>
        protected virtual void OnAppUpdated()
        {
            AppUpdated?.Invoke(this, EventArgs.Empty);
        }
    }