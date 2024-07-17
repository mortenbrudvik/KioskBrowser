using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
using SkiaSharp;
using Svg.Skia;

namespace KioskBrowser.Common;

public static class FaviconIcon
{
    public static async Task<BitmapImage?> DownloadAsync(string url)
    {
        try
        {
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(url);

            return IsSvgImage(bytes) ? RenderSvgToBitmapImage(bytes) : CreateBitmapImageFromBytes(bytes);
        }
        catch (Exception)
        {
            // Ignore
        }

        return null;
    }
    
    private static BitmapImage CreateBitmapImageFromBytes(byte[] bytes)
    {
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
    
    private static bool IsSvgImage(byte[] bytes)
    {
        var header = System.Text.Encoding.UTF8.GetString(bytes, 0, Math.Min(bytes.Length, 100));
        return header.Contains("<svg", StringComparison.OrdinalIgnoreCase);
    }

    private static BitmapImage? RenderSvgToBitmapImage(byte[] svgBytes)
    {
        using var stream = new MemoryStream(svgBytes);
        using var svg = new SKSvg();
        svg.Load(stream);

        var picture = svg.Picture;
        if (picture == null) return null;

        var info = new SKImageInfo((int)picture.CullRect.Width, (int)picture.CullRect.Height);
        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.DrawPicture(picture);
        canvas.Flush();

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var bitmapStream = new MemoryStream();
        data.SaveTo(bitmapStream);
        bitmapStream.Position = 0;

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.StreamSource = bitmapStream;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        bitmap.Freeze();

        return bitmap;
    }
}