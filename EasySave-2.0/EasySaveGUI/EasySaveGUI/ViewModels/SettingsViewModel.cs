using Models;
using System.Collections.ObjectModel;

namespace EasySaveGUI.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private FormatLogViewModel _logViewModel;
        private BusinessSoftwareViewModel _businessSoftwareViewModel;

        public ObservableCollection<string> EncryptionFileExtensions => new ObservableCollection<string>(CSettings.Instance._EncryptionExtensions);

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

        public BusinessSoftwareViewModel BusinessSoftwareVm
        {
            get => _businessSoftwareViewModel;
            set
            {
                if (_businessSoftwareViewModel != value)
                {
                    _businessSoftwareViewModel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public SettingsViewModel()
        {
            _logViewModel = new FormatLogViewModel();
            _businessSoftwareViewModel = new BusinessSoftwareViewModel();
        }
    }
}
