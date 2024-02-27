using Models.Backup;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasySaveGUI.Converters
{
    public class ETypeBackupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ETypeBackup type)
            {
                switch (type)
                {
                    case ETypeBackup.COMPLET:
                        return ETypeBackup.COMPLET.ToString();
                    case ETypeBackup.DIFFERENTIEL:
                        return ETypeBackup.DIFFERENTIEL.ToString();
                }
            }

            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return (ETypeBackup)Enum.Parse(typeof(ETypeBackup), text);
            }

            if (value is ComboBoxItem Item)
            {
                return (ETypeBackup)Enum.Parse(typeof(ETypeBackup), Item.Content.ToString());
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
