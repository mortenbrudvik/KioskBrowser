using System.IO;

namespace KioskBrowser;

public static class SimpleLogger
{
    private static readonly object LockObj = new();
    private static readonly string InstanceId = Guid.NewGuid().ToString();

    static SimpleLogger()
    {
        try
        {
            // Ensure the directory exists
            Directory.CreateDirectory(LogDirectoryPath);
            
            CleanupOldLogs();
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to create log directory: " + e.Message);
        }
    }
    
    public static string LogDirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Swift KioskBrowser");

    public static void LogError(Exception ex, string additionalInfo = "")
    {
        var logMessage = $"{DateTime.Now} | Instance: {InstanceId} | ERROR | {additionalInfo}\n{ex.Message}\n{ex.StackTrace}\n";
        WriteLog(logMessage);
    }
    
    public static void LogInfo(string message)
    {
        var logMessage = $"{DateTime.Now} | Instance: {InstanceId} | INFO | {message}\n";
        WriteLog(logMessage);
    }

    private static void WriteLog(string message)
    {
        lock (LockObj)
        {
            try
            {
                File.AppendAllText(GetDailyLogFilePath(), message);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine("Logging failed: " + ioEx.Message);
            }
        }
    }
    
    
    private static string GetDailyLogFilePath()
    {
        var fileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";
        return Path.Combine(LogDirectoryPath, fileName);
    }

    private static void CleanupOldLogs()
    {
        try
        {
            var logFiles = Directory.GetFiles(LogDirectoryPath, "log_*.txt");

            // Delete files older than 7 days
            foreach (var file in logFiles)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime < DateTime.Now.AddDays(-7))
                {
                    fileInfo.Delete();
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Cleanup of old logs failed: " + ex.Message);
        }
    }
}
