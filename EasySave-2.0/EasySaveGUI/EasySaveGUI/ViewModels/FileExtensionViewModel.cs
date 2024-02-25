using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasySaveGUI.ViewModels
{
    public class FileExtensionViewModel : BaseViewModel
    {
        public ObservableCollection<string> EncryptionExtensions { get; private set; }

        public FileExtensionViewModel()
        {
            EncryptionExtensions = new ObservableCollection<string>(CSettings.Instance.EncryptionExtensions);
        }

        public void AddEncryptionExtension(string extension)
        {
            if (!string.IsNullOrWhiteSpace(extension) && !EncryptionExtensions.Contains(extension))
            {
                EncryptionExtensions.Add(extension);
                CSettings.Instance.EncryptionExtensions.Add(extension);
                CSettings.Instance.SaveSettings();
                NotifyPropertyChanged(nameof(EncryptionExtensions));
            }
        }

        public void RemoveEncryptionExtensions(IEnumerable<string> extensions)
        {
            List<string> extensionsList = extensions.ToList();
            foreach (string extension in extensionsList)
            {
                if (EncryptionExtensions.Contains(extension))
                {
                    EncryptionExtensions.Remove(extension);
                    CSettings.Instance.EncryptionExtensions.Remove(extension);
                }
            }
            CSettings.Instance.SaveSettings();
            NotifyPropertyChanged(nameof(EncryptionExtensions));
        }


    }
}
