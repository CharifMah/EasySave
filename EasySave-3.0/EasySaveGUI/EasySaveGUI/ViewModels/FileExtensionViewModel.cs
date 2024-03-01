using Models.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EasySaveGUI.ViewModels
{
    public class FileExtensionViewModel : BaseViewModel
    {
        public ObservableCollection<string> EncryptionExtensions { get; private set; }

        public ObservableCollection<string> PriorityFileExtensions { get; private set; }

        public FileExtensionViewModel()
        {
            EncryptionExtensions = new ObservableCollection<string>(CSettings.Instance.EncryptionExtensions);
            PriorityFileExtensions = new ObservableCollection<string>(CSettings.Instance.PriorityFileExtensions);
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
            foreach (string extension in pExtensions)
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

        public void AddPriorityFileExtension(string pExtension)
        {
            if (!string.IsNullOrWhiteSpace(pExtension) && !PriorityFileExtensions.Contains(pExtension))
            {
                PriorityFileExtensions.Add(pExtension);
                CSettings.Instance.PriorityFileExtensions.Add(pExtension);
                CSettings.Instance.SaveSettings();
                NotifyPropertyChanged(nameof(PriorityFileExtensions));
            }
        }

        public void RemovePriorityFileExtensions(List<string> pExtensions)
        {
            foreach (string extension in pExtensions)
            {
                if (PriorityFileExtensions.Contains(extension))
                {
                    PriorityFileExtensions.Remove(extension);
                    CSettings.Instance.PriorityFileExtensions.Remove(extension);
                }
            }
            CSettings.Instance.SaveSettings();
            NotifyPropertyChanged(nameof(PriorityFileExtensions));
        }
    }
}
