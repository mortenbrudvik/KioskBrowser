using System.Drawing;
using System.IO;

namespace KioskBrowser.Native;

public class FileInformation(Shell32.SHFILEINFO fileInfo, string filePath)
{
    private Bitmap? _iconBitmap;
    private static readonly HashSet<string> ExecutableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", // Executable program
        ".bat", // Batch file
        ".cmd", // Command script
        ".msi", // Windows installer package
        ".scr"  // Windows screen saver
    };

    public string DisplayName { get; private set; } = fileInfo.szDisplayName;
    public string TypeName { get; private set; } = fileInfo.szTypeName;

    public bool IsExecutable { get; private set; } = IsDirectlyExecutable(filePath);

    public Bitmap? Icon
    {
        get
        {
            if (_iconBitmap == null && fileInfo.hIcon != IntPtr.Zero)
                _iconBitmap = GetIconBitmap(fileInfo.hIcon);

            return _iconBitmap;
        }
    }

    private static Bitmap? GetIconBitmap(IntPtr hIcon)
    {
        if (hIcon == IntPtr.Zero)
            return null;

        using var icon = System.Drawing.Icon.FromHandle(hIcon);

        var bmp = icon.ToBitmap();
        return new Bitmap(bmp);
    }

    private static bool IsDirectlyExecutable(string filePath)
    {
        var extension = Path.GetExtension(filePath);

        return ExecutableExtensions.Contains(extension);
    }

    public void Dispose() => _iconBitmap?.Dispose();
}