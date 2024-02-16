using Material.Icons.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EasySaveGUI.Converters
{
    public class LangueConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dictionary<int, string> dict)
            {
                return dict.Select(kvp => GetItemString(kvp));
            }

            return null;
        }

        private string GetItemString(KeyValuePair<int, string> kvp)
        {
            return $"{kvp.Value} - {GetDrapeau(kvp.Key)}";
        }

        private string GetDrapeau(int key)
        {
            switch (key)
            {
                case 0:
                    return "🇫🇷";
                case 1:
                    return "🇺🇸";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
