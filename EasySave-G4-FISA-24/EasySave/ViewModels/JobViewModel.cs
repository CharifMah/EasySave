using Models.Backup;


namespace EasySave.ViewModels
{
    public class JobViewModel : BaseViewModel
    {
        #region Attribute

        private CJobManager _jobManager;
        public CJobManager JobManager { get => _jobManager; set => _jobManager = value; }

        #endregion

        #region CTOR

        public JobViewModel()
        {
            _jobManager = CJobManager.LoadJobs();
        }

        #endregion

        public List<CJob> RunJobs(Tuple<int, int> pRange = null)
        {
            return _jobManager.RunJobs(pRange);
        }

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
