using LogsModels;
using Models.Backup;
using Models.Settings;
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
    [DataContract]
    public class JobViewModel : BaseViewModel
    {
        #region Attribute

        private List<CLogDaily?> _LogDailyBuffer;
        private CJob _SelectedJob;
        [DataMember]
        private ObservableCollection<CJob> _Jobs;
        [DataMember]
        private string _Name;
        private ObservableCollection<CJob> _jobsRunning;
        private ISauve _SauveCollection;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

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
        public ObservableCollection<CJob> JobsRunning { get => _jobsRunning; set { _jobsRunning = value; } }

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

            //Init la classe de sauvegarde avec le chemin definit par l'utilisateur ou celui par default
            if (String.IsNullOrEmpty(CSettings.Instance.JobConfigFolderPath))
                _SauveCollection = new SauveCollection(new FileInfo(CSettings.Instance.JobDefaultConfigPath).DirectoryName);
            else
                _SauveCollection = new SauveCollection(CSettings.Instance.JobConfigFolderPath);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Lance l'exécution des jobs sélectionnés
        /// </summary>
        /// <param name="pJobs">Liste des jobs à lancer</param>
        /// <returns> Liste mise à jour des jobs avec leur état après exécution </returns>
        public async Task RunJobs(List<CJob> pJobs)
        {
            try
            {
                if (CheckBusinessSoftwareRunning(CSettings.Instance.BusinessSoftware))
                {
                    // popup + boutons fermer les processus des logiciels detectés
                    OnBusinessSoftwareDetected?.Invoke("Un logiciel métier est actuellement en exécution.\nVeuillez fermer le(s) processus en cours.");
                    return;
                }


                uint lIndex = 0;

                Task lMonitoringBusinessSoftware = Task.Run(() => MonitorBusinessSoftware(_cancellationTokenSource.Token));

                // cm - parcours les jobs
                foreach (CJob lJob in pJobs)
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

                    await App.Current.Dispatcher.BeginInvoke(() =>
                    {
                        // cm - Ajoute le job lancer dans la liste des job en cours cette liste est vidée lors du lancement d'autre job
                        _jobsRunning.Add(lJob);
                    });
                    string lErrors = String.Empty;

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
                        App.Current.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var lLogDaily in _LogDailyBuffer)
                            {
                                string lName = "Logs - " + DateTime.Now.ToString("yyyy-MM-dd");
                                CLogger<CLogDaily>.Instance.GenericLogger.Log(lLogDaily, true, true, lName, "DailyLogs", lLogDaily.FormatLog);
                            }
                            _LogDailyBuffer.Clear();
                        });

                        if (!string.IsNullOrEmpty(lJob.SauveJobs.Errors))
                        {
                            App.Current.Dispatcher.BeginInvoke(() =>
                            {
                                CLogger<CLogBase>.Instance.StringLogger.Log(lJob.SauveJobs.Errors, false);
                            });
                        }
                    });

                    lJob.SauveJobs.LogState.Date = DateTime.Now;
                    lIndex++;

                    lJob.SauveJobs.LogState.ElapsedMilisecond = (long)lStopWatch.Elapsed.TotalSeconds;
                }
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }
        }

        private async Task MonitorBusinessSoftware(CancellationToken cancellationToken)
        {
            List<string> businessSoftware = CSettings.Instance.BusinessSoftware;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (CheckBusinessSoftwareRunning(businessSoftware))
                {
                    // Met en pause les jobs
                    // .... To do pause jobs
                    await Task.Delay(5000);
                }
                else
                {
                    // Reprend les jobs
                    // .... To do continue jobs
                    await Task.Delay(1000);
                }
            }
        }

        private bool CheckBusinessSoftwareRunning(List<string> pBusinessSoftware)
        {
            foreach (string software in pBusinessSoftware)
            {
                string processName = Path.GetFileNameWithoutExtension(software);

                if (Process.GetProcessesByName(processName).Length > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private void UpdateLog(CLogState pLogState, string pFormatLog, FileInfo? pFileInfo, string pTargetFilePath, Stopwatch pSw)
        {
            App.Current.Dispatcher.BeginInvoke(() =>
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

        private List<string> GetAccessibleFiles(string rootPath)
        {
            var accessibleFiles = new List<string>();
            var directories = new Stack<string>();
            directories.Push(rootPath);

            while (directories.Count > 0)
            {
                string currentDir = directories.Pop();
                string[] subDirs;

                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                }
                catch (UnauthorizedAccessException e)
                {
                    CLogger<CLogBase>.Instance.StringLogger.Log($"Access denied to directory {currentDir}: {e.Message}", false);
                    continue;
                }
                catch (Exception e)
                {
                    CLogger<CLogBase>.Instance.StringLogger.Log($"An error occurred while accessing directory {currentDir}: {e.Message}", false);
                    continue;
                }

                string[] files;
                try
                {
                    files = Directory.GetFiles(currentDir);
                }
                catch (UnauthorizedAccessException e)
                {
                    CLogger<CLogBase>.Instance.StringLogger.Log($"Access denied to files in directory {currentDir}: {e.Message}", false);
                    continue;
                }
                catch (Exception e)
                {
                    CLogger<CLogBase>.Instance.StringLogger.Log($"An error occurred while accessing files in directory {currentDir}: {e.Message}", false);
                    continue;
                }

                foreach (string file in files)
                {
                    try
                    {
                        // The FileInfo call can trigger an UnauthorizedAccessException
                        var fileInfo = new FileInfo(file);
                        accessibleFiles.Add(file); // If we get here, the file is accessible
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        CLogger<CLogBase>.Instance.StringLogger.Log($"Access denied to file {file}: {e.Message}", false);
                    }
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also throw an UnauthorizedAccessException, but we've already
                // caught that possibility above.
                foreach (string str in subDirs)
                    directories.Push(str);
            }

            return accessibleFiles;
        }
        #endregion
    }
}
