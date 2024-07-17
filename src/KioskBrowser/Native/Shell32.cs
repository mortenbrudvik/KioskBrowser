using System.Runtime.InteropServices;

namespace KioskBrowser.Native;

public static class Shell32
{
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern bool SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    public const uint SHGFI_TYPENAME = 0x000000400;
    public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
    public const uint SHGFI_ICON = 0x000000100; // Retrieve the file's icon.
    public const uint SHGFI_DISPLAYNAME = 0x000000200; // Retrieve the display name.
    public const uint SHGFI_ATTRIBUTES = 0x000000800; // Retrieve the attributes.
    public const uint SHGFI_ICONLOCATION = 0x000001000; // Retrieve the location of the file's icon.
    public const uint SHGFI_EXETYPE = 0x000002000; // Retrieve the type of the executable.
    public const uint SHGFI_SYSICONINDEX = 0x000004000; // Retrieve the index of the icon in the system image list.
    public const uint SHGFI_LINKOVERLAY = 0x000008000; // Show a link overlay on the icon.
    public const uint SHGFI_SELECTED = 0x000010000; // Show the icon in a selected state (useful for file explorers).
    public const uint SHGFI_ATTR_SPECIFIED = 0x000020000; // Specify that only specified attributes are needed.
}
