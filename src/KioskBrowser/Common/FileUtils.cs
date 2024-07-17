using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using KioskBrowser.Native;
using static KioskBrowser.Native.Shell32;

namespace KioskBrowser.Common;

public static class FileUtils
{
    public static bool IsFilePath(string path)
    {
        // Check for common absolute path patterns
        return Path.IsPathRooted(path) &&
               !path.StartsWith("http:", StringComparison.OrdinalIgnoreCase) &&
               !path.StartsWith("https:", StringComparison.OrdinalIgnoreCase) &&
               !path.StartsWith("ftp:", StringComparison.OrdinalIgnoreCase);
    }
    
    public static BitmapImage? GetFileIcon(string path)
    {
        var info = GetFileInfo(path);
        return info.Icon == null ? null : BitmapToBitmapImage(info.Icon);
    }
    
    private static FileInformation GetFileInfo(string path)
    {
        var info = new SHFILEINFO();
        const uint flags = SHGFI_ICON | SHGFI_DISPLAYNAME | SHGFI_TYPENAME | SHGFI_ATTRIBUTES | SHGFI_ICONLOCATION | SHGFI_EXETYPE;
        SHGetFileInfo(path, 0, ref info, (uint)Marshal.SizeOf(info), flags);

        return new FileInformation(info, path);
    }
    
    private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
    {
        using (var memoryStream = new MemoryStream())
        {
            // Save the bitmap to the memory stream
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Position = 0;

            // Create a BitmapImage from the stream
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze(); // Freeze to make it cross-thread accessible

            return bitmapImage;
        }
    }
}