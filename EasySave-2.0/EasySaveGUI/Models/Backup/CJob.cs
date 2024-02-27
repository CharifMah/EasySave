using LogsModels;
using Models.Settings;
using Stockage.Logs;
using Stockage.Save;
using System.Runtime.Serialization;
using static Stockage.Logs.ILogger<uint>;
namespace Models.Backup
{
    /// <summary>
    /// Représente un travail/tâche à exécuter
    /// </summary>
    [DataContract]
    public class CJob : BaseModel, IPath, IDisposable
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
        public string Name { get => _Name; set { _Name = value; NotifyPropertyChanged(); } }

        /// <summary>
        /// Répertoire source à sauvegarder
        /// </summary>
        public string SourceDirectory
        {
            get => _SourceDirectory; set { _SourceDirectory = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Répertoire cible de la sauvegarde
        /// </summary>
        public string TargetDirectory
        {
            get => _TargetDirectory; set { _TargetDirectory = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Type de sauvegarde
        /// </summary>
        public ETypeBackup BackupType
        {
            get => _BackupType; set { _BackupType = value; NotifyPropertyChanged(); }
        }
        public SauveJobsAsync SauveJobs
        {
            get => _SauveJobs; set { _SauveJobs = value; NotifyPropertyChanged(); }
        }



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
            _SauveJobs = new SauveJobsAsync(CSettings.Instance.EncryptionExtensions);
        }
        ~CJob()
        {
            this.Dispose();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lance l'exécution du job de sauvegarde
        /// </summary>
        /// <param name="pSauveJobs">Objet de sauvegarde des données de jobs</param>
        public void Run(UpdateLogDelegate pUpdateLog)
        {
            string lResult = String.Empty;
            switch (BackupType)
            {
                case ETypeBackup.COMPLET:
                    Backup(pUpdateLog);
                    break;
                case ETypeBackup.DIFFERENTIEL:
                    Backup(pUpdateLog, true);
                    break;
            }
        }

        /// <summary>
        /// Réalise la sauvegarde des données
        /// </summary>
        /// <param name="pDifferentiel = false">Indique une recopie forcée</param>
        /// <param name="pSauveJobs">Objet de sauvegarde des jobs</param>
        private void Backup(UpdateLogDelegate pUpdateLog, bool pDifferentiel = false)
        {
            try
            {
                DirectoryInfo lSourceDir = new DirectoryInfo(_SourceDirectory);
                DirectoryInfo lTargetDir = new DirectoryInfo(_TargetDirectory);

                if (_SourceDirectory != _TargetDirectory)
                {
                    _SauveJobs.CopyDirectoryAsync(lSourceDir, lTargetDir, pUpdateLog, true, pDifferentiel);
                }
                else
                {
                    CLogger<CLogBase>.Instance.StringLogger.Log("La chemin cible et le chemin source est identique");
                }
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is CJob job &&
                   _Name == job._Name &&
                   _SourceDirectory == job._SourceDirectory &&
                   _TargetDirectory == job._TargetDirectory &&
                   _BackupType == job._BackupType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_Name, _SourceDirectory, _TargetDirectory, _BackupType);
        }

        public void Dispose()
        {
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
