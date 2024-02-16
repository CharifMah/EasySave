using LogsModels;
using Stockage.Logs;
using System.Diagnostics;
namespace Stockage.Save
{
    /// <summary>
    /// Classe permettant de sauvegarder des jobs et de les logger
    /// </summary>
    public class SauveJobsAsync : BaseSave
    {
        private int _TransferedFiles;
        private List<CLogState> _LogStates;
        private CLogState _LogState;
        private string _FormatLog;

        /// <summary>
        /// Le nombre de fichier transférer
        /// </summary>
        public int TransferedFiles { get => _TransferedFiles; set => _TransferedFiles = value; }
        public CLogState LogState { get => _LogState; set => _LogState = value; }

        /// <summary>
        /// Constructeur de SauveJobs
        /// </summary>
        /// <param name="pPath">Le chemin du dossier</param>
        public SauveJobsAsync(string pPath = null, string pFormatLog = "json") : base(pPath)
        {
            _LogStates = new List<CLogState>();
            _TransferedFiles = 0;
            _LogState = new CLogState();
            _FormatLog = pFormatLog;
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
            CLogger<List<CLogState>>.Instance.GenericLogger.Log(_LogStates, true, false, "Logs", "", _FormatLog);
        }

        /// <summary>
        /// Copy files and directory from the source path to the destinationPath
        /// </summary>
        /// <param name="pSourceDir">Path of the directory you want tot copy</param>
        /// <param name="pTargetDir">Path of the target directory</param>
        /// <param name="pRecursive">True if recursive</param>
        /// <param name="pDiffertielle">true if the backup is differential</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public override async Task CopyDirectoryAsync(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, bool pDiffertielle = false)
        {
            string lName = "Logs - " + DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                // cm - Check if the source directory exists
                if (!pSourceDir.Exists)
                    throw new DirectoryNotFoundException($"Source directory not found: {pSourceDir.FullName}");

                Directory.CreateDirectory(pTargetDir.FullName);
                FileInfo[] lFiles = pSourceDir.GetFiles();
                
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
                            await CopyFileAsync(lFiles[i].FullName, lTargetFilePath);
                            lSw.Stop();
                            _LogState.SourceDirectory = lFiles[i].FullName;
                            _LogState.TargetDirectory = lTargetFilePath;
                            _LogState.RemainingFiles = _LogState.EligibleFileCount - _TransferedFiles;
                            UpdateLog(_LogState);
                            CLogDaily lLogFilesDaily = new CLogDaily();
                            lLogFilesDaily.Name = "Update : " + lFiles[i].Name;
                            lLogFilesDaily.SourceDirectory = lFiles[i].FullName;
                            lLogFilesDaily.TargetDirectory = lTargetFilePath;
                            lLogFilesDaily.Date = DateTime.Now;
                            lLogFilesDaily.TotalSize = lFiles[i].Length;
                            lLogFilesDaily.TransfertTime = lSw.Elapsed.TotalMilliseconds;
                            CLogger<CLogDaily>.Instance.GenericLogger.Log(lLogFilesDaily, true, true, lName, "DailyLogs", _FormatLog);
                        }
                    }
                    else
                    {
                        await CopyFileAsync(lFiles[i].FullName, lTargetFilePath);

                        lSw.Stop();
                        _LogState.SourceDirectory = lFiles[i].FullName;
                        _LogState.TargetDirectory = lTargetFilePath;
                        _LogState.RemainingFiles = _LogState.EligibleFileCount - _TransferedFiles;
                        UpdateLog(_LogState);
                        CLogDaily lLogFilesDaily = new CLogDaily();
                        lLogFilesDaily.Name = lFiles[i].Name;
                        lLogFilesDaily.SourceDirectory = lFiles[i].FullName;
                        lLogFilesDaily.TargetDirectory = lTargetFilePath;
                        lLogFilesDaily.Date = DateTime.Now;
                        lLogFilesDaily.TotalSize = lFiles[i].Length;
                        lLogFilesDaily.TransfertTime = lSw.Elapsed.TotalMilliseconds;

                        CLogger<CLogDaily>.Instance.GenericLogger.Log(lLogFilesDaily, true, true, lName, "DailyLogs", _FormatLog);
                    }
                }

                // cm - If recursive and copying subdirectories, recursively call this method
                if (pRecursive)
                {
                    foreach (DirectoryInfo lSubDir in pSourceDir.GetDirectories())
                    {
                        DirectoryInfo lNewDestinationDir = pTargetDir.CreateSubdirectory(lSubDir.Name);
                        await CopyDirectoryAsync(lSubDir, lNewDestinationDir, true, pDiffertielle);
                    }
                }
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false, true, lName, "", _FormatLog);
            }
        }

        public async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.OpenRead(sourcePath))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
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
