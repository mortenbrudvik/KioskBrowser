using System.Diagnostics;
using System.Windows;

namespace KioskBrowser;

public static class AppRestartHelper
{
    public static void RestartApplication()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "kioskbrowser.exe", // Use your execution alias here
                Arguments = "-t",
                UseShellExecute = true,
            };

            Process.Start(startInfo);
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            // Handle exception (e.g., log the error)
            MessageBox.Show($"Failed to restart the application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}