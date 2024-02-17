using LogsModels;
using Stockage.Logs;
using System.Diagnostics;
using System.Xml.Linq;

namespace Stockage.Save
{
    /// <summary>
    /// Classe permettant de sauvegarder des jobs et de les logger
    /// </summary>
    public class SauveJobsAsync : BaseSave
    {
        private List<CLogState> _LogStates;
        private CLogState _LogState;
        private string _FormatLog;


        public CLogState LogState { get => _LogState; set => _LogState = value; }

        /// <summary>
        /// Constructeur de SauveJobs
        /// </summary>
        /// <param name="pPath">Le chemin du dossier</param>
        public SauveJobsAsync(string pPath = null, string pFormatLog = "json") : base(pPath)
        {
            _LogStates = new List<CLogState>();
            _LogState = new CLogState();
            _LogState.TotalTransferedFile = 0;
            _FormatLog = pFormatLog;
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

                    // Vérifie si le fichier existe déjà  
                    if (lFiles[i].Exists && pDiffertielle)
                    {
                        // Compare les dates  
                        FileInfo destInfo = new FileInfo(lTargetFilePath);

                        if (lFiles[i].LastWriteTime > destInfo.LastWriteTime)
                        {
                            // cm -  Copy the file async if the target file is newer
                            await CopyFileAsync(lFiles[i].FullName, lTargetFilePath);
                            lSw.Stop();
                            await UpdateLog(lFiles[i], lTargetFilePath, lSw, lName);
                        }
                    }
                    else
                    {
                        // cm -  Copy the file async
                        await CopyFileAsync(lFiles[i].FullName, lTargetFilePath);
                        lSw.Stop();
                        await UpdateLog(lFiles[i], lTargetFilePath, lSw, lName);
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

        private async Task UpdateLog(FileInfo pFileInfo,string pTargetFilePath,Stopwatch pSw,string pName)
        {
            _LogState.TotalTransferedFile++;
            _LogState.SourceDirectory = pFileInfo.FullName;
            _LogState.TargetDirectory = pTargetFilePath;
            _LogState.RemainingFiles = _LogState.EligibleFileCount - _LogState.TotalTransferedFile;
            _LogState.BytesCopied += pFileInfo.Length;
            _LogState.Progress = _LogState.BytesCopied / _LogState.TotalSize * 100;
            _LogState.ElapsedMilisecond = (long)pSw.Elapsed.TotalMilliseconds;
            _LogState.Date = DateTime.Now;
            CLogDaily lLogFilesDaily = new CLogDaily();
            lLogFilesDaily.Name = pFileInfo.Name;
            lLogFilesDaily.SourceDirectory = pFileInfo.FullName;
            lLogFilesDaily.TargetDirectory = pTargetFilePath;
            lLogFilesDaily.Date = DateTime.Now;
            lLogFilesDaily.TotalSize = pFileInfo.Length;
            lLogFilesDaily.TransfertTime = pSw.Elapsed.TotalMilliseconds;

            await Task.Run(() =>
            {
                CLogger<List<CLogState>>.Instance.GenericLogger.Log(_LogStates, true, false, "Logs", "", _FormatLog);

            });
            CLogger<CLogDaily>.Instance.GenericLogger.Log(lLogFilesDaily, true, true, pName, "DailyLogs", _FormatLog);
        }

        public async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using Stream source = File.OpenRead(sourcePath);
            using Stream destination = File.Create(destinationPath);
            await source.CopyToAsync(destination);
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
