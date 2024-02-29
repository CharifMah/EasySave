using Models.Settings;
using Stockage.Load;
using System.IO;

namespace EasySaveGUI.ViewModels
{
    /// <summary>
    /// Modèle de vue principal regroupant les différents modèles de vue
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region Attributes
        private LangueViewModel _LangueVm;
        private FormatLogViewModel _FormatLogVm;
        private JobViewModel _JobVm;
        private PopupViewModel _PopupVm;
        private SettingsViewModel _SettingsVm;
        private LayoutViewModel _LayoutVm;
        private BusinessSoftwareViewModel _BusinessSoftwareVm;
        private FileExtensionViewModel _FileExtensionVm;
        #endregion

        #region Property
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
        public JobViewModel JobVm { get => _JobVm; set { _JobVm = value; NotifyPropertyChanged(); } }
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

        #endregion

        #region CTOR
        /// <summary>
        /// Le constructeur MainViewModel initialise les modèles de vue et charge les paramètres de l'utilisateur
        /// </summary>
        public MainViewModel()
        {
            string lPath;
            string lFolderPath = CSettings.Instance.JobConfigFolderPath;
            CSettings.Instance.LoadSettings();

            _LangueVm = new LangueViewModel();
            _FormatLogVm = new FormatLogViewModel();

            if (!string.IsNullOrEmpty(lFolderPath))
                lPath = Path.Combine(lFolderPath, "JobManager.json");
            else
                lPath = CSettings.Instance.JobDefaultConfigPath;

            _JobVm = LoadJobsFile(lPath);

            _PopupVm = new PopupViewModel();
            _SettingsVm = new SettingsViewModel();
            _LayoutVm = new LayoutViewModel();
            _BusinessSoftwareVm = new BusinessSoftwareViewModel();
            _FileExtensionVm = new FileExtensionViewModel();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Charge la liste des jobs depuis un fichier
        /// </summary>
        /// <param name="pPath"> Chemin du fichier de configuration. Null pour le fichier par défaut. </param>
        /// <returns> Instance du gestionnaire de jobs chargé </returns>
        public JobViewModel LoadJobsFile(string pPath = null)
        {
            // cm - Si le path est null on init le path par default
            if (string.IsNullOrEmpty(pPath))
                pPath = CSettings.Instance.JobDefaultConfigPath;

            ICharge lChargerCollection = new ChargerCollection(null);

            // cm - Charge le JobViewModel
            JobViewModel lJobViewModel = lChargerCollection.Charger<JobViewModel>(pPath, true);
            // cm - Si aucun fichier n'a été charger on crée un nouveau JobViewModel
            if (lJobViewModel == null)
            {
                lJobViewModel = new JobViewModel();
            }
            else
            {
                CSettings.Instance.JobConfigFolderPath = new FileInfo(pPath).DirectoryName;
            }

            CSettings.Instance.SaveSettings();

            return lJobViewModel;
        }

        /// <summary>
        /// Charge la liste des jobs depuis un fichier
        /// </summary>
        /// <param name="IsDefaultFile"> Indique si le fichier par défaut doit être chargé </param>
        /// <param name="pPath"> Chemin du fichier à charger, vide pour le fichier par défaut </param>
        public void LoadJobs(bool IsDefaultFile = true, string? pPath = null)
        {
            if (IsDefaultFile)
                _JobVm = LoadJobsFile();
            else
                _JobVm = LoadJobsFile(pPath);

            NotifyPropertyChanged("JobVm");
        }

        #endregion
    }
}
