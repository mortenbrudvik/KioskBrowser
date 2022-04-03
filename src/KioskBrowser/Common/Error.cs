using LanguageExt;

using static LanguageExt.Prelude;

namespace KioskBrowser.Common;

public static class Error
{
    public static Option<T> TryCatch<T>(Func<T> func)
    {
        try
        {
            return func();
        }
        catch (Exception)
        {
            return None;
        }
    }
    public static void TryCatch(Action action)
    {
        try
        {
            action();
        }
        catch (Exception)
        {
            // ignored
        }
    }
}