using System;
using Windows.UI.Xaml.Data;

namespace ModernCode.Uwp.Converter
{
    public class IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
