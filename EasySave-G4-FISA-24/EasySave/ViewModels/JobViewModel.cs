using Gtk;
using Models.Backup;


namespace EasySave.ViewModels
{
    /// <summary>
    /// Classe JobViewModel
    /// </summary>
    public class JobViewModel : BaseViewModel
    {
        #region Attribute

        private CJobManager _jobManager;
        /// <summary>
        /// JobManager
        /// </summary>
        public CJobManager JobManager { get => _jobManager; set => _jobManager = value; }

        #endregion

        #region CTOR
        /// <summary>
        /// Initialise le JobManager
        /// </summary>
        public JobViewModel()
        {
            _jobManager = CJobManager.LoadJobs();
        }

        #endregion

        /// <summary>
        /// Lance les jobs selectionnée
        /// </summary>
        /// <param name="pRange">L'interval de séléction</param>
        /// <returns>List de Job</returns>
        public List<CJob> RunJobs(List<CJob> pJobs)
        {
            return _jobManager.RunJobs(pJobs);
        }

        /// <summary>
        /// Crée un job
        /// </summary>
        /// <param name="pName">Nom du job</param>
        /// <param name="pSourceDir">chemin source</param>
        /// <param name="pTargetDir">chemin cible</param>
        /// <param name="pType">Type de backup</param>
        /// <returns>Vrai si le job a été crée</returns>
        public bool CreateBackupJob(string pName, string pSourceDir, string pTargetDir, ETypeBackup pType)
        {
            return _jobManager.CreateBackupJob(pName, pSourceDir, pTargetDir, pType);
        }

        #region Serialization
        public void SaveJobs()
        {
            _jobManager.SaveJobs();
        }

        public void LoadJobs(bool IsDefaultFile = true, string pPath = "")
        {
            if (IsDefaultFile)
                _jobManager = CJobManager.LoadJobs();
            else
                _jobManager = CJobManager.LoadJobs(pPath);
        }
        #endregion
    }
}
