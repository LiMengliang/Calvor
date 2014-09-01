using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PhotoGallery.View
{
    public class PathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var uri = (Uri) value;
            if (uri != null)
            {
                var path = new DirectoryInfo(uri.AbsolutePath).Extension;
                if (path.Contains("jpg"))
                {
                    return new BitmapImage(uri);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var image = (BitmapImage)value;
            return image.UriSource;
        }
    }
}
