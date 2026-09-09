using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Selection.Revit.Utils
{
    public static class ImageUtils
    {
        public static ImageSource PngImageSource(string embeddedPath)
        {
            Assembly assembly = typeof(ImageUtils).Assembly;

            Stream stream = assembly.GetManifestResourceStream(embeddedPath);

            if (stream == null)
            {
                throw new FileNotFoundException(
                    $"Embedded resource not found: {embeddedPath}",
                    embeddedPath);
            }

            var decoder = new PngBitmapDecoder(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            return decoder.Frames[0];
        }
    }
}
