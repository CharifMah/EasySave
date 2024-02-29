using CryptoSoft;
using LogsModels;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using static Stockage.Logs.ILogger<uint>;

namespace Stockage.Save
{
    /// <summary>
    /// Classe permettant de sauvegarder des jobs et de les logger
    /// </summary>
    [DataContract]
    public class SauveJobsAsync : BaseSave, IDisposable
    {
        #region Attributes
        private readonly object _lock = new object();
        [DataMember]
        private CLogState _LogState;
        private string _FormatLog;
        [DataMember]
        private Stopwatch _StopWatch;
        [DataMember]
        private string _Errors;
        [DataMember]
        private string[] _BlackList;

        private CancellationTokenSource _CancelationTokenSource;

        private ManualResetEventSlim _PauseEvent;

        private BlockingCollection<FileInfo> _priorityFilesQueue = new BlockingCollection<FileInfo>(new ConcurrentQueue<FileInfo>());
        private BlockingCollection<FileInfo> _nonPriorityFilesQueue = new BlockingCollection<FileInfo>(new ConcurrentQueue<FileInfo>());
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
        /// <param name="pBlackList"></param>
        /// <param name="pPath"></param>
        /// <param name="pFormatLog"></param>
        /// <param name="pStopwatch"></param>
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
            if (pBlackList == null)
                pBlackList = new List<string>();

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

