namespace ViewModels
{
    /// <summary>
    /// Modèle de vue principal regroupant les différents modèles de vue
    /// </summary>
    public class MainViewModel
    {
        private LangueViewModel _LangueVm;
        private JobViewModel _JobVm;
        private FormatLogViewModel _FormatLogVm;
        /// <summary>
        /// View Model de la langue
        /// </summary>
        public LangueViewModel LangueVm { get => _LangueVm; set => _LangueVm = value; }
        /// <summary>
        /// View model des jobs
        /// </summary>
        public FormatLogViewModel FormatLogVm { get => _FormatLogVm; set => _FormatLogVm = value; }
        /// <summary>
        /// View model des jobs
        /// </summary>
        public JobViewModel JobVm { get => _JobVm; set => _JobVm = value; }
        /// <summary>
        /// Le constructeur MainViewModel initialise les modèles de vue et charge les paramètres de l'utilisateur
        /// </summary>
        public MainViewModel()
        {
            Models.CSettings.Instance.LoadSettings();
            _LangueVm = new LangueViewModel();
            _FormatLogVm = new FormatLogViewModel();
            _JobVm = new JobViewModel();
        }
    }
}
