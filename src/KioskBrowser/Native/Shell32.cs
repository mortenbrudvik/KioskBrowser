using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KioskBrowser.Native;

public static class Shell32
{
    [DllImport("Shell32.dll", SetLastError = true)]
    public static extern int SHGetPropertyStoreForWindow(IntPtr hwnd, ref Guid iid, out IPropertyStore propertyStore);

    [ComImport, Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyStore
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCount([Out] out uint cProps);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAt([In] uint iProp, out PropertyKey pkey);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetValue([In] ref PropertyKey key, out PropVariant pv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetValue([In] ref PropertyKey key, [In] ref PropVariant pv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Commit();
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct PropertyKey(Guid guid, uint id)
    {
        public Guid fmtid = guid;
        public uint pid = id;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PropVariant
    {
        [FieldOffset(0)]
        public ushort vt;
        [FieldOffset(8)]
        public IntPtr pszVal;

        public VarEnum VariantType => (VarEnum)vt;
    }
    
    public static PropertyKey PKEY_AppUserModel_ID = new PropertyKey
    {
        fmtid = new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"),
        pid = 5
    };
    
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
