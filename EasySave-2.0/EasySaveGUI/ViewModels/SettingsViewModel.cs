using Models;

namespace ViewModels
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

        public FormatLogViewModel LogVm { get => _logViewModel; set => _logViewModel = value; }

        public SettingsViewModel()
        {
            _logViewModel = new FormatLogViewModel();
        }
    }
}
