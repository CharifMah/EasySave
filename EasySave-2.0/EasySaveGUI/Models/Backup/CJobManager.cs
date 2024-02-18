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
    public class CJobManager
    {
        #region Attribute
        [DataMember]
        private readonly int _MaxJobs;
        [DataMember]
        private ObservableCollection<CJob> _Jobs;
        [DataMember]
        private string _Name;

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

        #endregion

        #region CTOR
        /// <summary>
        /// Contructeur de CJobManager initialise le chemin de sauvegarde
        /// </summary>
        public CJobManager()
        {
            _Name = "JobManager";
            _Jobs = new ObservableCollection<CJob>();
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
        /// Lance l'exécution de la liste de jobs passée en paramètre
        /// </summary>
        /// <param name="pJobs">Liste des jobs à exécuter</param>
        /// <returns>
        /// La liste des jobs, mise à jour avec leur état après exécution
        /// </returns>
        public async Task RunJobs(ObservableCollection<CJob> pJobs)
        {
            try
            {
                // cm - Lance les jobs
                foreach (CJob lJob in pJobs)
                {
                    SauveJobsAsync _SauveJobs = new SauveJobsAsync("", CSettings.Instance.FormatLog.SelectedFormatLog.Value);
                    _SauveJobs.LogState.TotalTransferedFile = 0;
                    _SauveJobs.LogState.BytesCopied = 0;
                    string[] lFiles = Directory.GetFiles(lJob.SourceDirectory, "*", SearchOption.AllDirectories);
                    _SauveJobs.LogState.TotalSize = lFiles.Sum(file => new FileInfo(file).Length);
                    _SauveJobs.LogState.EligibleFileCount = lFiles.Length;
                    await lJob.Run(_SauveJobs);
                    _SauveJobs.LogState.Date = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }

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
