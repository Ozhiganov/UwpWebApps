using System;
using Windows.UI.Xaml.Data;


namespace UwpWebApps.Infrastructure.Converters
{
    public class CaseConverter : IValueConverter
    {
        public CharacterCasing Case { get; set; }


        public CaseConverter()
        {
            Case = CharacterCasing.Upper;
        }


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            if (str != null)
            {
                switch (Case)
                {
                    case CharacterCasing.Lower:
                        return str.ToLower();
                    case CharacterCasing.Normal:
                        return str;
                    case CharacterCasing.Upper:
                        return str.ToUpper();
                    default:
                        return str;
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }


        public enum CharacterCasing
        {
            Normal,
            Lower,
            Upper
        }
    }
}
