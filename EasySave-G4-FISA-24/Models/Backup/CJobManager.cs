using Stockage;
using System.Runtime.Serialization;
namespace Models.Backup
{
    [DataContract]
    public class CJobManager
    {
        #region Attribute
        [DataMember]
        private readonly int _MaxJobs = 5;
        [DataMember]
        private List<CJob> _Jobs;
        [DataMember]
        private string _Name;
        private ISauve _SauveCollection;

        #endregion

        #region Property
        public List<CJob> Jobs { get => _Jobs; }
        public string Name { get => _Name; set => _Name = value; }
        public ISauve SauveCollection { get => _SauveCollection; set => _SauveCollection = value; }

        #endregion
        #region CTOR
        /// <summary>
        /// Contructeur de CJobManager initialise le chemin de sauvegarde
        /// </summary>
        public CJobManager(string pConfigPath = "")
        {
            _Name = "JobManager";
            _Jobs = new List<CJob>();

            if (pConfigPath == "")
                Settings.Instance.JobConfigPath = Path.Combine(Environment.CurrentDirectory, "Jobs");
            else
                Settings.Instance.JobConfigPath = pConfigPath;

            _SauveCollection = new SauveCollection(Settings.Instance.JobConfigPath);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Crée un job
        /// </summary>
        /// <param name="name">nom du job</param>
        /// <param name="sourceDir">chemin source</param>
        /// <param name="targetDir">chemin cible</param>
        /// <param name="type">type de job</param>
        /// <returns>true si reussi</returns>
        /// <remarks>Mehmeti faik - 06/02/2024 - fixbug</remarks>
        public bool CreateBackupJob(CJob lJob)
        {
            bool lResult = true;
            // cm - Verifie que on n'a pas atteint la maximum de job
            if (_Jobs.Count <= _MaxJobs && !_Jobs.Contains(lJob))
                _Jobs.Add(lJob);
            else
                lResult = false;
            return lResult;
        }
        /// <summary>
        /// Supprimé un job par son index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>true si reussi</returns>
        /// <remarks>Mehmeti faik</remarks>
        public bool DeleteJobs(List<CJob> pJobs)
        {
            foreach (CJob lJob in pJobs)
            {
                _Jobs.Remove(lJob);
            }
            return true;
        }
        /// <summary>
        /// Lance les jobs dans un interval d'index
        /// </summary>
        /// <param name="pRange">Tuple d'index</param>
        public List<CJob> RunJobs(List<CJob> pJobs)
        {
            SauveJobs lSauveJobs = new SauveJobs(Settings.Instance.JobConfigPath);
            // cm - Lance les jobs
            foreach (CJob lJob in pJobs)
            {
                lSauveJobs.TransferedFiles = 0;
                lJob.Run(lSauveJobs);
            }
            return pJobs;
        }
        #region Serialization
        /// <summary>
        /// Sauvegarde le JobManager
        /// </summary>
        public void SaveJobs()
        {
            _SauveCollection.Sauver(this, _Name);
        }

        /// <summary>
        /// Charge les Jobs
        /// </summary>
        /// <param name="pPath">Absolute Path</param>
        /// <returns>CJobManager</returns>
        public static CJobManager LoadJobs(string pPath = null)
        {
            ICharge lChargerCollection = new ChargerCollection("");
            // cm - Si le path est null on init le path par default
            if (String.IsNullOrEmpty(pPath))
                pPath = Path.Combine(Environment.CurrentDirectory, "Jobs", "JobManager.json");
            // cm - Charge le job manager
            CJobManager lJobManager = lChargerCollection.Charger<CJobManager>(pPath);
            // cm - Si aucun fichier n'a été charger on crée un nouveau JobManager
            if (lJobManager == null)
                lJobManager = new CJobManager();
            else
            {
                Models.Settings.Instance.JobConfigPath = pPath;
                Models.Settings.Instance.SaveSettings();
            }
            return lJobManager;
        }
        #endregion
        #endregion
    }
}
