using System;
using TAlex.Common.Extensions;
using Windows.UI.Xaml.Data;


namespace UwpWebApps.Infrastructure.Converters
{
    public class ColorToFriendlyTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var str = (string)value;
                return StringExtensions.CamelToRegular(str);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
