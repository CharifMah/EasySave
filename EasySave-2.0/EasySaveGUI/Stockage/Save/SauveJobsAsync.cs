using LogsModels;
using Stockage.Logs;
using System.Diagnostics;
using static Stockage.Logs.ILogger<uint>;

namespace Stockage.Save
{
    /// <summary>
    /// Classe permettant de sauvegarder des jobs et de les logger
    /// </summary>
    public class SauveJobsAsync : BaseSave, IDisposable
    {
        #region Attributes
        private readonly object _lock = new object();
        private CLogState _LogState;
        private string _FormatLog;
        private Stopwatch _StopWatch;
        #endregion

        #region Property
        public CLogState LogState { get => _LogState; set => _LogState = value; }
        public Stopwatch StopWatch { get => _StopWatch; set => _StopWatch = value; }

        #endregion

        #region CTOR
        /// <summary>
        /// Constructeur de SauveJobs
        /// </summary>
        /// <param name="pPath">Le chemin du dossier</param>
        public SauveJobsAsync(string? pPath = null, string pFormatLog = "json", Stopwatch? pStopwatch = null) : base(pPath)
        {
            _LogState = new CLogState();
            _LogState.TotalTransferedFile = 0;
            _FormatLog = pFormatLog;
            if (pStopwatch != null)
                _StopWatch = pStopwatch;
            else
                _StopWatch = Stopwatch.StartNew();
        }
        ~SauveJobsAsync()
        {
            Dispose();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Copy files and directory from the source path to the destinationPath
        /// </summary>
        /// <param name="pSourceDir">Path of the directory you want tot copy</param>
        /// <param name="pTargetDir">Path of the target directory</param>
        /// <param name="pRecursive">True if recursive</param>
        /// <param name="pDiffertielle">true if the backup is differential</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public string CopyDirectoryAsync(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, UpdateLogDelegate pUpdateLog, bool pRecursive, bool pDiffertielle = false)
        {
            FileInfo[] lFiles = pSourceDir.GetFiles();
            string lErrors = String.Empty;

            try
            {
                // cm - Check if the source directory exists
                if (!pSourceDir.Exists)
                    throw new DirectoryNotFoundException($"Source directory not found: {pSourceDir.FullName}");

                Directory.CreateDirectory(pTargetDir.FullName);

                ParallelOptions lParallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 20
                };
             
                // cm - Get files in the source directory and copy to the destination directory
                Parallel.For(0, lFiles.Length, lParallelOptions, i =>
                {
                    string lTargetFilePath = Path.Combine(pTargetDir.FullName, lFiles[i].Name);

                    // Vérifie si le fichier existe déjà  
                    if (lFiles[i].Exists && pDiffertielle)
                    {
                        // Compare les dates  
                        FileInfo ldestInfo = new FileInfo(lTargetFilePath);

                        if (lFiles[i].LastWriteTime > ldestInfo.LastWriteTime)
                        {
                            // cm -  Copy the file async if the target file is newer
                            lErrors += CopyFileAsync(lFiles[i].FullName, lTargetFilePath);
                            lock (_lock)
                            {
                                pUpdateLog(_LogState, _FormatLog, lFiles[i], lTargetFilePath, _StopWatch);
                            }
                        
                        }
                    }
                    else
                    {
                        // cm -  Copy the file async
                        lErrors += CopyFileAsync(lFiles[i].FullName, lTargetFilePath);
                        lock (_lock)
                        {
                            pUpdateLog(_LogState, _FormatLog, lFiles[i], lTargetFilePath, _StopWatch);
                        }
                   
                    }
                });

                // cm - If recursive and copying subdirectories, recursively call this method
                if (pRecursive)
                {
                    foreach (DirectoryInfo lSubDir in pSourceDir.GetDirectories())
                    {
                        DirectoryInfo lNewDestinationDir = pTargetDir.CreateSubdirectory(lSubDir.Name);
                        lErrors += CopyDirectoryAsync(lSubDir, lNewDestinationDir, pUpdateLog, true, pDiffertielle);
                    }
                }
            }
            catch (Exception ex)
            {
                lErrors += "\n" + ex;
            }

            return lErrors;
        }

        public  string CopyFileAsync(string pSourcePath, string pDestinationPath)
        {
            string lErrors = String.Empty;
            try
            {
                using Stream lSource = File.OpenRead(pSourcePath);
                using Stream lDestination = File.Create(pDestinationPath);
                lSource.CopyTo(lDestination);
            }
            catch (Exception ex)
            {
                lErrors += "\n" + ex;
            }
            return lErrors;
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
