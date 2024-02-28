using CryptoSoft;
using LogsModels;
using System.Collections.Concurrent;
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
        private BlockingCollection<FileInfo> _priorityFilesQueue = new BlockingCollection<FileInfo>(new ConcurrentQueue<FileInfo>());
        private BlockingCollection<FileInfo> _nonPriorityFilesQueue = new BlockingCollection<FileInfo>(new ConcurrentQueue<FileInfo>());
        private ConcurrentBag<string> _ErrorMessages = new ConcurrentBag<string>();
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

        private bool IsPriorityExtension(string fileExtension, List<string>? priorityExtensions)
        {
            return priorityExtensions?.Any(ext => string.Equals(ext, fileExtension, StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        /// <summary>
        ///  parcourt récursivement le répertoire source, plaçant les fichiers dans les files d'attente appropriées.
        /// </summary>
        public override void CopyDirectoryAsync(DirectoryInfo sourceDir, DirectoryInfo targetDir, UpdateLogDelegate updateLog, bool recursive, bool differential, List<string>? priorityFileExtensions)
        {

            if (!sourceDir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir.FullName}");
            }

          

                Directory.CreateDirectory(targetDir.FullName);
                CLogState logState = new CLogState(); // Ou utilisez une instance existante si approprié

                // Producteur: enregistrement des fichiers dans les files d'attente avec priorité
                EnqueueFiles(sourceDir, priorityFileExtensions, recursive);

                // Consommateur: traitement des fichiers prioritaires
                ProcessFiles(_priorityFilesQueue, sourceDir, targetDir, logState, updateLog, differential); // Ajout du logState et differential ici

                // Consommateur: traitement des fichiers non prioritaires
                ProcessFiles(_nonPriorityFilesQueue, sourceDir, targetDir, logState, updateLog, differential); ; // De même ici
            
            
        }



        /// <summary>
        /// Trie les fichiers entre prioritaires et non prioritaires basé sur les extensions de fichiers.
        /// </summary>
        private void EnqueueFiles(DirectoryInfo directory, List<string>? priorityExtensions, bool recursive)
        {
            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20, // Limite le nombre de threads parallèles
                CancellationToken = _CancelationTokenSource.Token // Permet l'annulation de l'opération
            };

            Debug.WriteLine("Debut enqueu files");

            try
            {
                var options = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = recursive };
        

                // Pour chaque dossier, traitez les fichiers en parallèle

                foreach (var dirPath in Directory.EnumerateDirectories(directory.FullName, "*", options))
                {
                    DirectoryInfo dir = new DirectoryInfo(dirPath);
                    Parallel.ForEach(dir.EnumerateFiles("*", options), parallelOptions, file =>
                        {
                            try
                            {
                                bool isPriority = IsPriorityExtension(file.Extension, priorityExtensions);
                                var targetQueue = isPriority ? _priorityFilesQueue : _nonPriorityFilesQueue;
                                targetQueue.Add(file);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                Debug.WriteLine($"Accès refusé à {file.FullName}: {ex.Message}");
                               
                                // Ici, vous pouvez choisir de logger l'exception ou de la stocker pour un traitement ultérieur.
                            }
                        });

                }
                
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                {
                    if (ex is UnauthorizedAccessException)
                    {
                        // Gérer ou logger l'exception.
                        Debug.WriteLine($"Accès refusé : {ex.Message}");
                        return true; // Indique que l'exception a été gérée.
                    }
                    return false; // Indique que l'exception n'a pas été gérée.
                });
            }

            Debug.WriteLine("Fin enqueu files");
        }



        // Consommateur: Traite les fichiers dans la file d'attente en utilisant Parallel.ForEach pour le parallélisme.
        private void ProcessFiles(BlockingCollection<FileInfo> filesQueue, DirectoryInfo sourceDir, DirectoryInfo targetDir, CLogState logState, UpdateLogDelegate updateLog, bool differential)
        {
            try
            {

                ParallelOptions parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 20,
                    CancellationToken = _CancelationTokenSource.Token
                };

                Parallel.ForEach(filesQueue, parallelOptions, file =>
                {
               
                        // Calcul du chemin relatif
                        string relativePath = Path.GetRelativePath(sourceDir.FullName, file.FullName);
                        string targetFilePath = Path.Combine(targetDir.FullName, relativePath);

                        // Check for cancellation before doing work
                        //if (_cancelationtokensource.token.iscancellationrequested)
                        //    _cancelationtokensource.token.throwifcancellationrequested();
                        //else
                        //{
                        //    _pauseevent.wait(_cancelationtokensource.token);
                        //}


                        // Assurer que le dossier cible existe
                        string? targetDirectoryPath = Path.GetDirectoryName(targetFilePath);
                        if (targetDirectoryPath != null && !Directory.Exists(targetDirectoryPath))
                        {
                            Directory.CreateDirectory(targetDirectoryPath);
                        }

                        CopyFileAsync(file, targetFilePath, logState, updateLog, differential);

                });
            }
            catch (Exception ex)
            {
                _Errors += "\n" + ex.Message;
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

        private void CopyFileAsync(FileInfo sourceFile, string targetFilePath, CLogState logState, UpdateLogDelegate updateLog, bool differential)
        {
            try
            {
                // Vérification différentielle: copier seulement si le fichier de destination n'existe pas ou si le fichier source est plus récent
                FileInfo destinationFileInfo = new FileInfo(targetFilePath);
                if (!differential || !destinationFileInfo.Exists || sourceFile.LastWriteTime > destinationFileInfo.LastWriteTime)
                {
                    using (Stream sourceStream = File.OpenRead(sourceFile.FullName))
                    {
                        using (Stream destinationStream = File.Create(targetFilePath))
                        {
                            // Vérifie si le fichier est sur la liste noire pour le cryptage
                            if (_BlackList.Any(blacklisted => sourceFile.Extension.EndsWith(blacklisted, StringComparison.OrdinalIgnoreCase)))
                            {
                                // Le fichier est sur la liste noire, appliquez le cryptage
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    sourceStream.CopyTo(memoryStream);
                                    byte[] fileBytes = memoryStream.ToArray();

                                    // Appliquez ici la logique de cryptage
                                    CXorChiffrement xorEncryptor = new CXorChiffrement();
                                    byte[] key = Encoding.UTF8.GetBytes("secret"); // La clé de cryptage
                                    byte[] encryptedBytes = xorEncryptor.Encrypt(fileBytes, key);

                                    logState.EncryptTime = xorEncryptor.EncryptTime; // Mettez à jour le temps de cryptage
                                    destinationStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                                }
                            }
                            else
                            {
                                // Le fichier n'est pas sur la liste noire, copiez sans cryptage
                                sourceStream.CopyTo(destinationStream);
                            }
                        }
                    }

                    // Mise à jour du log après la copie du fichier
                    lock (_lock)
                    {
                        updateLog(logState, _FormatLog, sourceFile, targetFilePath, _StopWatch);
                    }
                }
            }
            catch (Exception ex)
            {
               
              
                 _Errors += $"\nError copying file '{sourceFile.Name}': {ex.Message}";
               
            }
        }


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
        #endregion
    }
}
