using EasySave.ViewModels;
using EasySaveDraft.Resources;
using LogsModels;
using Models.Backup;
using Stockage.Logs;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace EasySave.Views
{
    internal class JobView : BaseView
    {
        #region Attributes
        private JobViewModel _JobVm;
        public override string Title => "JobView";
        #endregion

        #region CTOR
        public JobView(JobViewModel pJobVm)
        {
            _JobVm = pJobVm;
            CLogger<CLogBase>.GenericLogger.Datas.CollectionChanged += LogGenericData_CollectionChanged;
            CLogger<CLogBase>.StringLogger.Datas.CollectionChanged += LogStringData_CollectionChanged;
        }


        #endregion

        #region Methods

        #region public

        public override void Run()
        {
            Console.WriteLine();
            ListJobs();
            Console.WriteLine();

            if (_JobVm.JobManager.Jobs.Any())
            {
                Tuple<int, int> lRange = SelectJobs(_JobVm.JobManager.Jobs);
                if (lRange == null)
                    return;

                List<CJob> lJobsRunning = _JobVm.RunJobs(lRange);

                foreach (CJob lJobRunning in lJobsRunning)
                    ConsoleExtention.WriteLineSucces($"Job {lJobRunning.Name} copy is finished");
            }
            else
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("NoJobCreated").ToString());
        }

        /// <summary>
        /// Print all jobs
        /// </summary>
        public void ListJobs()
        {
            if (_JobVm.JobManager != null && _JobVm.JobManager.Jobs.Any())
            {
                int lConsoleWidth = Console.WindowWidth;
                int lNameColumnWidth = 30;
                int lPathSourceColumnWidth = (lConsoleWidth - lNameColumnWidth) / 2;
                int lPathTargetColumnWidth = lConsoleWidth - lNameColumnWidth - lPathSourceColumnWidth - 2;

                // cm - Ecrit le nom de la config
                ConsoleExtention.WriteTitle(_JobVm.JobManager.Name);

                Console.ForegroundColor = ConsoleColor.Yellow;
                // cm - Affiche les colonne du tableau
                Console.WriteLine(
                    "{0,-30} {1,-" + lPathSourceColumnWidth + "} {2,-" + lPathTargetColumnWidth + "}",
                    $"{Strings.ResourceManager.GetObject("Name")} : ",
                    $"{Strings.ResourceManager.GetObject("SourceDir")} : ",
                    $"{Strings.ResourceManager.GetObject("TargetDir")} : ");

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                Console.ForegroundColor = ConsoleColor.Green;
                // cm - Affiche les ligne du tableau
                for (int i = 0; i < _JobVm.JobManager.Jobs.Count; i++)
                {
                    string? lTruncatedSource = TruncateMiddle(_JobVm.JobManager.Jobs[i].SourceDirectory.Replace(@"\", @"\\"), lPathSourceColumnWidth);
                    string? lTruncatedTarget = TruncateMiddle(_JobVm.JobManager.Jobs[i].TargetDirectory.Replace(@"\", @"\\"), lPathTargetColumnWidth);

                    Console.WriteLine("{0,-30} {1,-" + lPathSourceColumnWidth + "} {2,-" + lPathTargetColumnWidth + "}", i + " - " + _JobVm.JobManager.Jobs[i].Name, lTruncatedSource, lTruncatedTarget);
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
                ConsoleExtention.WriteLineError("Aucun job a été trouvée");
        }

        /// <summary>
        /// Create and add a new job to the JobManager
        /// </summary>
        public void CreateJob()
        {
            ConsoleExtention.WriteTitle("Création d'un job");

            // cm - Demande a l'utilisateur de saisir les info du job  
            string lName = ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("Name")}: ", new Regex("^[a-zA-Z0-9]+$"));
            if (lName == "-1")
                return;
            string lSourceDir = ConsoleExtention.ReadFolder($"\n{Strings.ResourceManager.GetObject("SourceDir")} ");
            if (lSourceDir == "-1")
                return;
            string lTargetDir = ConsoleExtention.ReadFolder($"\n{Strings.ResourceManager.GetObject("TargetDir")} ");
            if (lTargetDir == "-1")
                return;

            Console.WriteLine($"\n{Strings.ResourceManager.GetObject("PossibleTypeBackup")}:");

            // Gget enum values
            foreach (ETypeBackup type in Enum.GetValues(typeof(ETypeBackup)))
            {
                Console.WriteLine((int)type + " - " + type);
            }

            string lInput = ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("SelectChoice")}: ");
            if (lInput == "-1")
                return;

            // Parse la valeur saisie
            ETypeBackup lBackupType = (ETypeBackup)Enum.Parse(typeof(ETypeBackup), lInput);


            if ((lInput == "0" || lInput == "1") && _JobVm.CreateBackupJob(lName, lSourceDir, lTargetDir, lBackupType))
            {
                SaveJobs();
                // Afficher confirmation
                ConsoleExtention.WriteLineSucces(Strings.ResourceManager.GetObject("JobCreated").ToString());
            }
            else
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("JobNotCreated").ToString());
            }
        }
        #endregion

        #region private
        /// <summary>
        /// Truncate the middle of a string if the string is greater than maxLenght
        /// </summary>
        /// <param name="pMessage">string to truncate</param>
        /// <param name="pMaxLength">max lenght of the message</param>
        /// <returns>truncated string</returns>
        /// <remarks>Mahmoud Charif - 05/02/2024 - Création</remarks>
        public string TruncateMiddle(string pMessage, int pMaxLength)
        {
            if (string.IsNullOrEmpty(pMessage) || pMaxLength < 5 || pMessage.Length <= pMaxLength)
                return pMessage;

            int leftCharacters = (pMaxLength / 2) - 2;
            int rightCharacters = pMaxLength - leftCharacters - 3;
            int startIndex = pMessage.Length - rightCharacters;

            return pMessage.Substring(0, leftCharacters) + "..." + pMessage.Substring(startIndex);
        }

        /// <summary>
        /// Print the range selection to run jobs
        /// </summary>
        /// <returns>Tuple of index where the first item is lower or equal than item2</returns>
        private Tuple<int, int> SelectJobs(List<CJob> pJobs)
        {
            int lStartIndex = 0;
            int lEndIndex = 0;
            Console.WriteLine("Sélectionnez la plage de jobs à exécuter");

            lStartIndex = int.Parse(ConsoleExtention.ReadResponse("Index de début : ", new Regex("^[0-" + (pJobs.Count - 1) + "]+$")));
            if (lStartIndex == -1)
                return null;

            Console.WriteLine();

            lEndIndex = int.Parse(ConsoleExtention.ReadResponse("Index de fin : ", new Regex("^[0-" + (pJobs.Count - 1) + "]+$")));
            if (lEndIndex == -1)
                return null;

            // Check indexs
            if (lStartIndex < 0 || lEndIndex > _JobVm.JobManager.Jobs.Count || lStartIndex > lEndIndex)
            {
                Console.WriteLine($"Plage d'indices invalide le nombre de job disponible est de {_JobVm.JobManager.Jobs.Count}");
                //Restart SelectJobs if the range is not correct
                return SelectJobs(pJobs);
            }

            return Tuple.Create(lStartIndex, lEndIndex);
        }

        #endregion

        #endregion

        #region Serialization
        /// <summary>
        /// Save Jobs and print
        /// </summary>
        public void SaveJobs()
        {
            _JobVm.SaveJobs();
            ConsoleExtention.WriteLineSucces($"Job {_JobVm.JobManager.Name} saved");
        }

        /// <summary>
        /// Load jobs and print
        /// </summary>
        public void LoadJobs()
        {
            ConsoleExtention.WriteTitle(Strings.ResourceManager.GetObject("LoadJobs").ToString());

            string lInput = ConsoleExtention.ReadResponse("0 - Fichier par defaut\n" +
                                                          "1 - Autre fichier\n");
            switch (lInput)
            {
                case "0":
                    _JobVm.LoadJobs();
                    break;
                case "1":
                    _JobVm.LoadJobs(false, ConsoleExtention.ReadFile("Choisir le fichier de configuration", new Regex("^.*\\.(json | JSON)$")));

                    if (_JobVm.JobManager != null)
                        ConsoleExtention.WriteLineSucces($"{_JobVm.JobManager.Name} Loaded");
                    break;
            }
        }
        #endregion

        #region Events

        private void LogGenericData_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Count >= 1)
            {
                CLogBase lLogFileState = (sender as ObservableCollection<CLogBase>).Last();

                if (!lLogFileState.IsSummary)
                {
                    ConsoleExtention.WriteTitle(lLogFileState.Name);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Date: ");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(lLogFileState.Date.Date);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Source Directory: ");

                    ConsoleExtention.WriteLinePath(lLogFileState.SourceDirectory);

                    Console.ResetColor();
                    Console.WriteLine("=>");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Target Directory: ");

                    ConsoleExtention.WriteLinePath(lLogFileState.TargetDirectory);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Total Size: ");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(lLogFileState.TotalSize);

                    Console.ResetColor();
                }
                else
                {
                    CLogState lLogState = (CLogState)(sender as ObservableCollection<CLogBase>).Last();

                    ConsoleExtention.WriteTitle(lLogState.Name, ConsoleColor.Red);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Date: ");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(lLogState.Date.Date);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Temps elapsed: ");

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(lLogState.ElapsedMilisecond + "ms");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Source Directory: ");

                    ConsoleExtention.WriteLinePath(lLogState.SourceDirectory);

                    Console.ResetColor();
                    Console.WriteLine("=>");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Target Directory: ");

                    ConsoleExtention.WriteLinePath(lLogState.TargetDirectory);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Total Size: ");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(lLogState.TotalSize);

                    Console.ResetColor();
                }

            }
        }


        private void LogStringData_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Count >= 1)
            {
                string lLog = (sender as ObservableCollection<string>).Last();
                ConsoleExtention.WriteLineWarning(DateTime.Now + " " + lLog);
            }
        }

        #endregion
    }
}
