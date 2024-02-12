using LogsModels;
using Stockage;
using System.Diagnostics;
using System.Reflection.Metadata;
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

        private CLogState _LogState;
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

            _LogState = new CLogState();
        }

        #endregion

        #region Methods

        public void Run(SauveJobs pSauveJobs)
        {
            switch (BackupType)
            {
                case ETypeBackup.COMPLET:
                    Backup(true, pSauveJobs);
                    break;
                case ETypeBackup.DIFFERENTIEL:
                    Backup(false, pSauveJobs);
                    break;
            }
        }

        private void Backup(bool pForceCopy, SauveJobs pSauveJobs)
        {
            try
            {
                DirectoryInfo lSourceDir = new DirectoryInfo(_SourceDirectory);
                DirectoryInfo lTargetDir = new DirectoryInfo(_TargetDirectory);

                _LogState.TotalSize = pSauveJobs.GetDirSize(lSourceDir.FullName);
                _LogState.Name = Name + " - " + lSourceDir.Name;
                _LogState.SourceDirectory = lSourceDir.FullName;
                _LogState.TargetDirectory = lTargetDir.FullName;
                _LogState.EligibleFileCount = lSourceDir.GetFiles("*", SearchOption.AllDirectories).Length;

                Stopwatch lSw = Stopwatch.StartNew();
                lSw.Start();

                _LogState.RemainingFiles = _LogState.EligibleFileCount;
                _LogState.Date = DateTime.Now;
                _LogState.ElapsedMilisecond = lSw.ElapsedMilliseconds;
                _LogState.IsActive = true;
                pSauveJobs.UpdateLog(_LogState);

                if (_SourceDirectory != _TargetDirectory)
                {
                    pSauveJobs.CopyDirectory(lSourceDir, lTargetDir, true, ref _LogState, pForceCopy);

                    lSw.Stop();
                    _LogState.Date = DateTime.Now;
                    _LogState.RemainingFiles = 0;
                    _LogState.ElapsedMilisecond = lSw.ElapsedMilliseconds;
                    _LogState.IsActive = false;
                    _LogState.IsSummary = true;
                    pSauveJobs.UpdateLog(_LogState);
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

        public override bool Equals(object? obj)
        {
            CJob lJob = (obj as CJob);
            return lJob.Name == this._Name && lJob.SourceDirectory == this._SourceDirectory && lJob.TargetDirectory == this.TargetDirectory;
        }

        #endregion
    }
}
