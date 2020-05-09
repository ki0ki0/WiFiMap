using System.IO;
using System.Windows.Media.Imaging;

namespace WiFiMapCore.Model
{
    public static class ImageCoder
    {
        public static byte[] ImageToByte(BitmapSource bitmap)
        {
            var encoder = new PngBitmapEncoder();
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using var stream = new MemoryStream();
            encoder.Save(stream);
            return stream.ToArray();
        }

        public static BitmapSource ByteToImage(byte[] base64)
        {
            var stream = new MemoryStream(base64);
            return BitmapFrame.Create(stream);
        }
    }
}