using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace EasySaveGUI.ValidationRules
{
    public class FolderPathValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool lResult = false;

            lResult = Directory.Exists(value.ToString());

            return new ValidationResult(lResult, "Dossier inexistant");
        }
    }
}
