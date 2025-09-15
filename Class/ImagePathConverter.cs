using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace beauty_salon.Class
{
        public class ImagePathConverter : IValueConverter
        {
                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        if (value is string path && !string.IsNullOrEmpty(path) && File.Exists(path))
                        {
                                try
                                {
                                        var image = new BitmapImage();
                                        image.BeginInit();
                                        image.CacheOption = BitmapCacheOption.OnLoad;
                                        image.UriSource = new Uri(path);
                                        image.EndInit();
                                        return image;
                                }
                                catch
                                {
                                        return GetDefaultImage();
                                }
                        }
                        return GetDefaultImage();
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        throw new NotImplementedException();
                }

                private BitmapImage GetDefaultImage()
                {
                        return new BitmapImage(new Uri("pack://application:,,,/Resources/default-service.png"));
                }
        }
}