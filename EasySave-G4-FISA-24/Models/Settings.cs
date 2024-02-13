using Stockage;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class Settings
    {
        #region Attributes
        private ICharge _loadSettings;
        private ISauve _saveSettings;
        private static Settings? _Instance;
        [DataMember]
        private string _JobConfigPath;
        [DataMember]
        private CLangue _Langue;

        #endregion
        public CLangue Langue { get => _Langue; set => _Langue = value; }
        public string JobConfigPath { get => _JobConfigPath; set => _JobConfigPath = value; }
        #region CTOR

        public static Settings Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Settings();
                return _Instance;
            }
        }


        private Settings()
        {

        }
        ~Settings()
        {
            SaveSettings();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Save Settings in a json file
        /// </summary>
        public void SaveSettings()
        {
            _saveSettings = new SauveCollection(Environment.CurrentDirectory);
            _saveSettings.Sauver(this, "Settings");
        }

        /// <summary>
        /// Load Settigns from json file
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
        #endregion
    }
}
