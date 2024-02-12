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

        #endregion

        #region CTOR

        /// <summary>
        /// Contructeur de CJobManager initialise le chemin de sauvegarde
        /// </summary>
        public CJobManager()
        {
            _Name = "JobManager";
            _Jobs = new List<CJob>();

            string lPath = Path.Combine(Environment.CurrentDirectory, "Jobs");

            _SauveCollection = new SauveCollection(lPath);
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
        public bool CreateBackupJob(string pName, string pSourceDir, string pTargetDir, ETypeBackup pType)
        {
            bool lResult = true;

            // cm - Verifie que on n'a pas atteint la maximum de job
            if (_Jobs.Count <= _MaxJobs)
                _Jobs.Add(new CJob(pName, pSourceDir, pTargetDir, pType));
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
        public bool DeleteJobByIndex(int index)
        {
            bool lResult = false;

            if (index >= 0 && index < Jobs.Count)
            {
                _Jobs.RemoveAt(index);
                lResult = true;
            }
            return lResult;
        }
        /// <summary>
        /// Lance les jobs dans un interval d'index
        /// </summary>
        /// <param name="pRange">Tuple d'index</param>
        public List<CJob> RunJobs(List<CJob> pJobs)
        {
            SauveJobs lSauveJobs = new SauveJobs(Path.Combine(Environment.CurrentDirectory, "Jobs"));
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
            ICharge lChargerCollection = new ChargerCollection();

            // cm - Si le path est null on init le path par default
            if (pPath == null)
                pPath = Path.Combine(Environment.CurrentDirectory, "Jobs", "JobManager.json");

            // cm - Charge le job manager
            CJobManager lJobManager = lChargerCollection.Charger<CJobManager>(pPath);

            // cm - Si aucun fichier n'a été charger on crée un nouveau JobManager
            if (lJobManager == null)
                lJobManager = new CJobManager();

            return lJobManager;
        }
        #endregion

        #endregion
    }
}

