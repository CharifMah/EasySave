using Models.Backup;
using System.Collections.ObjectModel;
using static System.Reflection.Metadata.BlobBuilder;

namespace ViewModels
{
    /// <summary>
    /// Classe JobViewModel
    /// </summary>
    public class JobViewModel : BaseViewModel
    {
        #region Attribute
        private CJob _SelectedJob;
        private CJobManager _jobManager;
        /// <summary>
        /// JobManager
        /// </summary>
        public CJobManager JobManager
        {
            get => _jobManager;
            set
            {
                _jobManager = value;
                NotifyPropertyChanged();
            }
        }

        public CJob SelectedJob
        {
            get { return _SelectedJob; }
            set
            {
                _SelectedJob = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<CJob> JobsRunning { get => _jobsRunning; set { _jobsRunning = value; NotifyPropertyChanged(); } }

        private ObservableCollection<CJob> _jobsRunning;

        #endregion

        #region CTOR

        /// <summary>
        /// Constructeur de JobViewModel initialise le JobManager
        /// </summary>
        public JobViewModel()
        {
            string lPath;
            string lFolderPath = Models.CSettings.Instance.JobConfigFolderPath;
            if (!string.IsNullOrEmpty(lFolderPath))
                lPath = Path.Combine(lFolderPath, "JobManager.json");
            else
                lPath = Models.CSettings.Instance.JobDefaultConfigPath;
            _jobsRunning = new ObservableCollection<CJob>();
            _jobManager = Models.CSettings.Instance.LoadJobsFile(lPath);
        }

        #endregion

        /// <summary>
        /// Lance l'exécution des jobs sélectionnés
        /// </summary>
        /// <param name="pJobs">Liste des jobs à lancer</param>
        /// <returns> Liste mise à jour des jobs avec leur état après exécution </returns>
        public void RunJobs(List<CJob> pJobs)
        {
            _jobsRunning = new ObservableCollection<CJob>(pJobs);
            NotifyPropertyChanged("JobsRunning");
            _jobManager.RunJobs(_jobsRunning);           
        }

        /// <summary>
        /// Crée un nouveau job de sauvegarde
        /// </summary>
        /// <param name="lJob">Job à créer</param>
        /// <returns>Succès de la création</returns>
        public bool CreateBackupJob(CJob lJob)
        {
            return _jobManager.CreateBackupJob(lJob);
        }

        /// <summary>
        /// Supprimer un ou plusieurs jobs
        /// </summary>
        /// <param name="pJobs">List de jobs a delete</param>
        /// <returns>vrai si les jobs on été delete</returns>
        public bool DeleteJobs(List<CJob> pJobs)
        {
            return _jobManager.DeleteJobs(pJobs);
        }

        /// <summary>
        /// Sauvegarde la configuration des jobs
        /// </summary>
        public void SaveJobs()
        {
            _jobManager.SaveJobs();
            NotifyPropertyChanged("JobManager");
        }

        /// <summary>
        /// Charge la liste des jobs depuis un fichier
        /// </summary>
        /// <param name="IsDefaultFile"> Indique si le fichier par défaut doit être chargé </param>
        /// <param name="pPath"> Chemin du fichier à charger, vide pour le fichier par défaut </param>
        public void LoadJobs(bool IsDefaultFile = true, string pPath = null)
        {
            if (IsDefaultFile)
                _jobManager = Models.CSettings.Instance.LoadJobsFile();
            else
                _jobManager = Models.CSettings.Instance.LoadJobsFile(pPath);

            NotifyPropertyChanged("JobManager");
        }
    }
}
