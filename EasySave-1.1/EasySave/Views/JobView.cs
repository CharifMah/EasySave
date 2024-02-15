using LogsModels;
using Models.Backup;
using Ressources;
using Stockage.Logs;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ViewModels;
namespace EasySave.Views
{
    /// <summary>
    /// Vue en rapport avec les jobs
    /// </summary>
    public class JobView : BaseView
    {
        #region Attributes
        private JobViewModel _JobVm;
        /// <summary>
        /// Titre de la vue Job
        /// </summary>
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
        /// <summary>
        /// Lance
        /// </summary>
        public override void Run()
        {
            if (!_JobVm.JobManager.Jobs.Any())
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("NoJobCreated").ToString());
                return;
            }
            // Liste les jobs
            ListJobs();

            // Invite l'utilisateur à sélectionner les jobs
            List<CJob> lJobsToRun = SelectJobs();

            // Run les jobs si il en existe
            if (_JobVm.JobManager.Jobs.Any() && lJobsToRun != null)
            {
                List<CJob> lJobsRunning = _JobVm.RunJobs(lJobsToRun);
                foreach (CJob lJobRunning in lJobsRunning)
                    ConsoleExtention.WriteLineSucces($"Job {lJobRunning.Name} " + Strings.ResourceManager.GetObject("CopyEnd"));
                ShowSummary(CLogger<List<CLogState>>.GenericLogger.Datas.Last());
            }
        }

        /// <summary>
        /// Print all jobs
        /// </summary>
        public void ListJobs()
        {
            if (_JobVm.JobManager != null && _JobVm.JobManager.Jobs.Any())
            {
                int lConsoleWidth = Console.WindowWidth;
                int lNameColumnWidth = lConsoleWidth / 6;
                int lTypeColumnWidth = (lConsoleWidth - lNameColumnWidth) / 4;
                int lPathSourceColumnWidth = (lConsoleWidth - lNameColumnWidth - lTypeColumnWidth) / 2;
                int lPathTargetColumnWidth = lConsoleWidth - lNameColumnWidth - lTypeColumnWidth - lPathSourceColumnWidth - 3;


                // cm - Écrit le nom de la config
                ConsoleExtention.WriteTitle(_JobVm.JobManager.Name);
                Console.ForegroundColor = ConsoleColor.Yellow;
                // cm - Affiche les colonne du tableau
                Console.WriteLine("{0,-" + lNameColumnWidth + "} {1,-" + lTypeColumnWidth + "} {2,-" + lPathSourceColumnWidth + "} {3,-" + lPathTargetColumnWidth + "}",
                                 $"{Strings.ResourceManager.GetObject("Name")} : ",
                                 $"{Strings.ResourceManager.GetObject("Type")} : ",
                                 $"{Strings.ResourceManager.GetObject("SourceDir")} : ",
                                 $"{Strings.ResourceManager.GetObject("TargetDir")} : ");

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Green;
                // cm - Affiche les ligne du tableau
                for (int i = 0; i < _JobVm.JobManager.Jobs.Count; i++)
                {
                    string? lTruncatedSource = TruncateMiddle(_JobVm.JobManager.Jobs[i].SourceDirectory.Replace(@"\", @"\\"), lTypeColumnWidth);
                    string? lTruncatedTarget = TruncateMiddle(_JobVm.JobManager.Jobs[i].TargetDirectory.Replace(@"\", @"\\"), lPathTargetColumnWidth);
                    Console.WriteLine("{0,-" + lNameColumnWidth + "} {1,-" + lTypeColumnWidth + "} {2,-" + lPathSourceColumnWidth + "} {3,-" + lPathTargetColumnWidth + "}", i + " - " + _JobVm.JobManager.Jobs[i].Name, _JobVm.JobManager.Jobs[i].BackupType, lTruncatedSource, lTruncatedTarget);
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("NoJobCreated").ToString());
        }

        /// <summary>
        /// Create and add a new job to the JobManager
        /// </summary>
        public void CreateJob()
        {
            ConsoleExtention.WriteTitle(Strings.ResourceManager.GetObject("JobCreation").ToString());
            // cm - Demande a l'utilisateur de saisir les info du job  
            string lName = ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("Name")}: ", new Regex("^[a-zA-Z0-9]+$"));
            if (lName == "-1")
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("JobNotCreated").ToString());
                return;
            }
            string lSourceDir = ConsoleExtention.ReadFolder($"\n{Strings.ResourceManager.GetObject("SourceDir")} ");
            if (lSourceDir == "-1")
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("JobNotCreated").ToString());
                return;
            }
            string lTargetDir = ConsoleExtention.ReadFolder($"\n{Strings.ResourceManager.GetObject("TargetDir")} ");
            if (lTargetDir == "-1")
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("JobNotCreated").ToString());
                return;
            }
            else if (lTargetDir == lSourceDir)
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("SamePath").ToString());
                return;
            }

            if (!Directory.Exists(lSourceDir) || !Directory.Exists(lTargetDir))
            {
                ConsoleExtention.WriteLineError($"{Strings.ResourceManager.GetObject("PossibleTypeBackup")} " + " " + Strings.ResourceManager.GetObject("JobNotCreated").ToString());
                return;
            }

            Console.WriteLine($"\n{Strings.ResourceManager.GetObject("PossibleTypeBackup")}:");
            // Get enum values
            foreach (ETypeBackup type in Enum.GetValues(typeof(ETypeBackup)))
            {
                Console.WriteLine((int)type + " - " + type);
            }
            string lInput = ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("SelectChoice")}", new Regex("^[0-1]$"));
            if (lInput == "-1")
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("JobNotCreated").ToString());
                return;
            }
            // Parse la valeur saisie
            ETypeBackup lBackupType = (ETypeBackup)Enum.Parse(typeof(ETypeBackup), lInput);

            if (_JobVm.CreateBackupJob(new CJob(lName, lSourceDir, lTargetDir, lBackupType)))
            {
                SaveJobs();
                // Afficher confirmation
                ConsoleExtention.WriteLineSucces(Strings.ResourceManager.GetObject("JobCreated").ToString());
            }
            else
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("JobNotCreated").ToString());
        }

        /// <summary>
        ///  Delete a job from the JobManager
        /// </summary>
        public void DeleteJob()
        {
            ConsoleExtention.WriteTitle(Strings.ResourceManager.GetObject("DeletingJobs").ToString());
            if (!_JobVm.JobManager.Jobs.Any())
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("NoJobCreated").ToString());
                return;
            }
            // Listez les jobs
            ListJobs();

            //Invite l'utilisateur à sélectionner les jobs
            List<CJob> lJobsToDelete = SelectJobs();

            // Appelez la méthode DeleteJobs si il existe des jobs à supprimer            
            if (_JobVm.JobManager.Jobs.Any() && lJobsToDelete != null)
            {
                if (_JobVm.DeleteJobs(lJobsToDelete))
                {
                    SaveJobs();
                    ConsoleExtention.WriteLineSucces(Strings.ResourceManager.GetObject("JobsDeleted").ToString());
                }
                else
                {
                    ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("JobDeleteError").ToString());
                }

            }
        }

        /// <summary>
        /// Save Jobs and print
        /// </summary>
        public void SaveJobs()
        {
            _JobVm.SaveJobs();
            ConsoleExtention.WriteLineSucces($"Job {_JobVm.JobManager.Name} " + Strings.ResourceManager.GetObject("Saved"));
        }

        /// <summary>
        /// Load jobs and print
        /// </summary>
        public void LoadJobs()
        {
            ConsoleExtention.WriteTitle(Strings.ResourceManager.GetObject("LoadJobs").ToString());
            string lInput = ConsoleExtention.ReadResponse($"\n0 - {Strings.ResourceManager.GetObject("DefaultFile")}\n" +
                                                          $"1 - {Strings.ResourceManager.GetObject("OtherFile")}\n\n" +
                                                          Strings.ResourceManager.GetObject("SelectChoice").ToString(), new Regex("^[0-1]$"));
            if (lInput == "-1")
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("Error").ToString());
                return;
            }

            switch (lInput)
            {
                case "0":
                    _JobVm.LoadJobs();
                    break;
                case "1":
                    _JobVm.LoadJobs(false, ConsoleExtention.ReadFile($"\n{Strings.ResourceManager.GetObject("SelectConfigurationFile")}", new Regex("^.*\\.(json | JSON)$"), Path.GetDirectoryName(Models.CSettings.Instance.JobConfigFolderPath)));
                    if (_JobVm.JobManager.Jobs.Count > 0)
                        ConsoleExtention.WriteLineSucces($"{_JobVm.JobManager.Name} " + Strings.ResourceManager.GetObject("Loaded").ToString());
                    else
                        ConsoleExtention.WriteLineError($"{_JobVm.JobManager.Name} " + Strings.ResourceManager.GetObject("WithoutJobLoaded").ToString());
                    break;
            }
        }

        /// <summary>
        ///  Invite l'utilisateur à sélectionner des jobs à partir
        ///  d'une saisie spécifique et retourne une liste des jobs sélectionnés.
        /// </summary>
        /// <returns>List<CJob> ou null</returns>
        private List<CJob> SelectJobs()
        {
            // Instructions pour l'utilisateur sur le format de saisie
            Console.WriteLine(
            $"\n0 - {Strings.ResourceManager.GetObject("InputFormat")} \n" +
            $"1 - {Strings.ResourceManager.GetObject("SingleIndex")}\n" +
            $"2 - {Strings.ResourceManager.GetObject("MultipleIndexes")}\n" +
            $"3 - {Strings.ResourceManager.GetObject("RangeIndexes")}\n" +
            $"4 - {Strings.ResourceManager.GetObject("CombineIndexes")}\n");

            // Demande à l'utilisateur de saisir les indices des jobs à supprimer
            string pattern = @"^(\d+(-\d+)?)(,\d+(-\d+)?)*$";
            Func<string, bool> pValidator = lInput => CheckSelectJobs(lInput);
            string lInput = ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("SelectChoice")} ", new Regex(pattern), pValidator);

            if (lInput == "-1")
                return null; // L'utilisateur a choisi de sortir

            // Demande la confirmation avant de procéder
            string lConfirmation = ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("Confirm")} ", new Regex("^[YyNn]$"));
            if (lConfirmation.ToLower() == "n" || lConfirmation == "-1")
                return null; // L'utilisateur a annulé la suppression



            // Récupére et retourne la liste des jobs basée sur l'entrée de l'utilisateur
            List<CJob> lSelectedJobs = SelectJobsFromInput(lInput);

            return lSelectedJobs.Count > 0 ? lSelectedJobs : null;
        }

        /// <summary>
        /// Gère et vérifie les indices entrées par l'utilisateur
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Collection unique indice ou -1 si erreur</returns>
        private HashSet<int> GetIndicesFromInput(string input)
        {
            HashSet<int> indices = new HashSet<int>();
            string[] parts = input.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            int jobCount = _JobVm.JobManager.Jobs.Count;

            foreach (string part in parts)
            {
                try
                {
                    if (part.Contains('-'))
                    {
                        string[] rangeParts = part.Split('-');
                        int start = int.Parse(rangeParts[0]);
                        int end = int.Parse(rangeParts[1]);
                        if (start > end || start < 0 || end >= jobCount)
                        {
                            // Retourne une erreur si l'intervalle est invalide
                            return new HashSet<int> { -1 };
                        }
                        for (int i = start; i <= end; i++)
                        {
                            indices.Add(i);
                        }
                    }
                    else
                    {
                        int index = int.Parse(part);
                        if (index < 0 || index >= jobCount)
                        {
                            // Retourne une erreur si l'indice est invalide
                            return new HashSet<int> { -1 };
                        }
                        indices.Add(index);
                    }
                }
                catch
                {
                    // Retourne une erreur si le parse échoue
                    return new HashSet<int> { -1 };
                }
            }
            return indices;
        }

        /// <summary>
        /// Gère un état booléen indiquant le résultat de la vérification des indices fournis en entrée
        /// </summary>
        /// <param name="pInput"></param>
        private bool CheckSelectJobs(string pInput)
        {
            HashSet<int> indices = GetIndicesFromInput(pInput);

            if (indices.Contains(-1))
            {
                ConsoleExtention.WriteLineError($"\n{Strings.ResourceManager.GetObject("InvalidIndice")}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sélectionne et retourne une liste de jobs basée sur les indices spécifiés dans l'entrée utilisateur
        /// </summary>
        /// <param name="pInput"></param>
        /// <returns>List<CJob></returns>
        private List<CJob> SelectJobsFromInput(string pInput)
        {
            HashSet<int> indices = GetIndicesFromInput(pInput);
            int jobCount = _JobVm.JobManager.Jobs.Count;

            List<CJob> selectedJobs = new List<CJob>();

            foreach (int index in indices)
            {
                if (index >= 0 && index < jobCount)
                {
                    selectedJobs.Add(_JobVm.JobManager.Jobs[index]);
                }
            }

            return selectedJobs;
        }

        #endregion

        #region private

        /// <summary>
        /// Truncate the middle of a string if the string is greater than maxLenght
        /// </summary>
        /// <param name="pMessage">string to truncate</param>
        /// <param name="pMaxLength">max length of the message</param>
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
        private List<CJob> SelectJobs(List<CJob> pJobs)
        {
            List<CJob> lSelectedJobs = new List<CJob>();
            int lStartIndex = 0;
            int lEndIndex = 0;
            List<int> lListIndex = new List<int>();
            Console.WriteLine($"\n{Strings.ResourceManager.GetObject("SelectRangeJobs")}");
            lStartIndex = int.Parse(ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("StartIndex")}", new Regex("^[0-" + (pJobs.Count - 1) + "]+$")));
            if (lStartIndex == -1)
                return null;
            Console.WriteLine();
            lEndIndex = int.Parse(ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("SelectRangeJobs")}", new Regex("^[0-" + (pJobs.Count - 1) + "]+$")));
            if (lEndIndex == -1)
                return null;
            if (lEndIndex < pJobs.Count - 1)
            {
                string lIndividualIndex = ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("AdditionalJobs")}", new Regex("^[YyNn]$"));
                if (lIndividualIndex == "-1")
                    return null;
                if (lIndividualIndex.ToLower() == "y")
                {
                    string lResponse = String.Empty;
                    do
                    {
                        lResponse = ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("IndividualIndex")}", new Regex("^((0-" + (pJobs.Count - 1) + ")|[^" + lStartIndex + "-" + lEndIndex + "])$"));
                        if (lResponse == "-1")
                            return null;
                        if (lResponse != "q")
                            lListIndex.Add(int.Parse(lResponse));
                    } while (lResponse != "q");
                }
            }
            foreach (int lIndex in lListIndex)
            {
                lSelectedJobs.Add(pJobs[lIndex]);
            }
            for (int i = lStartIndex; i <= lEndIndex; i++)
            {
                lSelectedJobs.Add(pJobs[i]);
            }
            // Check indexes
            if (lStartIndex > lEndIndex)
            {
                Console.WriteLine($"{Strings.ResourceManager.GetObject("InvalidIndexRange")}" + $"{_JobVm.JobManager.Jobs.Count}");
                //Restart SelectJobs if the range is not correct
                return SelectJobs(pJobs);
            }
            return lSelectedJobs;
        }

        private void ShowSummary(List<CLogState> pLogStates)
        {
            foreach (CLogState lLog in pLogStates)
            {
                ConsoleExtention.WriteTitle(lLog.Name, ConsoleColor.Red);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Date: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(lLog.Date);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n{Strings.ResourceManager.GetObject("TimeElapsed")}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(lLog.ElapsedMilisecond + " ms");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n{Strings.ResourceManager.GetObject("SourceDirectory")}");
                ConsoleExtention.WritePath(lLog.SourceDirectory);
                Console.ResetColor();
                Console.WriteLine("=>");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n{Strings.ResourceManager.GetObject("TargetDirectory")}");
                ConsoleExtention.WritePath(lLog.TargetDirectory);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n{Strings.ResourceManager.GetObject("TotalSize")}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(lLog.TotalSize + $" : {Strings.ResourceManager.GetObject("bytes")}");
                Console.ResetColor();
            }
        }

        #region Events
        private void LogGenericData_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Count >= 1)
            {
                CLogBase lLogFileState = (sender as ObservableCollection<CLogBase>).Last();
                ConsoleExtention.WriteTitle(lLogFileState.Name);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n{Strings.ResourceManager.GetObject("SourceDirectory")}");
                ConsoleExtention.WritePath(lLogFileState.SourceDirectory);
                Console.ResetColor();
                Console.WriteLine("=>");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n{Strings.ResourceManager.GetObject("TargetDirectory")}");
                ConsoleExtention.WritePath(lLogFileState.TargetDirectory);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n{Strings.ResourceManager.GetObject("TotalSize")}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(lLogFileState.TotalSize + $" : {Strings.ResourceManager.GetObject("bytes")}");
                Console.ResetColor();
            }
        }
        private void LogStringData_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Count >= 1)
            {
                string lLog = (sender as ObservableCollection<string>).Last();
                ConsoleExtention.WriteLineWarning(System.DateTime.Now + " " + lLog);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
