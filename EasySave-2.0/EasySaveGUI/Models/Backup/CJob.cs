using LogsModels;
using Stockage.Logs;
using Stockage.Save;
using System.Runtime.Serialization;
namespace Models.Backup
{
    /// <summary>
    /// Représente un travail/tâche à exécuter
    /// </summary>
    [DataContract]
    public class CJob : IPath
    {
        #region Attribute
        [DataMember]
        private string _Name;
        [DataMember]
        private string _SourceDirectory;
        [DataMember]
        private string _TargetDirectory;
        [DataMember]
        private ETypeBackup _BackupType;
        private SauveJobsAsync _SauveJobs;
        #endregion

        #region Property
        /// <summary>
        /// Nom du job de sauvegarde
        /// </summary>
        public string Name { get => _Name; set => _Name = value; }

        /// <summary>
        /// Répertoire source à sauvegarder
        /// </summary>
        public string SourceDirectory { get => _SourceDirectory; set => _SourceDirectory = value; }

        /// <summary>
        /// Répertoire cible de la sauvegarde
        /// </summary>
        public string TargetDirectory { get => _TargetDirectory; set => _TargetDirectory = value; }

        /// <summary>
        /// Type de sauvegarde
        /// </summary>
        public ETypeBackup BackupType { get => _BackupType; set => _BackupType = value; }
        public SauveJobsAsync SauveJobs { get => _SauveJobs; set => _SauveJobs = value; }



        #endregion

        #region CTOR
        /// <summary>
        /// Constructeur de job
        /// </summary>
        /// <param name="pName">Nom du job</param>
        /// <param name="pSourceDirectory">Chemin source</param>
        /// <param name="pTargetDirectory">Chemin destination</param>
        /// <param name="pTypeBackup">Type de sauvegarde</param>
        /// <remarks>Mahmoud Charif - 30/01/2024 - Création</remarks>
        public CJob(string pName, string pSourceDirectory, string pTargetDirectory, ETypeBackup pTypeBackup)
        {
            _Name = pName;
            _SourceDirectory = pSourceDirectory;
            _TargetDirectory = pTargetDirectory;
            _BackupType = pTypeBackup;
            _SauveJobs = new SauveJobsAsync();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lance l'exécution du job de sauvegarde
        /// </summary>
        /// <param name="pSauveJobs">Objet de sauvegarde des données de jobs</param>
        public async Task Run(List<CLogState> pLogState)
        {
            switch (BackupType)
            {
                case ETypeBackup.COMPLET:
                    await Backup(pLogState);
                    break;
                case ETypeBackup.DIFFERENTIEL:
                    await Backup(pLogState, true);
                    break;
            }
        }

        /// <summary>
        /// Réalise la sauvegarde des données
        /// </summary>
        /// <param name="pDifferentiel = false">Indique une recopie forcée</param>
        /// <param name="pSauveJobs">Objet de sauvegarde des jobs</param>
        private async Task Backup(List<CLogState> pLogStates, bool pDifferentiel = false)
        {
            try
            {
                DirectoryInfo lSourceDir = new DirectoryInfo(_SourceDirectory);
                DirectoryInfo lTargetDir = new DirectoryInfo(_TargetDirectory);

                if (_SourceDirectory != _TargetDirectory)
                {
                    await _SauveJobs.CopyDirectoryAsync(lSourceDir, lTargetDir, true, pLogStates, pDifferentiel);
                }
                else
                {
                    throw new Exception("La chemin cible et le chemin source est identique");
                }
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }
        }
        public override bool Equals(object? obj)
        {
            CJob lJob = obj as CJob;

            if (lJob == null)
                return false;

            return lJob.Name == _Name && lJob.SourceDirectory == _SourceDirectory && lJob.TargetDirectory == TargetDirectory;
        }
        #endregion
    }
}
