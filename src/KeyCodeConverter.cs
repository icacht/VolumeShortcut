using System;
using System.Windows.Input;
using System.Windows.Data;

namespace VolumeShortcut
{
    [ValueConversion(typeof(int), typeof(string))]
    public class KeyCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return KeyInterop.KeyFromVirtualKey((int)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(Enum.TryParse<Key>((string)value, out Key keyCode))
            {
                return KeyInterop.VirtualKeyFromKey(keyCode);
            }
            return null;
        }
    }
}