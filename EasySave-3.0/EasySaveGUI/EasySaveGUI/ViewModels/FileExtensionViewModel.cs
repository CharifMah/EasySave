using Models.Settings;
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

        public void AddEncryptionExtension(string pExtension)
        {
            if (!string.IsNullOrWhiteSpace(pExtension) && !EncryptionExtensions.Contains(pExtension))
            {
                EncryptionExtensions.Add(pExtension);
                CSettings.Instance.EncryptionExtensions.Add(pExtension);
                CSettings.Instance.SaveSettings();
                NotifyPropertyChanged(nameof(EncryptionExtensions));
            }
        }

        public void RemoveEncryptionExtensions(List<string> pExtensions)
        {
            List<string> extensionsList = pExtensions.ToList();
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
