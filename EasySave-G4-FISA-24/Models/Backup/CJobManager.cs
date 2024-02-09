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
        /// Contructeur de CJobManager
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

            if (_Jobs.Count <= _MaxJobs)
                _Jobs.Add(new CJob(pName, pSourceDir, pTargetDir, pType));
            else
                lResult = false;

            return lResult;
        }

        /// <summary>
        /// Lance les jobs dans un interval d'index
        /// </summary>
        /// <param name="pRange">Tuple d'index</param>
        public List<CJob> RunJobs(Tuple<int, int> pRange = null)
        {
            List<CJob> lRunningJobs = new List<CJob>();

            if (pRange == null)
                pRange = new Tuple<int, int>(0, _Jobs.Count - 1);

            if (pRange.Item1 >= 0 && pRange.Item2 <= _Jobs.Count())
                for (int i = pRange.Item1; i <= pRange.Item2; i++)
                {
                    lRunningJobs.Add(_Jobs[i]);
                    _Jobs[i].Run();
                }

            return lRunningJobs;
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

            if (pPath == null)
                pPath = Path.Combine(Environment.CurrentDirectory, "Jobs", "JobManager.json");

            CJobManager lJobManager = lChargerCollection.Charger<CJobManager>(pPath);

            if (lJobManager == null)
                lJobManager = new CJobManager();

            return lJobManager;
        }
        #endregion

        #endregion
    }
}

