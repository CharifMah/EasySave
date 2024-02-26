using Models.Backup;
using Models.Langue;
using Models.Settings.Theme;
using Stockage.Load;
using Stockage.Save;
using System.Runtime.Serialization;

namespace Models.Settings
{
    /// <summary>
    /// Classe des settings de l'application permettant le chargement et la sauvegarde des paramètres de l'utilisateur
    /// </summary>
    [DataContract]
    public class CSettings
    {
        #region Attributes

        private ICharge _loadSettings;
        private ISauve _saveSettings;
        private string _LayoutDefaultFolderPath;
        private string _JobDefaultConfigPath;
        private string _LogDefaultFolderPath;
        [DataMember]
        private CTheme _Theme;
        [DataMember]
        private string _JobConfigFolderPath;
        [DataMember]
        private CLangue _Langue;
        [DataMember]
        private CFormatLog _FormatLog;
        [DataMember]
        private List<string> _BusinessSoftware = new List<string>();
        [DataMember]
        public List<string> _EncryptionExtensions = new List<string>();

        private static CSettings? _Instance;
        #endregion

        #region Property
        /// <summary>
        /// Langue préférer de l'utilisateur
        /// </summary>
        public CLangue Langue { get => _Langue; set => _Langue = value; }

        /// <summary>
        /// Format de logs
        /// </summary>
        public CFormatLog FormatLog { get => _FormatLog; set => _FormatLog = value; }

        /// <summary>
        /// Logiciels métiers
        /// </summary>
        public List<string> BusinessSoftware
        {
            get => _BusinessSoftware;
            set => _BusinessSoftware = value;
        }

        public List<string> EncryptionExtensions
        {
            get => _EncryptionExtensions; 
            set => _EncryptionExtensions = value;
        }

        /// <summary>
        /// Emplacement du répertoire dans lequel le fichier de configuration du travail est stocké
        /// </summary>
        public string JobConfigFolderPath
        {
            get
            {
                return _JobConfigFolderPath;
            }
            set { _JobConfigFolderPath = value; SaveSettings(); }
        }
        /// <summary>
        /// Emplacement par défaut du répertoire dans lequel le fichier de configuration du travail est stocké
        /// </summary>
        public string JobDefaultConfigPath { get => _JobDefaultConfigPath; }
        /// <summary>
        /// Chemin du dossier par default des logs
        /// </summary>
        public string LogDefaultFolderPath { get => _LogDefaultFolderPath; set => _LogDefaultFolderPath = value; }
        /// <summary>
        /// Chemin par defaut du dossier de sauvegarde des layouts
        /// </summary>
        public string LayoutDefaultFolderPath { get => _LayoutDefaultFolderPath; set => _LayoutDefaultFolderPath = value; }
        /// <summary>
        /// Theme de l'application
        /// </summary>
        public CTheme Theme { get => _Theme; set => _Theme = value; }

        public static CSettings Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new CSettings();
                return _Instance;
            }
        }

        #endregion

        #region CTOR

        /// <summary>
        /// Constructeur Settings initialise le path par default de la configuration des jobs
        /// </summary>
        private CSettings()
        {
            _JobDefaultConfigPath = Path.Combine(Environment.CurrentDirectory, "Jobs", "JobManager.json");
            _LogDefaultFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
            _LayoutDefaultFolderPath = Path.Combine(_LogDefaultFolderPath, "Layout");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Enregistrer les paramètres dans un fichier json
        /// </summary>
        public void SaveSettings()
        {
            _saveSettings = new SauveCollection(GetSettingsFolderPath());
            _saveSettings.Sauver(this, "Settings");
        }

        private string GetSettingsFolderPath()
        {
            string lAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string lLogsFolder = Path.Combine(lAppDataFolder, "EasySave");
            return lLogsFolder;
        }

        /// <summary>
        ///  Chargement des paramètres à partir d'un fichier json
        /// </summary>
        public void LoadSettings()
        {
            _loadSettings = new ChargerCollection(GetSettingsFolderPath());
            // cm - Charge les settings
            CSettings lInstance = _loadSettings.Charger<CSettings>(Path.Combine("Settings"));
            if (lInstance != null)
            {
                _Instance = lInstance;
            }
            if (_Instance != null) // cm - Initialisation pour ne pas avoir d'instance null
            {
                if (_Instance.Langue == null)
                    _Instance.Langue = new CLangue();
                if (_Instance.FormatLog == null)
                    _Instance.FormatLog = new CFormatLog();
                if (_Instance.Theme == null)
                {
                    _Instance._Theme = new CTheme();
                }
            }
        }


        /// <summary>
        /// Reset le chemin de sauvegarde du jobmanager
        /// </summary>
        public void ResetJobConfigPath()
        {
            _JobConfigFolderPath = new FileInfo(_JobDefaultConfigPath).DirectoryName;
        }
        /// <summary>
        /// Définit le chemin de sauvegarde du job manager a partir d'un chemin correspondant a un fichier
        /// </summary>
        /// <param name="pFullPath">chemin du fichier complet</param>
        public void SetJobConfigPath(string pFullPath)
        {
            _JobConfigFolderPath = new FileInfo(pFullPath).DirectoryName;
        }
        #endregion
    }
}
