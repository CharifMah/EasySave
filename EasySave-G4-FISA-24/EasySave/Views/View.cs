using EasySave.ViewModels;
using EasySaveDraft.Resources;

namespace EasySave.Views
{
    public class View : BaseView
    {
        private MainViewModel _MainVm;
        private LangueView _LangView;
        private JobView _JobView;
        private string _Menu;
        #region CTOR

        public View()
        {
            _MainVm = new MainViewModel();
            _LangView = new LangueView(_MainVm.LangueVm);
            _JobView = new JobView(_MainVm.JobVm);
            _Menu = $"\n0 - {Strings.ResourceManager.GetObject("ChooseLang")} \n" +
                    $"1 - {Strings.ResourceManager.GetObject("ListJobs")}\n" +
                    $"2 - {Strings.ResourceManager.GetObject("LoadJobs")}\n" +
                    $"3 - {Strings.ResourceManager.GetObject("CreateJob")}\n" +
                    $"4 - {Strings.ResourceManager.GetObject("RunJobs")}\n";

            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        #endregion

        #region METHODES

        /// <summary>
        /// Start the main program
        /// </summary>
        public override void Run()
        {
            string lInput = string.Empty;
            while (true)
            {
                lInput = ShowMenu();
                switch (lInput)
                {
                    case "-1": // cm - Restart the program if the user press CTRL+C
                        Run();
                        break;
                    case "0":
                        _LangView.Run();
                        break;
                    case "1":
                        _JobView.ListJobs();
                        break;
                    case "2":
                        _JobView.LoadJobs();
                        break;
                    case "3":
                        _JobView.CreateJob();
                        break;
                    case "4":
                        _JobView.Run();
                        break;
                }
            }
        }

        #endregion

        public string ShowMenu()
        {
            ConsoleExtention.WriteTitle("Menu");

            string lInput = ConsoleExtention.ReadResponse(_Menu + $"\n{Strings.ResourceManager.GetObject("SelectChoice")} ");

            return lInput;
        }

        private void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            ConsoleExtention.Clear();
            Run();
            e.Cancel = true;
        }
    }
}
