using CryptoSoft;
using LogsModels;
using System.Diagnostics;
using System.Text;
using System.Threading;
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
        private string _Errors;
        private string[] _BlackList;
        private CancellationTokenSource _CancelationTokenSource;
        private ManualResetEventSlim _PauseEvent;

        #endregion

        #region Property
        public CancellationTokenSource CancelationTokenSource { get => _CancelationTokenSource; set => _CancelationTokenSource = value; }
        public CLogState LogState { get => _LogState; set => _LogState = value; }
        public Stopwatch StopWatch { get => _StopWatch; set => _StopWatch = value; }
        public string Errors { get => _Errors; set => _Errors = value; }
        public ManualResetEventSlim PauseEvent { get => _PauseEvent; set => _PauseEvent = value; }

        #endregion

        #region CTOR
        /// <summary>
        /// Constructeur de SauveJobs
        /// </summary>
        /// <param name="pPath">Le chemin du dossier</param>
        public SauveJobsAsync(List<string> pBlackList, string? pPath = null, string pFormatLog = "json", Stopwatch? pStopwatch = null) : base(pPath)
        {
            _LogState = new CLogState();
            _LogState.TotalTransferedFile = 0;
            _FormatLog = pFormatLog;
            if (pStopwatch != null)
                _StopWatch = pStopwatch;
            else
                _StopWatch = Stopwatch.StartNew();
            _Errors = String.Empty;

            _BlackList = pBlackList.ToArray();
            _CancelationTokenSource = new CancellationTokenSource();
            _PauseEvent = new ManualResetEventSlim(true);
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
        public override void CopyDirectoryAsync(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, UpdateLogDelegate pUpdateLog, bool pRecursive, bool pDiffertielle = false)
        {
            FileInfo[] lFiles = pSourceDir.GetFiles();

            try
            {
                // cm - Check if the source directory exists
                if (!pSourceDir.Exists)
                    throw new DirectoryNotFoundException($"Source directory not found: {pSourceDir.FullName}");

                Directory.CreateDirectory(pTargetDir.FullName);

                ParallelOptions lParallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 20,
                    CancellationToken = _CancelationTokenSource.Token,
                };

                // cm - Get files in the source directory and copy to the destination directory
                Parallel.For(0, lFiles.Length, lParallelOptions, i =>
                {
                    if (!_CancelationTokenSource.Token.IsCancellationRequested)
                        _PauseEvent.Wait(_CancelationTokenSource.Token);
                    string lTargetFilePath = Path.Combine(pTargetDir.FullName, lFiles[i].Name);

                    // Vérifie si le fichier existe déjà  
                    if (lFiles[i].Exists && pDiffertielle)
                    {
                        // Compare les dates  
                        FileInfo ldestInfo = new FileInfo(lTargetFilePath);

                        if (lFiles[i].LastWriteTime > ldestInfo.LastWriteTime)
                        {
                            // cm -  Copy the file async if the target file is newer
                            CopyFileAsync(lFiles[i], lTargetFilePath, _LogState);
                            lock (_lock)
                            {
                                pUpdateLog(_LogState, _FormatLog, lFiles[i], lTargetFilePath, _StopWatch);
                            }
                        }
                    }
                    else
                    {
                        // cm -  Copy the file async
                        CopyFileAsync(lFiles[i], lTargetFilePath, _LogState);
                        lock (_lock)
                        {
                            pUpdateLog(_LogState, _FormatLog, lFiles[i], lTargetFilePath, _StopWatch);
                        }
                    }
                });

                // cm - If recursive and copying subdirectories, recursively call this method
                if (pRecursive)
                {
                    if (_CancelationTokenSource.Token.IsCancellationRequested)
                        return;
                    _PauseEvent.Wait(_CancelationTokenSource.Token);

                    foreach (DirectoryInfo lSubDir in pSourceDir.GetDirectories())
                    {
                        DirectoryInfo lNewDestinationDir = pTargetDir.CreateSubdirectory(lSubDir.Name);
                        CopyDirectoryAsync(lSubDir, lNewDestinationDir, pUpdateLog, true, pDiffertielle);
                    }
                }
            }
            catch (Exception ex)
            {
                _Errors += "\n" + ex.Message;
            }
        }

        /// <summary>
        /// Copie une fichier de maniere asychrone et chiffre le fichier si il sont blacklister
        /// </summary>
        /// <param name="pSourcePath">chemin source</param>
        /// <param name="pDestinationPath">chemin cible</param>
        /// <returns></returns>
        public void CopyFileAsync(FileInfo pSourcePath, string pDestinationPath, CLogState pLogState)
        {
            try
            {
                using (Stream lSource = File.OpenRead(pSourcePath.FullName))
                {
                    using (Stream lDestination = File.Create(pDestinationPath))
                    {
                        // cm - Check if the sourcePath is blacklisted
                        if (!_BlackList.Any(lPath => pSourcePath.Extension.EndsWith(lPath, StringComparison.OrdinalIgnoreCase)))
                        {
                            // cm - If the file is blacklisted, we copy without encryption
                            lSource.CopyTo(lDestination);
                        }
                        else
                        {
                            // cm - If the file is not blacklisted, encrypt and then copy
                            using (MemoryStream lMemoryStream = new MemoryStream())
                            {
                                // cm - Copy the content of the file to a memory buffer
                                lSource.CopyTo(lMemoryStream);
                                byte[] lFileBytes = lMemoryStream.ToArray();

                                // cm - Encrypt the buffer with the XOR encryption
                                CXorChiffrement lXorEncryptor = new CXorChiffrement();
                                byte[] lKey = Encoding.UTF8.GetBytes("secret"); // Convert the string key to byte array
                                byte[] lEncryptedBytes = lXorEncryptor.Encrypt(lFileBytes, lKey);
                                pLogState.EncryptTime = lXorEncryptor.EncryptTime;
                                // cm - Write the encrypted data to the destination file
                                lDestination.Write(lEncryptedBytes, 0, lEncryptedBytes.Length);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _Errors += "\n" + ex.Message;
            }
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
