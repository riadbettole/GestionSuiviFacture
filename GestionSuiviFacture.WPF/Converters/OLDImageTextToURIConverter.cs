using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GestionSuiviFacture.WPF.Converters
{
    class ImageTextToURIConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imagePath = value.ToString();
            Uri imageUri = new Uri(imagePath, UriKind.RelativeOrAbsolute);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = imageUri;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
