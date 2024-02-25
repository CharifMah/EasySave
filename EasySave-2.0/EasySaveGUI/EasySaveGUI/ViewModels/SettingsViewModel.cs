using Models.Settings;

namespace EasySaveGUI.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private FormatLogViewModel _logViewModel;
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

        public SettingsViewModel()
        {
            _logViewModel = new FormatLogViewModel();
        }
    }
}
