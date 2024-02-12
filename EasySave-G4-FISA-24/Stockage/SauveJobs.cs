using LogsModels;
using Stockage.Logs;
using System.Diagnostics;
namespace Stockage
{
    public class SauveJobs : BaseSave
    {
        private int _TransferedFiles;
        private List<CLogState> _LogStates;
        public int TransferedFiles { get => _TransferedFiles; set => _TransferedFiles = value; }
        public SauveJobs(string pPath) : base(pPath)
        {
            _LogStates = new List<CLogState>();
            _TransferedFiles = 0;
        }
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
        /// <param name="pForce">true if overwrite</param>
        /// <param name="pLogger">Logger</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public override void CopyDirectory(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, ref CLogState pLogState, bool pForce = false)
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
                lLogFilesDaily.IsSummary = false;
                // cm - Get files in the source directory and copy to the destination directory
                for (int i = 0; i < lFiles.Length; i++)
                {
                    Stopwatch lSw = Stopwatch.StartNew();
                    lSw.Start();
                    string lTargetFilePath = Path.Combine(pTargetDir.FullName, lFiles[i].Name);
                    _TransferedFiles++;
                    lFiles[i].CopyTo(lTargetFilePath, pForce);
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
                    lLogFilesDaily.TransfertTimeSecond = lSw.Elapsed.TotalSeconds;
                    CLogger<CLogBase>.GenericLogger.Log(lLogFilesDaily, true, true, lName);
                }
                // cm - If recursive and copying subdirectories, recursively call this method
                if (pRecursive)
                {
                    foreach (DirectoryInfo lSubDir in pSourceDir.GetDirectories())
                    {
                        DirectoryInfo lNewDestinationDir = pTargetDir.CreateSubdirectory(lSubDir.Name);
                        CopyDirectory(lSubDir, lNewDestinationDir, true, ref pLogState, pForce);
                    }
                }
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.StringLogger.Log(ex.Message, false, true, lName);
            }
        }
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
