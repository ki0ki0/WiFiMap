using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WiFiMapCore.ViewModels
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, 
            object parameter, CultureInfo culture)
        {
            var v = (bool) value;
            var black = v ? Colors.DarkRed: Colors.LimeGreen;
            
            var solidColorBrush = new SolidColorBrush(black);
            return solidColorBrush;
        }
 
        public object ConvertBack(object value, Type targetType, 
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}