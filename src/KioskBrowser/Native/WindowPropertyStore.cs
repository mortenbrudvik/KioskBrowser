using System.Runtime.InteropServices;

namespace KioskBrowser.Native;

public class WindowPropertyStore(IntPtr hwnd)
{
    public void SetAppUserModelId(string appUserModelId)
    {
        var guidPropertyStore = new Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99");
        var result = Shell32.SHGetPropertyStoreForWindow(hwnd, ref guidPropertyStore, out var propertyStore);
        if (result != 0) return;
        
        var propValue = new Shell32.PropVariant
        {
            vt = (ushort)VarEnum.VT_LPWSTR,
            pszVal = Marshal.StringToCoTaskMemUni(appUserModelId)
        };

        propertyStore.SetValue(ref Shell32.PKEY_AppUserModel_ID, propValue);
        propertyStore.Commit();

        Marshal.FreeCoTaskMem(propValue.pszVal);
    }
}