using Models.Backup;
using Stockage.Load;
using Stockage.Save;
using System.Runtime.Serialization;

namespace Models
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
        public string LogDefaultFolderPath { get => _LogDefaultFolderPath; set => _LogDefaultFolderPath = value; }
        public string LayoutDefaultFolderPath { get => _LayoutDefaultFolderPath; set => _LayoutDefaultFolderPath = value; }
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
            _saveSettings = new SauveCollection(Environment.CurrentDirectory);
            _saveSettings.Sauver(this, "Settings");
        }

        /// <summary>
        ///  Chargement des paramètres à partir d'un fichier json
        /// </summary>
        public void LoadSettings()
        {
            _loadSettings = new ChargerCollection(Environment.CurrentDirectory);
            CSettings lInstance = _loadSettings.Charger<CSettings>(Path.Combine("Settings"));
            if (lInstance != null)
            {
                _Instance = lInstance;
            }
            if (_Instance != null)
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
        /// Charge la liste des jobs depuis un fichier
        /// </summary>
        /// <param name="pPath"> Chemin du fichier de configuration. Null pour le fichier par défaut. </param>
        /// <returns> Instance du gestionnaire de jobs chargé </returns>
        public CJobManager LoadJobsFile(string pPath = null)
        {
            // cm - Si le path est null on init le path par default
            if (String.IsNullOrEmpty(pPath))
                pPath = _JobDefaultConfigPath;

            ICharge lChargerCollection = new ChargerCollection(null);

            // cm - Charge le job manager
            CJobManager lJobManager = lChargerCollection.Charger<CJobManager>(pPath, true);
            // cm - Si aucun fichier n'a été charger on crée un nouveau JobManager
            if (lJobManager == null)
            {
                lJobManager = new CJobManager();
            }
            else
            {
                _JobConfigFolderPath = new FileInfo(pPath).DirectoryName;
            }

            SaveSettings();

            return lJobManager;
        }

        public void ResetJobConfigPath()
        {
            _JobConfigFolderPath = new FileInfo(_JobDefaultConfigPath).DirectoryName;
        }

        public void SetJobConfigPath(string pFullPath)
        {
            _JobConfigFolderPath = new FileInfo(pFullPath).DirectoryName;
        }
        #endregion
    }
}
