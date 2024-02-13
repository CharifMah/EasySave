using Stockage;
using System.Runtime.Serialization;
namespace Models.Backup
{
    /// <summary>
    /// Gestionnaire de jobs
    /// </summary>
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
        /// <summary>
        /// Liste des jobs gérés
        /// </summary>
        public List<CJob> Jobs { get => _Jobs; }

        /// <summary>
        /// Nom du gestionnaire
        /// </summary>
        public string Name { get => _Name; set => _Name = value; }

        /// <summary>
        /// Interface de sauvegarde des données
        /// </summary>
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

            if (!String.IsNullOrEmpty(pConfigPath))
                Settings.Instance.JobConfigPath = pConfigPath;

            _SauveCollection = new SauveCollection(Settings.Instance.JobConfigPath);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Crée un nouveau job de sauvegarde
        /// </summary>
        /// <param name="backupJob">Objet représentant le job de sauvegarde à créer</param>
        /// <returns>True si le job a été créé avec succès, false sinon</returns>
        /// <remarks> Created by Mehmeti Faik on 06/02/2024 Updated validation logic to handle null parameters</remarks>
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
        /// Supprimé un job
        /// </summary>
        /// <param name="pJobs">List de jobs à supprimer</param>
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
        /// Lance l'exécution de la liste de jobs passée en paramètre
        /// </summary>
        /// <param name="pJobs">Liste des jobs à exécuter</param>
        /// <returns>
        /// La liste des jobs, mise à jour avec leur état après exécution
        /// </returns>
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

        /// <summary>
        /// Sauvegarde le JobManager
        /// </summary>
        public void SaveJobs()
        {
            _SauveCollection.Sauver(this, _Name);
        }

        /// <summary>
        /// Charge la liste des jobs depuis un fichier
        /// </summary>
        /// <param name="pPath"> Chemin du fichier de configuration. Null pour le fichier par défaut. </param>
        /// <returns> Instance du gestionnaire de jobs chargé </returns>
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
                lJobManager.SauveCollection = new SauveCollection(Models.Settings.Instance.JobConfigPath);
                Models.Settings.Instance.SaveSettings();
            }
            return lJobManager;
        }
        #endregion
    }
}
