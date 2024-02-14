using Models.Backup;
using Stockage.Load;
using Stockage.Save;
using System.Runtime.Serialization;

namespace Models
{
    /// <summary>
    /// Classe des settings de l'application permettant le chargement et la sauvegarde des parametres de l'utilisateur
    /// </summary>
    [DataContract]
    public class Settings
    {
        #region Attributes

        private ICharge _loadSettings;
        private ISauve _saveSettings;

        private string _JobDefaultConfigPath;
        [DataMember]
        private string _JobConfigFolderPath;
        [DataMember]
        private CLangue _Langue;

        #endregion

        /// <summary>
        /// Langue préférer de l'utilisateur
        /// </summary>
        public CLangue Langue { get => _Langue; set => _Langue = value; }

        /// <summary>
        /// Emplacement du répertoire dans lequel le fichier de configuration du travail est stocké
        /// </summary>
        public string JobConfigFolderPath
        {
            get
            {
                return _JobConfigFolderPath;
            }
            set => _JobConfigFolderPath = value;
        }
        /// <summary>
        /// Emplacement par défaut du répertoire dans lequel le fichier de configuration du travail est stocké
        /// </summary>
        public string JobDefaultConfigPath { get => _JobDefaultConfigPath; set => _JobDefaultConfigPath = value; }


        #region CTOR
        private static Settings? _Instance;
        public static Settings Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Settings();
                return _Instance;
            }
        }
        /// <summary>
        /// Constructeur Settings initialise le path par default de la configuration des jobs
        /// </summary>
        private Settings()
        {
            _JobDefaultConfigPath = Path.Combine(Environment.CurrentDirectory, "Jobs", "JobManager.json");
        }
        ~Settings()
        {
            SaveSettings();
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
            Settings lInstance = _loadSettings.Charger<Settings>(Path.Combine("Settings"));
            if (lInstance != null)
            {
                _Instance = lInstance;
            }
            if (_Instance != null)
            {
                if (_Instance.Langue == null)
                    _Instance.Langue = new CLangue();
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
                SaveSettings();
            }
            return lJobManager;
        }
        #endregion
    }
}
