using Models.Settings;
using Ressources;

namespace EasySaveGUI.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Attributes
        private string _CurrentLayout;
        private FormatLogViewModel _logViewModel;
        private BusinessSoftwareViewModel _businessSoftwareViewModel;
        private FileExtensionViewModel _fileExtensionViewModel;
        #endregion

        #region Property
        /// <summary>
        /// Les parameter global pour le binding
        /// </summary>
        public CSettings Settings
        {
            get
            {
                return CSettings.Instance;
            }
        }
        /// <summary>
        /// Layout actuel
        /// </summary>
        public string CurrentLayout
        {
            get
            {
                return _CurrentLayout;
            }
            set
            {
                _CurrentLayout = value;
                CSettings.Instance.Theme.CurrentLayout = _CurrentLayout;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Format de log view model
        /// </summary>
        public FormatLogViewModel LogVm { get => _logViewModel; set => _logViewModel = value; }
        #endregion

        public BusinessSoftwareViewModel BusinessSoftwareVm { get => _businessSoftwareViewModel; set => _businessSoftwareViewModel = value; }

        public FileExtensionViewModel FileExtensionVm { get => _fileExtensionViewModel; set => _fileExtensionViewModel = value; }

        #region CTOR
        /// <summary>
        /// Constructeur du settings view model
        /// </summary>
        public SettingsViewModel()
        {
            _logViewModel = new FormatLogViewModel();
            _businessSoftwareViewModel = new BusinessSoftwareViewModel();
            _fileExtensionViewModel = new FileExtensionViewModel();

            string lLayout = CSettings.Instance.Theme.CurrentLayout;

            if (!string.IsNullOrEmpty(lLayout))
                _CurrentLayout = lLayout;
            else
                ResetCurrentLayout();
        }
        #endregion

        public void ResetCurrentLayout()
        {
            _CurrentLayout = Strings.DefaultLayout;
            NotifyPropertyChanged("CurrentLayout");
        }
    }
}
