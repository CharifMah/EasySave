namespace EasySaveGUI.ViewModels
{
    /// <summary>
    /// Modèle de vue principal regroupant les différents modèles de vue
    /// </summary>
    public class MainViewModel
    {
        private LangueViewModel _LangueVm;
        private FormatLogViewModel _FormatLogVm;
        private JobViewModel _JobVm;
        private PopupViewModel _PopupVm;
        private SettingsViewModel _SettingsVm;
        private LayoutViewModel _LayoutVm;
        private BusinessSoftwareViewModel _BusinessSoftwareVm;
        private FileExtensionViewModel _FileExtensionVm;
        /// <summary>
        /// View Model de la langue
        /// </summary>
        public LangueViewModel LangueVm { get => _LangueVm; set => _LangueVm = value; }
        /// <summary>
        /// Format Log View Model
        /// </summary>
        public FormatLogViewModel FormatLogVm { get => _FormatLogVm; set => _FormatLogVm = value; }
        /// <summary>
        /// View model des jobs
        /// </summary>
        public JobViewModel JobVm { get => _JobVm; set => _JobVm = value; }
        public PopupViewModel PopupVm { get => _PopupVm; set => _PopupVm = value; }
        public SettingsViewModel SettingsVm { get => _SettingsVm; set => _SettingsVm = value; }
        public LayoutViewModel LayoutVm { get => _LayoutVm; set => _LayoutVm = value; }
        
        /// <summary>
        /// View Model pour la gestion des logiciels métiers
        /// </summary>
        public BusinessSoftwareViewModel BusinessSoftwareVm
        {
            get => _BusinessSoftwareVm;
            set => _BusinessSoftwareVm = value;
        }

        public FileExtensionViewModel FileExtensionVm
        {
            get => _FileExtensionVm;
            set => _FileExtensionVm = value;
        }



        /// <summary>
        /// Le constructeur MainViewModel initialise les modèles de vue et charge les paramètres de l'utilisateur
        /// </summary>
        public MainViewModel()
        {
            Models.CSettings.Instance.LoadSettings();
            _LangueVm = new LangueViewModel();
            _FormatLogVm = new FormatLogViewModel();
            _JobVm = new JobViewModel();
            _PopupVm = new PopupViewModel();
            _SettingsVm = new SettingsViewModel();
            _LayoutVm = new LayoutViewModel();
            _BusinessSoftwareVm = new BusinessSoftwareViewModel();
            _FileExtensionVm = new FileExtensionViewModel();
        }
    }
}
