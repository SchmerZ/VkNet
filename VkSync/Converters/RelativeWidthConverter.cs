using System;
using System.Windows.Data;

namespace VkSync.Converters
{
    public class RelativeWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var actualWidth = (double) values[0];
            var value = (double)values[1];
            var maxValue = (double)values[2];

            var complete = value / maxValue;

            return (actualWidth * complete);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}