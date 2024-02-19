using LogsModels;
using Stockage.Logs;
using Stockage.Save;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
namespace Models.Backup
{
    /// <summary>
    /// Gestionnaire de jobs
    /// </summary>
    [DataContract]
    public class CJobManager : BaseModel
    {
        #region Attribute
        [DataMember]
        private readonly int _MaxJobs;
        [DataMember]
        private ObservableCollection<CJob> _Jobs;
        [DataMember]
        private string _Name;

        private ObservableCollection<CJob> _jobsRunning;

        private ISauve _SauveCollection;

        #endregion

        #region Property
        /// <summary>
        /// Liste des jobs gérés
        /// </summary>
        public ObservableCollection<CJob> Jobs { get => _Jobs; set { _Jobs = value; } }

        /// <summary>
        /// Nom du gestionnaire
        /// </summary>
        public string Name { get => _Name; set => _Name = value; }

        /// <summary>
        /// Interface de sauvegarde des données
        /// </summary>
        public ISauve SauveCollection { get => _SauveCollection; set => _SauveCollection = value; }

        public ObservableCollection<CJob> JobsRunning { get => _jobsRunning; set { _jobsRunning = value; } }


        #endregion

        #region CTOR
        /// <summary>
        /// Contructeur de CJobManager initialise le chemin de sauvegarde
        /// </summary>
        public CJobManager()
        {
            _Name = "JobManager";
            _Jobs = new ObservableCollection<CJob>();
            _jobsRunning = new ObservableCollection<CJob>();
            _MaxJobs = 5;
            SauveJobsAsync _SauveJobs = new SauveJobsAsync();
            if (String.IsNullOrEmpty(CSettings.Instance.JobConfigFolderPath))
            {
                _SauveCollection = new SauveCollection(new FileInfo(CSettings.Instance.JobDefaultConfigPath).DirectoryName);
            }
            else
            {
                _SauveCollection = new SauveCollection(CSettings.Instance.JobConfigFolderPath);
            }
        }
        #endregion

        #region Methods


        /// <summary>
        /// Lance l'exécution de la liste de jobs passée en paramètre
        /// </summary>
        /// <param name="pJobs">Liste des jobs à exécuter</param>
        /// <returns>
        /// La liste des jobs, mise à jour avec leur état après exécution
        /// </returns>
        public async Task RunJobs(List<CJob> pJobs)
        {
            try
            {
                uint lIndex = 0;
                // cm - Lance les jobs
                foreach (CJob lJob in pJobs)
                {
                    SauveJobsAsync _SauveJobs = new SauveJobsAsync("", CSettings.Instance.FormatLog.SelectedFormatLog.Value);
                    lJob.SauveJobs = _SauveJobs;
                    lJob.SauveJobs.LogState.Name = lIndex + ' ' + _SauveJobs.LogState.Name;
                    lJob.SauveJobs.LogState.TotalTransferedFile = 0;
                    lJob.SauveJobs.LogState.BytesCopied = 0;
                    string[] lFiles = Directory.GetFiles(lJob.SourceDirectory, "*", SearchOption.AllDirectories);
                    lJob.SauveJobs.LogState.TotalSize = lFiles.Sum(file => new FileInfo(file).Length);
                    lJob.SauveJobs.LogState.EligibleFileCount = lFiles.Length;

                    _jobsRunning.Add(lJob);
                    await lJob.Run(_jobsRunning.Select(lJob => lJob.SauveJobs.LogState).ToList());
                    lJob.SauveJobs.LogState.Date = DateTime.Now;
                    lIndex++;
                }

            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }
        }

        /// <summary>
        /// Crée un nouveau job de sauvegarde
        /// </summary>
        /// <param name="lJob">Objet représentant le job de sauvegarde à créer</param>
        /// <returns>True si le job a été créé avec succès, false sinon</returns>
        /// <remarks> Created by Mehmeti Faik on 06/02/2024 Updated validation logic to handle null parameters</remarks>
        public bool CreateBackupJob(CJob lJob)
        {
            bool lResult = true;
            // cm - Verifies que on n'a pas atteint la maximum de job
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
        /// <returns>true si réussi</returns>
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
        /// Sauvegarde le JobManager
        /// </summary>
        public void SaveJobs()
        {
            _SauveCollection.Sauver(this, _Name);
        }

        #endregion
    }
}
