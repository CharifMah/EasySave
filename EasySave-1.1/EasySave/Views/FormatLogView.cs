using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Models;
using Ressources;
using ViewModels;


namespace EasySave.Views
{
    public class FormatLogView : BaseView
    {
        private FormatLogViewModel _FormatLogVm;

        /// <summary>
        /// Constructeur de la Vue pour le format des logs
        /// </summary>
        /// <param name="pJobVm">Le JobViewModel</param>
        public FormatLogView(FormatLogViewModel pJobVm)
        {
            _FormatLogVm = pJobVm;
        }

        public override string Title => Strings.ResourceManager.GetObject("FormatLogs").ToString();

        /// <summary>
        /// Lance la selection du format des logs
        /// </summary>
        public override void Run()
        {
            ListFormatsLogs();

            Console.WriteLine(Strings.ResourceManager.GetObject("CurrentFormat").ToString() + $" {_FormatLogVm.FormatLog.SelectedLogFormat}\n");

            string? lInput = ConsoleExtention.ReadResponse(Strings.ResourceManager.GetObject("SelectChoice").ToString(), new Regex("^[" + _FormatLogVm.FormatLog.FormatsLogs.First().Key + "-" + _FormatLogVm.FormatLog.FormatsLogs.Last().Key + "]$"));
            if (lInput == "-1")
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("SelectedFormatLogs").ToString() + lInput);
                return;
            }

            int lFormatLog = int.Parse(lInput);

            // cm - if the input is correct print 
            if (_FormatLogVm.SetFormatLog(_FormatLogVm.FormatLog.FormatsLogs[lFormatLog]))
                ConsoleExtention.WriteLineSucces(Strings.ResourceManager.GetObject("SelectedFormatLogs").ToString() + $" {_FormatLogVm.FormatLog.SelectedLogFormat}");
        }

        /// <summary>
        /// Liste les formats de logs disponibles
        /// </summary>
        public void ListFormatsLogs()
        {
            ConsoleExtention.WriteTitle(Title);
            foreach (KeyValuePair<int, string> lFormatLog in _FormatLogVm.FormatLog.FormatsLogs)
            {
                Console.WriteLine($"{lFormatLog.Key} - {lFormatLog.Value} ");
            }
            Console.WriteLine();
        }
    }
}
