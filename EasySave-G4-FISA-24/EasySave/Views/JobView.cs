﻿using EasySave.ViewModels;
using EasySaveDraft.Resources;
using Models.Backup;

namespace EasySave.Views
{
    internal class JobView : BaseView
    {
        #region Attributes
        private JobViewModel _JobVm;
        #endregion

        #region CTOR
        public JobView(JobViewModel pJobVm)
        {
            _JobVm = pJobVm;
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
                Tuple<int, int> lRange = SelectJobs();
                if (lRange == null)
                    return;

                List<CJob> lJobsRunning = _JobVm.RunJobs(lRange);

                foreach (CJob lJobRunning in lJobsRunning)
                    ConsoleExtention.WriteLineSucces($"Job {lJobRunning.Name} is running");
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
                    string? lTruncatedSource = TruncateMiddle(_JobVm.JobManager.Jobs[i].SourceDirectory, lPathSourceColumnWidth);
                    string? lTruncatedTarget = TruncateMiddle(_JobVm.JobManager.Jobs[i].TargetDirectory, lPathTargetColumnWidth);

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
            string lName = ConsoleExtention.ReadResponse($"\n{Strings.ResourceManager.GetObject("Name")}: ");
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


            if (_JobVm.CreateBackupJob(lName, lSourceDir, lTargetDir, lBackupType))
            {
                // Afficher confirmation
                ConsoleExtention.WriteLineSucces(Strings.ResourceManager.GetObject("JobCreated").ToString());
            }
            else
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("JobNotCreated").ToString());
            }
            SaveJobs();
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
        private Tuple<int, int> SelectJobs()
        {
            int lStartIndex = 0;
            int lEndIndex = 0;
            Console.WriteLine("Sélectionnez la plage de jobs à exécuter");

            lStartIndex = int.Parse(ConsoleExtention.ReadResponse("Index de début : "));
            if (lStartIndex == -1)
                return null;

            Console.WriteLine();

            lEndIndex = int.Parse(ConsoleExtention.ReadResponse("Index de fin : "));
            if (lEndIndex == -1)
                return null;

            // Check indexs
            if (lStartIndex < 0 || lEndIndex > _JobVm.JobManager.Jobs.Count || lStartIndex > lEndIndex)
            {
                Console.WriteLine($"Plage d'indices invalide le nombre de job disponible est de {_JobVm.JobManager.Jobs.Count}");
                //Restart SelectJobs if the range is not correct
                return SelectJobs();
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
                    _JobVm.LoadJobs(true, ConsoleExtention.ReadFile("Choisir le fichier de configuration"));

                    if (_JobVm.JobManager != null)
                        ConsoleExtention.WriteLineSucces($"{_JobVm.JobManager.Name} Loaded");
                    break;
            }
        }
        #endregion
    }
}