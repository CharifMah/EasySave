using Models.Settings;
using Models;
using System.Collections.ObjectModel;

namespace EasySaveGUI.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private FormatLogViewModel _logViewModel;
        private BusinessSoftwareViewModel _businessSoftwareViewModel;
        private FileExtensionViewModel _fileExtensionViewModel;

        public CSettings Settings
        {
            get
            {
                return CSettings.Instance;
            }
        }

        public string CurrentLayout
        {
            get
            {
                return CSettings.Instance.Theme.CurrentLayout;
            }
            set
            {
                CSettings.Instance.Theme.CurrentLayout = value;
                NotifyPropertyChanged();
            }
        }

        public FormatLogViewModel LogVm { get => _logViewModel; set => _logViewModel = value; }

        public BusinessSoftwareViewModel BusinessSoftwareVm { get => _businessSoftwareViewModel; set => _businessSoftwareViewModel = value; }

        public FileExtensionViewModel FileExtensionVm { get => _fileExtensionViewModel; set => _fileExtensionViewModel = value; }

        public SettingsViewModel()
        {
            _logViewModel = new FormatLogViewModel();
            _businessSoftwareViewModel = new BusinessSoftwareViewModel();
            _fileExtensionViewModel = new FileExtensionViewModel();
        }
    }
}