        public void Dispose()
        {
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            _CancelationTokenSource.Dispose();
            _PauseEvent.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Renvoi un état booléen sur le fichier pour déterminier 
        /// selon des extensions définis, si il est prioritaire ou non.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="priorityExtensions"></param>
        /// <returns></returns>
        private bool IsPriorityExtension(string fileExtension, List<string>? priorityExtensions)
        {
            return priorityExtensions?.Any(ext => string.Equals(ext, fileExtension, StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        /// <summary>
        /// Copie de manière asynchrone le contenu d'un répertoire source vers un répertoire cible,
        /// en prenant en compte la possibilité d'une copie récursive et différentielle,
        /// et en appliquant une priorité de copie basée sur les extensions de fichier spécifiées.
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="targetDir"></param>
        /// <param name="updateLog"></param>
        /// <param name="recursive"></param>
        /// <param name="differential"></param>
        /// <param name="priorityFileExtensions"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public override void CopyDirectoryAsync(DirectoryInfo sourceDir, DirectoryInfo targetDir, UpdateLogDelegate updateLog, bool recursive, bool differential, List<string>? priorityFileExtensions)
        {

            if (!sourceDir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir.FullName}");
            }

            Directory.CreateDirectory(targetDir.FullName);
            CLogState logState = new CLogState();

            // Producteur: enregistrement des fichiers dans les files d'attente avec priorité
            EnqueueFiles(sourceDir, priorityFileExtensions);

            // Consommateur: traitement des fichiers prioritaires
            ProcessFiles(_priorityFilesQueue, sourceDir, targetDir, logState, updateLog, differential);

            // Consommateur: traitement des fichiers non prioritaires
            ProcessFiles(_nonPriorityFilesQueue, sourceDir, targetDir, logState, updateLog, differential);
        }

        /// <summary>
        /// Trie les fichiers entre prioritaires et non prioritaires basé sur les extensions de fichiers.
        /// </summary>
        /// <param name="pSourceDir"></param>
        /// <param name="pPriorityExtensions"></param>
        /// <param name="pParallelOptions"></param>
        /// <param name="pOptions"></param>
        private void EnqueueFiles(DirectoryInfo pSourceDir, List<string>? pPriorityExtensions)
        {
            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20, // Limite de nombre de threads en parallèles
                CancellationToken = _CancelationTokenSource.Token // Annulation de l'opération
            };

            EnumerationOptions options = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true };

            try
            {
                Parallel.ForEach(pSourceDir.EnumerateFiles("*", options), parallelOptions, file =>
                {
                    // Check for cancellation before doing work
                    if (_CancelationTokenSource.Token.IsCancellationRequested)
                        _CancelationTokenSource.Token.ThrowIfCancellationRequested();
                    else
                    {
                        _PauseEvent.Wait(_CancelationTokenSource.Token);
                    }

                    bool isPriority = IsPriorityExtension(file.Extension, pPriorityExtensions);
                    BlockingCollection<FileInfo> targetQueue = isPriority ? _priorityFilesQueue : _nonPriorityFilesQueue;
                    targetQueue.Add(file);
                });

                _priorityFilesQueue.CompleteAdding();
                _nonPriorityFilesQueue.CompleteAdding();
            }
            catch (Exception ex)
            {
                _Errors += "\n" + ex.Message;

                _priorityFilesQueue.CompleteAdding();
                _nonPriorityFilesQueue.CompleteAdding();
            }
        }


        /// <summary>
        ///  Consommateur: Traite la sauvegarde des fichiers dans la file d'attente en utilisant Parallel.ForEach pour le parallélisme
        /// </summary>
        /// <param name="pFilesQueue"></param>
        /// <param name="pSourceDir"></param>
        /// <param name="pTargetDir"></param>
        /// <param name="pLogState"></param>
        /// <param name="pUpdateLog"></param>
        /// <param name="pDifferential"></param>
        private void ProcessFiles(BlockingCollection<FileInfo> pFilesQueue, DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, CLogState pLogState, UpdateLogDelegate pUpdateLog, bool pDifferential)
        {
            try
            {
                ParallelOptions parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 20,
                    CancellationToken = _CancelationTokenSource.Token
                };

                Parallel.ForEach(pFilesQueue.GetConsumingEnumerable(), parallelOptions, file =>
                {
                    string lRelativePath = Path.GetRelativePath(pSourceDir.FullName, file.FullName);
                    string lTargetFilePath = Path.Combine(pTargetDir.FullName, lRelativePath);
                    
                    try
                    {
                        // Check for cancellation before doing work
                        if (_CancelationTokenSource.Token.IsCancellationRequested)
                            _CancelationTokenSource.Token.ThrowIfCancellationRequested();
                        else
                        {
                            _PauseEvent.Wait(_CancelationTokenSource.Token);
                        }
                    } catch (Exception ex)
                    {
                        return;
                    }
                    

                    string? lTargetDirectoryPath = Path.GetDirectoryName(lTargetFilePath);
                    if (lTargetDirectoryPath != null && !Directory.Exists(lTargetDirectoryPath))
                    {
                        Directory.CreateDirectory(lTargetDirectoryPath);
                    }

                    CopyFileAsync(file, lTargetFilePath, pLogState, pUpdateLog, pDifferential);

                    lock (_lock)
                    {
                        pUpdateLog(_LogState, _FormatLog, file, lTargetFilePath, _StopWatch);
                    }
                });
            }
            catch (Exception ex)
            {
                _Errors += "\n" + ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSourceFile"></param>
        /// <param name="pTargetFilePath"></param>
        /// <param name="pLogState"></param>
        /// <param name="pUpdateLog"></param>
        /// <param name="pDifferential"></param>
        private void CopyFileAsync(FileInfo pSourceFile, string pTargetFilePath, CLogState pLogState, UpdateLogDelegate pUpdateLog, bool pDifferential)
        {
            try
            {
                // Vérification différentielle: copier seulement si le fichier de destination n'existe pas ou si le fichier source est plus récent
                FileInfo lDestinationFileInfo = new FileInfo(pTargetFilePath);

                if (!pDifferential || !lDestinationFileInfo.Exists || pSourceFile.LastWriteTime > lDestinationFileInfo.LastWriteTime)
                {
                    using (Stream lSourceStream = File.OpenRead(pSourceFile.FullName))
                    {
                        using (Stream lDestinationStream = File.Create(pTargetFilePath))
                        {
                            // Vérifie si le fichier est sur la backlist pour le cryptage
                            if (_BlackList.Any(blacklisted => pSourceFile.Extension.EndsWith(blacklisted, StringComparison.OrdinalIgnoreCase)))
                            {
                                // Le fichier est sur la blacklist, appliquez le cryptage
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    lSourceStream.CopyTo(memoryStream);
                                    byte[] fileBytes = memoryStream.ToArray();

                                    // Cryptage
                                    CXorChiffrement xorEncryptor = new CXorChiffrement();
                                    byte[] key = Encoding.UTF8.GetBytes("secret"); // La clé de cryptage
                                    byte[] encryptedBytes = xorEncryptor.Encrypt(fileBytes, key);

                                    pLogState.EncryptTime = xorEncryptor.EncryptTime; // Mettez à jour le temps de cryptage
                                    lDestinationStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                                }
                            }
                            else
                            {
                                // Le fichier n'est pas sur la liste noire, copiez sans cryptage
                                lSourceStream.CopyTo(lDestinationStream);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _Errors += $"\nError copying file '{pSourceFile.Name}': {ex.Message}";

            }
        }


        /// <summary>
        /// Copy files and directory from the source path to the destinationPath
        /// </summary>
        /// <param name="pSourceDir">Path of the directory you want tot copy</param>
        /// <param name="pTargetDir">Path of the target directory</param>
        /// <param name="pRecursive">True if recursive</param>
        /// <param name="pDiffertielle">true if the backup is differential</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        //public override void CopyDirectoryAsync(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, UpdateLogDelegate pUpdateLog, bool pRecursive, bool pDiffertielle = false, List<string>? pPriorityFileExtensions = null)
        //{

        //    FileInfo[] lFiles = pSourceDir.GetFiles();

        //    try
        //    {
        //        // cm - Check if the source directory exists
        //        if (!pSourceDir.Exists)
        //            throw new DirectoryNotFoundException($"Source directory not found: {pSourceDir.FullName}");

        //        Directory.CreateDirectory(pTargetDir.FullName);

        //        ParallelOptions lParallelOptions = new ParallelOptions
        //        {
        //            MaxDegreeOfParallelism = 20,
        //            CancellationToken = _CancelationTokenSource.Token
        //        };

        //        // cm - Get files in the source directory and copy to the destination directory
        //        Parallel.For(0, lFiles.Length, lParallelOptions, i =>
        //        {
        //            string lTargetFilePath = Path.Combine(pTargetDir.FullName, lFiles[i].Name);
        //            // Check for cancellation before doing work
        //            if (_CancelationTokenSource.Token.IsCancellationRequested)
        //                _CancelationTokenSource.Token.ThrowIfCancellationRequested();
        //            else
        //            {
        //                _PauseEvent.Wait(_CancelationTokenSource.Token);
        //            }


        //            // Vérifie si le fichier existe déjà  
        //            if (lFiles[i].Exists && pDiffertielle)
        //            {
        //                // Compare les dates  
        //                FileInfo ldestInfo = new FileInfo(lTargetFilePath);

        //                if (lFiles[i].LastWriteTime > ldestInfo.LastWriteTime)
        //                {
        //                    // cm -  Copy the file async if the target file is newer
        //                    CopyFileAsync(lFiles[i], lTargetFilePath, _LogState);
        //                    lock (_lock)
        //                    {
        //                        pUpdateLog(_LogState, _FormatLog, lFiles[i], lTargetFilePath, _StopWatch);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // cm -  Copy the file async
        //                CopyFileAsync(lFiles[i], lTargetFilePath, _LogState);
        //                lock (_lock)
        //                {
        //                    pUpdateLog(_LogState, _FormatLog, lFiles[i], lTargetFilePath, _StopWatch);
        //                }
        //            }
        //        });

        //        // cm - If recursive and copying subdirectories, recursively call this method
        //        if (pRecursive)
        //        {
        //            if (_CancelationTokenSource.Token.IsCancellationRequested)
        //                return;
        //            _PauseEvent.Wait(_CancelationTokenSource.Token);

        //            foreach (DirectoryInfo lSubDir in pSourceDir.GetDirectories())
        //            {
        //                DirectoryInfo lNewDestinationDir = pTargetDir.CreateSubdirectory(lSubDir.Name);
        //                CopyDirectoryAsync(lSubDir, lNewDestinationDir, pUpdateLog, true, pDiffertielle);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _Errors += "\n" + ex.Message;
        //    }
        //}


        /// <summary>
        /// Copie une fichier de maniere asychrone et chiffre le fichier si il sont blacklister
        /// </summary>
        /// <param name="pSourcePath">chemin source</param>
        /// <param name="pDestinationPath">chemin cible</param>
        /// <returns></returns>
        //public void CopyFileAsync(FileInfo pSourcePath, string pDestinationPath, CLogState pLogState)
        //{
        //    try
        //    {
        //        using (Stream lSource = File.OpenRead(pSourcePath.FullName))
        //        {
        //            using (Stream lDestination = File.Create(pDestinationPath))
        //            {
        //                // cm - Check if the sourcePath is blacklisted
        //                if (!_BlackList.Any(lPath => pSourcePath.Extension.EndsWith(lPath, StringComparison.OrdinalIgnoreCase)))
        //                {
        //                    // cm - If the file is blacklisted, we copy without encryption
        //                    lSource.CopyTo(lDestination);
        //                }
        //                else
        //                {
        //                    // cm - If the file is not blacklisted, encrypt and then copy
        //                    using (MemoryStream lMemoryStream = new MemoryStream())
        //                    {
        //                        // cm - Copy the content of the file to a memory buffer
        //                        lSource.CopyTo(lMemoryStream);
        //                        byte[] lFileBytes = lMemoryStream.ToArray();

        //                        // cm - Encrypt the buffer with the XOR encryption
        //                        CXorChiffrement lXorEncryptor = new CXorChiffrement();
        //                        byte[] lKey = Encoding.UTF8.GetBytes("secret"); // Convert the string key to byte array
        //                        byte[] lEncryptedBytes = lXorEncryptor.Encrypt(lFileBytes, lKey);
        //                        pLogState.EncryptTime = lXorEncryptor.EncryptTime;
        //                        // cm - Write the encrypted data to the destination file
        //                        lDestination.Write(lEncryptedBytes, 0, lEncryptedBytes.Length);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _Errors += "\n" + ex.Message;
        //    }
        //}
        #endregion
    }
}
