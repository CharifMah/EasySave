using LogsModels;
using Stockage;
using Stockage.Logs;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Models.Backup
{
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

        #endregion

        #region Property

        public string Name { get => _Name; set => _Name = value; }
        public string SourceDirectory { get => _SourceDirectory; set => _SourceDirectory = value; }
        public string TargetDirectory { get => _TargetDirectory; set => _TargetDirectory = value; }
        public ETypeBackup BackupType { get => _BackupType; set => _BackupType = value; }

        #endregion

        #region CTOR

        /// <summary>
        /// Constructeur de job
        /// </summary>
        /// <param name="pName">Nom du job</param>
        /// <param name="pSourceDestination">Chemin source</param>
        /// <param name="pDesitnationDirectory">Chemin destination</param>
        /// <param name="pTypeBackup">Type de sauvegarde</param>
        /// <remarks>Mahmoud Charif - 30/01/2024 - Création</remarks>
        public CJob(string pName, string pSourceDestination, string pDesitnationDirectory, ETypeBackup pTypeBackup)
        {
            _Name = pName;
            _SourceDirectory = pSourceDestination;
            _TargetDirectory = pDesitnationDirectory;
            _BackupType = pTypeBackup;
        }

        #endregion

        #region Methods

        public void Run()
        {
            switch (BackupType)
            {
                case ETypeBackup.COMPLET:
                    Backup(true);
                    break;
                case ETypeBackup.DIFFERENTIEL:
                    Backup(false);
                    break;
            }
        }

        private void Backup(bool pForceCopy)
        {
            try
            {
                DirectoryInfo lSourceDir = new DirectoryInfo(SourceDirectory);
                DirectoryInfo lTargetDir = new DirectoryInfo(TargetDirectory);
                SauveCollection lSauveCollection = new SauveCollection(_SourceDirectory);

                CLogState lLogState = new CLogState
                {
                    Name = lSourceDir.Name,
                    SourceDirectory = lSourceDir.FullName,
                    TargetDirectory = lTargetDir.FullName,
                    TotalSize = lSauveCollection.GetDirSize(lSourceDir.FullName),
                };

                Stopwatch lSw = Stopwatch.StartNew();
                lSw.Start();

                if (_SourceDirectory != _TargetDirectory)
                {
                    lSauveCollection.CopyDirectory(new DirectoryInfo(_SourceDirectory), new DirectoryInfo(_TargetDirectory), true, pForceCopy);

                    lSw.Stop();
                    lLogState.Date = DateTime.Now;
                    lLogState.ElapsedMilisecond = lSw.ElapsedMilliseconds;
                    CLogger<CLogBase>.GenericLogger.Log(lLogState);
                }
                else
                {
                    throw new Exception("La chemin cible et le chemin source est identique");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
