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
        public bool CreateBackupJob(CJob lJob)
        {
            return _jobManager.CreateBackupJob(lJob);
        }


        /// <summary>
        /// Supprimer un ou plusieurs jobs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool DeleteJobs(string input)
        {
            var indices = ParseUserInput(input);
            var sortedIndices = indices.OrderByDescending(x => x).Distinct();
            bool jobDeleted = false;

            foreach (var index in sortedIndices)
            {
                if (index >= 0 && index < _jobManager.Jobs.Count)
                {
                    bool result = _jobManager.DeleteJobByIndex(index);
                    jobDeleted = result;
                }
            }

            SaveJobs();
            return jobDeleted;
        }

        /// <summary>
        /// Récupère tout les indices de selections de job
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private HashSet<int> ParseUserInput(string input)
        {
            HashSet<int> indices = new HashSet<int>();
            string[] parts = input.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                if (part.Contains('-'))
                {
                    string[] rangeParts = part.Split('-');
                    int start = int.Parse(rangeParts[0]);
                    int end = int.Parse(rangeParts[1]);

                    for (int i = start; i <= end; i++)
                    {
                        indices.Add(i);
                    }
                }
                else
                {
                    indices.Add(int.Parse(part));
                }
            }

            return indices;
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
