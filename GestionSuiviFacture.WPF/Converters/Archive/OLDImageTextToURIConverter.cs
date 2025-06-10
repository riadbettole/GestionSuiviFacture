//using System.Globalization;
//using System.Windows.Data;
//using System.Windows.Media.Imaging;

//namespace GestionSuiviFacture.WPF.Converters.Archive
//{
//    class ImageTextToURIConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value is not string imagePath || string.IsNullOrWhiteSpace(imagePath))
//                return Binding.DoNothing;

//            Uri imageUri;
//            try
//            {
//                imageUri = new Uri(imagePath, UriKind.RelativeOrAbsolute);
//            }
//            catch (UriFormatException)
//            {
//                return Binding.DoNothing; // Or handle invalid URI cases
//            }

//            BitmapImage bitmapImage = new BitmapImage();
//            bitmapImage.BeginInit();
//            bitmapImage.UriSource = imageUri;
//            bitmapImage.EndInit();

//            return bitmapImage;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
