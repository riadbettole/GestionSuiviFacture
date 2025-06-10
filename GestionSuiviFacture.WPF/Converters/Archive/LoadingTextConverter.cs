//using System;
//using System.Globalization;
//using System.Windows.Data;

//namespace GestionSuiviFacture.WPF.Converters.Archive
//{
//    public class LoadingTextConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            return value is bool isLoading && isLoading ? "Traitement..." : "Traiter";
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}