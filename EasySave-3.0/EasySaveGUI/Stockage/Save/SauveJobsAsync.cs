﻿using CryptoSoft;
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
        CXorChiffrement _XorEncryptor;

        private CancellationTokenSource _CancelationTokenSource;

        private ManualResetEventSlim _PauseEvent;

        private BlockingCollection<FileInfo> _priorityFilesQueue;
        private BlockingCollection<FileInfo> _nonPriorityFilesQueue;
        #endregion

        #region Property
        public CancellationTokenSource CancelationTokenSource { get => _CancelationTokenSource; set => _CancelationTokenSource = value; }
        public CLogState LogState { get => _LogState; set => _LogState = value; }
        public Stopwatch StopWatch { get => _StopWatch; set => _StopWatch = value; }
        public string Errors { get => _Errors; set => _Errors = value; }
        public ManualResetEventSlim PauseEvent { get => _PauseEvent; set => _PauseEvent = value; }
        public BlockingCollection<FileInfo> PriorityFilesQueue { get => _priorityFilesQueue; set => _priorityFilesQueue = value; }
        public BlockingCollection<FileInfo> NonPriorityFilesQueue { get => _nonPriorityFilesQueue; set => _nonPriorityFilesQueue = value; }

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

            _priorityFilesQueue = new BlockingCollection<FileInfo>(new ConcurrentQueue<FileInfo>());
            _nonPriorityFilesQueue = new BlockingCollection<FileInfo>(new ConcurrentQueue<FileInfo>());

            _XorEncryptor = new CXorChiffrement();
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
            return priorityExtensions?.Any(ext => fileExtension.EndsWith(ext)) == true;
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
        private void EnqueueFiles(DirectoryInfo sourceDir, List<string>? priorityExtensions)
        {
            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20,
                CancellationToken = _CancelationTokenSource.Token
            };


            List<string> allFiles = new List<string>();

            CollectFilesRecursively(sourceDir, allFiles);

            try
            {
                Parallel.ForEach(allFiles, parallelOptions, filePath =>
                {
                    try
                    {
                        FileInfo file = new FileInfo(filePath);

                        if (_CancelationTokenSource.Token.IsCancellationRequested)
                            _CancelationTokenSource.Token.ThrowIfCancellationRequested();
                        else
                        {
                            _PauseEvent.Wait(_CancelationTokenSource.Token);
                        }

                        bool isPriority = IsPriorityExtension(file.Extension, priorityExtensions);
                        BlockingCollection<FileInfo> targetQueue = isPriority ? _priorityFilesQueue : _nonPriorityFilesQueue;
                        targetQueue.Add(file);
                    }
                    catch (Exception ex)
                    {
                        _Errors += "\n" + ex.Message;
                    }
                });
            }
            finally
            {
                _priorityFilesQueue.CompleteAdding();
                _nonPriorityFilesQueue.CompleteAdding();
            }
        }

        private void CollectFilesRecursively(DirectoryInfo dir, List<string> allFiles)
        {
            try
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    allFiles.Add(file.FullName);
                }

                DirectoryInfo[] subDirs = dir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    CollectFilesRecursively(subDir, allFiles);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                _Errors += "\n" + e.Message;
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
            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20,
                CancellationToken = _CancelationTokenSource.Token
            };

            try
            {
                Parallel.ForEach(pFilesQueue, parallelOptions, file =>
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
                    }
                    catch (Exception ex)
                    {
                        return;
                    }


                    string? lTargetDirectoryPath = Path.GetDirectoryName(lTargetFilePath);
                    if (lTargetDirectoryPath != null && !Directory.Exists(lTargetDirectoryPath))
                    {
                        Directory.CreateDirectory(lTargetDirectoryPath);
                    }

                    CopyFileAsync(file, lTargetFilePath, pDifferential);

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
        /// <param name="pTargetFilePath"></param>mp
        /// <param name="pLogState"></param>
        /// <param name="pUpdateLog"></param>
        /// <param name="pDifferential"></param>
        public override void CopyFileAsync(FileInfo pSourceFile, string pTargetFilePath, bool pDifferential)
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
                            // Vérifie si le fichier est sur la backlist pour le chiffrement
                            if (_BlackList.Any(blacklisted => pSourceFile.Extension.EndsWith(blacklisted, StringComparison.OrdinalIgnoreCase)))
                            {
                                // Le fichier est sur la blacklist, appliquez le chiffrement
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    lSourceStream.CopyTo(memoryStream);
                                    byte[] fileBytes = memoryStream.ToArray();

                                    byte[] key = Encoding.UTF8.GetBytes("secret"); // La clé de chiffrement
                                    byte[] encryptedBytes = _XorEncryptor.Encrypt(fileBytes, key);

                                    _LogState.EncryptTime = _XorEncryptor.EncryptTime; // Mettez à jour le temps de chiffrement
                                    lDestinationStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                                }
                            }
                            else
                            {
                                // Le fichier n'est pas sur la liste noire, copiez sans chiffrement
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

        #endregion
    }
}
