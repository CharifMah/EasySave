using Models.Backup;
using Models.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace EasySaveGUI.ViewModels
{
    /// <summary>
    /// Classe JobViewModel
    /// </summary>
    public class JobViewModel : BaseViewModel
    {
        #region Attribute
        private CJob _SelectedJob;
        private CJobManager _jobManager;
        #endregion

        #region Property
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
        /// <summary>
        /// Job selectionnée par l'utilisateur
        /// </summary>
        public CJob SelectedJob
        {
            get { return _SelectedJob; }
            set
            {
                _SelectedJob = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Job charger du job manager
        /// </summary>
        public ObservableCollection<CJob> Jobs
        {
            get
            {
                return _jobManager.Jobs;
            }
            set
            {
                _jobManager.Jobs = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Les job en cours execution du job manager
        /// </summary>
        public ObservableCollection<CJob> JobsRunning { get => _jobManager.JobsRunning; set { _jobManager.JobsRunning = value; NotifyPropertyChanged(); } }
        #endregion

        #region CTOR

        /// <summary>
        /// Constructeur de JobViewModel initialise le JobManager
        /// </summary>
        public JobViewModel()
        {
            string lPath;
            string lFolderPath = CSettings.Instance.JobConfigFolderPath;
            if (!string.IsNullOrEmpty(lFolderPath))
                lPath = Path.Combine(lFolderPath, "JobManager.json");
            else
                lPath = CSettings.Instance.JobDefaultConfigPath;

            _jobManager = CSettings.Instance.LoadJobsFile(lPath);
        }

        #endregion

        /// <summary>
        /// Lance l'exécution des jobs sélectionnés
        /// </summary>
        /// <param name="pJobs">Liste des jobs à lancer</param>
        /// <returns> Liste mise à jour des jobs avec leur état après exécution </returns>
        public async Task RunJobs(List<CJob> pJobs)
        {
            await _jobManager.RunJobs(pJobs);
            NotifyPropertyChanged("Jobs");
            NotifyPropertyChanged("JobsRunning");
        }

        /// <summary>
        /// Crée un nouveau job de sauvegarde
        /// </summary>
        /// <param name="lJob">Job à créer</param>
        /// <returns>Succès de la création</returns>
        public bool CreateBackupJob(CJob lJob)
        {
            bool lResult = _jobManager.CreateBackupJob(lJob);
            NotifyPropertyChanged("Jobs");
            NotifyPropertyChanged("JobsRunning");
            return lResult;
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
            NotifyPropertyChanged("SelectedJob");
            NotifyPropertyChanged("Jobs");

        }

        /// <summary>
        /// Charge la liste des jobs depuis un fichier
        /// </summary>
        /// <param name="IsDefaultFile"> Indique si le fichier par défaut doit être chargé </param>
        /// <param name="pPath"> Chemin du fichier à charger, vide pour le fichier par défaut </param>
        public void LoadJobs(bool IsDefaultFile = true, string pPath = null)
        {
            if (IsDefaultFile)
                _jobManager = CSettings.Instance.LoadJobsFile();
            else
                _jobManager = CSettings.Instance.LoadJobsFile(pPath);

            NotifyPropertyChanged("Jobs");
        }
    }
}
