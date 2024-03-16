using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KioskBrowser;

public static class ImageUtils
{
    public static async Task<BitmapImage> DownloadFaviconAsync(string faviconUrl)
    {
        using var client = new HttpClient();
        var bytes = await client.GetByteArrayAsync(faviconUrl);

        var image = new BitmapImage();
        using (var stream = new MemoryStream(bytes))
        {
            stream.Position = 0;
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = null;
            image.StreamSource = stream;
            image.EndInit();
        }
        image.Freeze();

        return image;
    }
}