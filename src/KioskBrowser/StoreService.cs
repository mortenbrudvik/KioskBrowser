using System.Windows;
using System.Windows.Interop;
using Windows.Services.Store;

namespace KioskBrowser;

public class StoreService
{
    private StoreContext? _context;

    private StoreContext? Context
    {
        get
        {
            if (_context != null)
                return _context;

            var context = StoreContext.GetDefault();
            var window = Application.Current.MainWindow;
            if (window is null)
                return null;

            var hwnd = new WindowInteropHelper(window).Handle;
            WinRT.Interop.InitializeWithWindow.Initialize(context, hwnd);

            return _context = context;
        }
    }

    public async Task<bool> IsUpdateAvailableAsync()
    {
        if (Context == null)
            return false;

        try
        {
            var updates = await Context.GetAppAndOptionalStorePackageUpdatesAsync();
            return updates.Any();
        }
        catch (Exception)
        {
            return false;
        }
    }
}
