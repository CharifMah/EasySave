using LogsModels;
using Models.Backup;
using Models.Settings;
using Newtonsoft.Json;
using Stockage.Logs;
using Stockage.Save;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace EasySaveGUI.ViewModels
{
    /// <summary>
    /// Classe JobViewModel  Gestionnaire de jobs
    public class JobViewModel : BaseViewModel
    {
        #region Attribute

        private CancellationTokenSource _CancellationTokenSource;
        [DataMember]
        private List<CLogDaily> _LogDailyBuffer;
        [DataMember]
        private CJob _SelectedJob;
        [DataMember]
        private ObservableCollection<CJob> _Jobs;
        [DataMember]
        private string _Name;
        [DataMember]
        private ObservableCollection<CJob> _jobsRunning;

        private ISauve _SauveCollection;

        private ObservableCollection<CJob> _PausedJobsByMonitor;

        public event Action<string> OnBusinessSoftwareDetected;

        #endregion

        #region Property

        /// <summary>
        /// Job selectionnée par l'utilisateur
        /// </summary>
        public CJob SelectedJob
        {
            get { return _SelectedJob; }
            set
            {
                _SelectedJob = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Liste des jobs gérés
        /// </summary>
        public ObservableCollection<CJob> Jobs { get => _Jobs; set { _Jobs = value; } }

        /// <summary>
        /// Nom du gestionnaire
        /// </summary>
        public string Name { get => _Name; set => _Name = value; }

        /// <summary>
        /// Interface de sauvegarde des données
        /// </summary>
        public ISauve SauveCollection { get => _SauveCollection; set => _SauveCollection = value; }
        /// <summary>
        /// Les jobs en cours d’exécution
        /// </summary>
        public ObservableCollection<CJob> JobsRunning { get => _jobsRunning; set { _jobsRunning = value; NotifyPropertyChanged(); } }

        public ObservableCollection<CJob> PausedJobsByMonitor { get => _PausedJobsByMonitor; set => _PausedJobsByMonitor = value; }


        #endregion

        #region CTOR

        /// <summary>
        /// Constructeur de JobViewModel initialise le chemin de sauvegarde
        /// </summary>
        public JobViewModel()
        {
            _Name = "JobManager";
            _LogDailyBuffer = new List<CLogDaily?>();
            _Jobs = new ObservableCollection<CJob>();
            _jobsRunning = new ObservableCollection<CJob>();
            _PausedJobsByMonitor = new ObservableCollection<CJob>();
            _CancellationTokenSource = new CancellationTokenSource();
            //Init la classe de sauvegarde avec le chemin definit par l'utilisateur ou celui par default
            if (string.IsNullOrEmpty(CSettings.Instance.JobConfigFolderPath))
                _SauveCollection = new SauveCollection(new FileInfo(CSettings.Instance.JobDefaultConfigPath).DirectoryName);
            else
                _SauveCollection = new SauveCollection(CSettings.Instance.JobConfigFolderPath);
        }

        #endregion

        #region Methods

        #region Public
        /// <summary>
        /// Lance l'exécution des jobs sélectionnés
        /// </summary>
        /// <param name="pJobs">Liste des jobs à lancer</param>
        /// <returns> Liste mise à jour des jobs avec leur état après exécution </returns>
        public async Task RunJobs(List<CJob> pJobs)
        {
            try
            {
                uint lIndex = 0;

                if (CheckBusinessSoftwareRunning(CSettings.Instance.BusinessSoftware))
                {
                    // popup + boutons fermer les processus des logiciels detectés
                    OnBusinessSoftwareDetected?.Invoke("Un logiciel métier est actuellement en exécution.\nVeuillez fermer le(s) processus en cours.");
                    return;
                }



                Task lMonitoringBusinessSoftware = Task.Run(async () =>
                {
                    await MonitorBusinessSoftware(_CancellationTokenSource.Token);
                });

                ParallelOptions lParallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 20,
                    CancellationToken = _CancellationTokenSource.Token
                };


                // cm - parcours les jobs
                await Parallel.ForEachAsync(pJobs, async (lJob, lCancellationTokenSource) =>
                {
                    Stopwatch lStopWatch = new Stopwatch();
                    lStopWatch.Start();

                    SauveJobsAsync _SauveJobs = new SauveJobsAsync(CSettings.Instance.EncryptionExtensions, "", CSettings.Instance.FormatLog.SelectedFormatLog.Value, lStopWatch);

                    lJob.SauveJobs = _SauveJobs;

                    lJob.SauveJobs.LogState.ElapsedMilisecond = (long)lStopWatch.Elapsed.TotalMilliseconds;
                    lJob.SauveJobs.LogState.Name = lIndex + ' ' + _SauveJobs.LogState.Name;
                    lJob.SauveJobs.LogState.TotalTransferedFile = 0;
                    lJob.SauveJobs.LogState.BytesCopied = 0;

                    List<string> lFiles = new List<string>();

                    await System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        // cm - Ajoute le job lancer dans la liste des job en cours cette liste est vidée lors du lancement d'autre job
                        _jobsRunning.Add(lJob);
                    });
                    string lErrors = string.Empty;

                    Task lTask = Task.Run(() =>
                    {
                        lFiles = GetAccessibleFiles(lJob.SourceDirectory);
                        lJob.SauveJobs.LogState.TotalSize = lFiles.Sum(file => new FileInfo(file).Length);
                        lJob.SauveJobs.LogState.EligibleFileCount = lFiles.Count;

                        // cm - Lance le job
                        lJob.Run(UpdateLog);

                        lStopWatch.Stop();

                    });
                    await lTask.ContinueWith(t =>
                    {
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(async () =>
                        {
                            foreach (var lLogDaily in _LogDailyBuffer)
                            {
                                string lName = "Logs - " + DateTime.Now.ToString("yyyy-MM-dd");
                                CLogger<CLogDaily>.Instance.GenericLogger.Log(lLogDaily, true, true, lName, "DailyLogs", lLogDaily.FormatLog);
                            }
                            _LogDailyBuffer.Clear();
                            if (UserViewModel.Instance.ClientViewModel != null)
                                await UserViewModel.Instance.UserSignalRService.SendClientViewModel(UserViewModel.Instance.ClientViewModel.ToJson());
                            if (!string.IsNullOrEmpty(lJob.SauveJobs.Errors))
                                CLogger<CLogBase>.Instance.StringLogger.Log(lJob.SauveJobs.Errors, false);
                        });
                    });

                    lJob.SauveJobs.LogState.Date = DateTime.Now;
                    lIndex++;
                    lJob.SauveJobs.LogState.ElapsedMilisecond = (long)lStopWatch.Elapsed.TotalSeconds;
                });
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }
        }
        /// <summary>
        /// Reprend les jobs selectionnée en cours
        /// </summary>
        /// <param name="pJobs">job qu'on veux reprendre</param>
        public void Resume(List<CJob> pJobs, bool isResumeByMonitor = false)
        {
            foreach (CJob lJob in pJobs)
            {
                lJob.SauveJobs.PauseEvent.Set();

                if(isResumeByMonitor)
                {
                    App.Current.Dispatcher.Invoke(() => _PausedJobsByMonitor.Remove(lJob));
                }
            }
        }
        /// <summary>
        /// Met en pause les jobs
        /// </summary>
        /// <param name="pJobs">jobs a mettre en pause</param>
        public void Pause(List<CJob> pJobs, bool isPausedByMonitor = false)
        {
            foreach (CJob lJob in pJobs)
            {
                lJob.SauveJobs.PauseEvent.Reset();

                // Si le job est mis en pause par le moniteur et qu'il n'est pas déjà dans la collection des jobs en pause du monitor
                if (isPausedByMonitor && !_PausedJobsByMonitor.Contains(lJob))
                {
                    App.Current.Dispatcher.Invoke(() => _PausedJobsByMonitor.Add(lJob));
                }
            }
        }
        /// <summary>
        /// Arrete definitivement les jobs
        /// </summary>
        /// <param name="pJobs">jobs a arreter</param>
        public void Stop(List<CJob> pJobs)
        {
            foreach (CJob lJob in pJobs)
            {
                lJob.SauveJobs.CancelationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Crée un nouveau job de sauvegarde
        /// </summary>
        /// <param name="lJob">Objet représentant le job de sauvegarde à créer</param>
        /// <returns>True si le job a été créé avec succès, false sinon</returns>
        /// <remarks> Created by Mehmeti Faik on 06/02/2024 Updated validation logic to handle null parameters</remarks>
        public bool CreateBackupJob(CJob lJob)
        {
            bool lResult = true;
            // cm - Verifies que on n'a pas atteint la maximum de job
            if (!_Jobs.Contains(lJob))
                _Jobs.Add(lJob);
            else
                lResult = false;
            return lResult;
        }

        /// <summary>
        /// Supprimé un job
        /// </summary>
        /// <param name="pJobs">List de jobs à supprimer</param>
        /// <returns>true si réussi</returns>
        /// <remarks>Mehmeti faik</remarks>
        public bool DeleteJobs(List<CJob> pJobs)
        {
            foreach (CJob lJob in pJobs)
            {
                _Jobs.Remove(lJob);
            }
            return true;
        }

        /// <summary>
        /// Sauvegarde le JobViewModel
        /// </summary>
        public void SaveJobs()
        {
            _SauveCollection.Sauver(this, _Name);
        }
        #endregion

        #region private


        private void UpdateLog(CLogState pLogState, string pFormatLog, FileInfo? pFileInfo, string pTargetFilePath, Stopwatch pSw)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                pLogState.TotalTransferedFile++;
                pLogState.SourceDirectory = pFileInfo.FullName;
                pLogState.BytesCopied += pFileInfo.Length;
                pLogState.TargetDirectory = pTargetFilePath;
                pLogState.RemainingFiles = pLogState.EligibleFileCount - pLogState.TotalTransferedFile;
                pLogState.Progress = pLogState.BytesCopied / pLogState.TotalSize * 100;
                pLogState.ElapsedMilisecond = (long)pSw.Elapsed.TotalSeconds;
                pLogState.Date = DateTime.Now;

                CLogDaily lLogFilesDaily = new CLogDaily();
                lLogFilesDaily.Name = pFileInfo.Name;
                lLogFilesDaily.SourceDirectory = pFileInfo.FullName;
                lLogFilesDaily.TotalSize = pFileInfo.Length;
                lLogFilesDaily.TargetDirectory = pTargetFilePath;
                lLogFilesDaily.Date = DateTime.Now;
                lLogFilesDaily.FormatLog = pFormatLog;
                lLogFilesDaily.Progress = pLogState.Progress;
                lLogFilesDaily.TransfertTime = pSw.Elapsed.TotalMilliseconds;

                CLogger<List<CLogState>>.Instance.GenericLogger.Log(_jobsRunning.Select(lJob => lJob.SauveJobs.LogState).ToList(), true, false, "Logs", "", pFormatLog);
                _LogDailyBuffer.Add(lLogFilesDaily);
            });
        }

        private async Task MonitorBusinessSoftware(CancellationToken cancellationToken)
        {
            List<string> businessSoftware = CSettings.Instance.BusinessSoftware;
            int previousCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (CheckBusinessSoftwareRunning(businessSoftware))
                {
                    int currentCountBeforePause = _PausedJobsByMonitor.Count;
                    Pause(this._jobsRunning.ToList(), true);

                    if (_PausedJobsByMonitor.Count > currentCountBeforePause || (currentCountBeforePause == 0 && _PausedJobsByMonitor.Any()))
                    {
                        OnBusinessSoftwareDetected?.Invoke("Un logiciel métier est actuellement en exécution.\nVeuillez fermer le(s) processus en cours.");
                    }

                    previousCount = _PausedJobsByMonitor.Count;
                    await Task.Delay(5000);
                }
                else
                {
                    if (this._PausedJobsByMonitor.Count > 0)
                    {
                        Resume(this._PausedJobsByMonitor.ToList(), true);
                    
                        if (!this._PausedJobsByMonitor.Any())
                        {
                            previousCount = 0;
                        }
                    }
                    await Task.Delay(1000);
                }
            }
        }

        private bool CheckBusinessSoftwareRunning(List<string> pBusinessSoftware)
        {
            foreach (string lSoftware in pBusinessSoftware)
            {
                string lProcessName = Path.GetFileNameWithoutExtension(lSoftware);

                if (Process.GetProcessesByName(lProcessName).Length > 0)
                {
                    return true;
                }
            }
            return false;
        }


        private List<string> GetAccessibleFiles(string rootPath)
        {
            var lAccessibleFiles = new List<string>();
            var lDirectories = new Stack<string>();
            lDirectories.Push(rootPath);

            while (lDirectories.Count > 0)
            {
                string lCurrentDir = lDirectories.Pop();
                string[] lSubDirs;

                try
                {
                    lSubDirs = Directory.GetDirectories(lCurrentDir);
                }
                catch (UnauthorizedAccessException e)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        CLogger<CLogBase>.Instance.StringLogger.Log($"Access denied to directory {lCurrentDir}: {e.Message}", false);

                    });
                    continue;
                }
                catch (Exception e)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        CLogger<CLogBase>.Instance.StringLogger.Log($"An error occurred while accessing directory {lCurrentDir}: {e.Message}", false);

                    });
                    continue;
                }

                string[] files;
                try
                {
                    files = Directory.GetFiles(lCurrentDir);
                }
                catch (UnauthorizedAccessException e)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        CLogger<CLogBase>.Instance.StringLogger.Log($"Access denied to files in directory {lCurrentDir}: {e.Message}", false);

                    });
                    continue;
                }
                catch (Exception e)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        CLogger<CLogBase>.Instance.StringLogger.Log($"An error occurred while accessing files in directory {lCurrentDir}: {e.Message}", false);

                    });
                    continue;
                }

                foreach (string file in files)
                {
                    try
                    {
                        // The FileInfo call can trigger an UnauthorizedAccessException
                        var lFileInfo = new FileInfo(file);
                        lAccessibleFiles.Add(file); // If we get here, the file is accessible
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            CLogger<CLogBase>.Instance.StringLogger.Log($"Access denied to file {file}: {e.Message}", false);
                        });

                    }
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also throw an UnauthorizedAccessException, but we've already
                // caught that possibility above.
                foreach (string str in lSubDirs)
                    lDirectories.Push(str);
            }

            return lAccessibleFiles;
        }
        #endregion

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}
