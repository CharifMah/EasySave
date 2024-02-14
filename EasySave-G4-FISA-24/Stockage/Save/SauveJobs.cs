using LogsModels;
using Stockage.Logs;
using System.Diagnostics;
namespace Stockage.Save
{
    /// <summary>
    /// Classe permettant de sauvegarder des jobs et de les logger
    /// </summary>
    public class SauveJobs : BaseSave
    {
        private int _TransferedFiles;
        private List<CLogState> _LogStates;
        public int TransferedFiles { get => _TransferedFiles; set => _TransferedFiles = value; }

        public SauveJobs(string pPath = null) : base(pPath)
        {
            _LogStates = new List<CLogState>();
            _TransferedFiles = 0;
        }

        /// <summary>
        /// UpdateLog
        /// </summary>
        /// <param name="logState">Log a jour</param>
        public void UpdateLog(CLogState logState)
        {
            if (_LogStates.Contains(logState))
                _LogStates.Remove(logState);
            _LogStates.Add(logState);
            CLogger<List<CLogState>>.GenericLogger.Log(_LogStates, true, false);
        }

        /// <summary>
        /// Copy files and directory from the soruce path to the destinationPath
        /// </summary>
        /// <param name="pSourceDir">Path of the directory you want tot copy</param>
        /// <param name="pTargetDir">Path of the target directory</param>
        /// <param name="pRecursive">True if recursive</param>
        /// <param name="pDiffertielle">true if the backup is differentiel</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public override void CopyDirectory(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, ref CLogState pLogState, bool pDiffertielle = false)
        {
            string lName = "Logs - " + DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                // cm - Check if the source directory exists
                if (!pSourceDir.Exists)
                    throw new DirectoryNotFoundException($"Source directory not found: {pSourceDir.FullName}");

                Directory.CreateDirectory(pTargetDir.FullName);
                FileInfo[] lFiles = pSourceDir.GetFiles();
                CLogDaily lLogFilesDaily = new CLogDaily();
                // cm - Get files in the source directory and copy to the destination directory
                for (int i = 0; i < lFiles.Length; i++)
                {
                    Stopwatch lSw = Stopwatch.StartNew();
                    lSw.Start();
                    string lTargetFilePath = Path.Combine(pTargetDir.FullName, lFiles[i].Name);
                    _TransferedFiles++;

                    // Vérifie si le fichier existe déjà  
                    if (lFiles[i].Exists && pDiffertielle)
                    {
                        // Compare les dates  
                        FileInfo destInfo = new FileInfo(lTargetFilePath);

                        if (lFiles[i].LastWriteTime > destInfo.LastWriteTime)
                        {
                            lFiles[i].CopyTo(lTargetFilePath, true);
                            lSw.Stop();
                            pLogState.SourceDirectory = lFiles[i].FullName;
                            pLogState.TargetDirectory = lTargetFilePath;
                            pLogState.RemainingFiles = pLogState.EligibleFileCount - _TransferedFiles;
                            UpdateLog(pLogState);
                            lLogFilesDaily.Name = "Update : " + lFiles[i].Name;
                            lLogFilesDaily.SourceDirectory = lFiles[i].FullName;
                            lLogFilesDaily.TargetDirectory = lTargetFilePath;
                            lLogFilesDaily.Date = DateTime.Now;
                            lLogFilesDaily.TotalSize = lFiles[i].Length;
                            lLogFilesDaily.TransfertTime = lSw.Elapsed.TotalSeconds;
                            CLogger<CLogBase>.GenericLogger.Log(lLogFilesDaily, true, true, lName);
                        }
                    }
                    else
                    {
                        lFiles[i].CopyTo(lTargetFilePath, true);
                        lSw.Stop();
                        pLogState.SourceDirectory = lFiles[i].FullName;
                        pLogState.TargetDirectory = lTargetFilePath;
                        pLogState.RemainingFiles = pLogState.EligibleFileCount - _TransferedFiles;
                        UpdateLog(pLogState);
                        lLogFilesDaily.Name = lFiles[i].Name;
                        lLogFilesDaily.SourceDirectory = lFiles[i].FullName;
                        lLogFilesDaily.TargetDirectory = lTargetFilePath;
                        lLogFilesDaily.Date = DateTime.Now;
                        lLogFilesDaily.TotalSize = lFiles[i].Length;
                        lLogFilesDaily.TransfertTime = lSw.Elapsed.TotalMilliseconds;
                        CLogger<CLogBase>.GenericLogger.Log(lLogFilesDaily, true, true, lName);
                    }
                }

                // cm - If recursive and copying subdirectories, recursively call this method
                if (pRecursive)
                {
                    foreach (DirectoryInfo lSubDir in pSourceDir.GetDirectories())
                    {
                        DirectoryInfo lNewDestinationDir = pTargetDir.CreateSubdirectory(lSubDir.Name);
                        CopyDirectory(lSubDir, lNewDestinationDir, true, ref pLogState, pDiffertielle);
                    }
                }
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.StringLogger.Log(ex.Message, false, true, lName);
            }
        }

        /// <summary>
        /// Calcule la taille d'un repertoire
        /// </summary>
        /// <param name="pPath">Chemin du repertoire</param>
        /// <returns>la taille du repertoire en bytes</returns>
        public long GetDirSize(string pPath)
        {
            try
            {
                return Directory.EnumerateFiles(pPath).Sum(x => new FileInfo(x).Length)
                    +
                       Directory.EnumerateDirectories(pPath).Sum(x => GetDirSize(x));
            }
            catch
            {
                return 0L;
            }
        }
    }
}
