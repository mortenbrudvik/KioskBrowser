using System.Runtime.InteropServices;
using static KioskBrowser.Native.Shell32;

namespace KioskBrowser.Native;

public static class ShellHelper
{
    public static void SetAppUserModelId(IntPtr hwnd, string appUserModelId)
    {
        var guidPropertyStore = new Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99");
        var result = SHGetPropertyStoreForWindow(hwnd, ref guidPropertyStore, out var propertyStore);
        if (result != 0) return;
        
        var propValue = new PropVariant
        {
            vt = (ushort)VarEnum.VT_LPWSTR,
            pszVal = Marshal.StringToCoTaskMemUni(appUserModelId)
        };

        propertyStore.SetValue(ref PKEY_AppUserModel_ID, propValue);
        propertyStore.Commit();

        Marshal.FreeCoTaskMem(propValue.pszVal);
    }
}