using EasySave.ViewModels;
using EasySaveDraft.Resources;
using Models;
using System.Text.RegularExpressions;

namespace EasySave.Views
{
    public class LangueView : BaseView
    {
        private LangueViewModel _LangueVm;

        public override string Title => Strings.ResourceManager.GetObject("Lang").ToString();

        public LangueView(LangueViewModel pJobVm)
        {
            _LangueVm = pJobVm;
        }

        /// <summary>
        /// Lance la selection du language
        /// </summary>
        public override void Run()
        {
            ListLanguage();

            string? lInput = ConsoleExtention.ReadResponse(Strings.ResourceManager.GetObject("SelectChoice").ToString(), new Regex("^[0-1]$"));
            // cm - if the input is correct printe 
            if (_LangueVm.SetLanguage(lInput))
                ConsoleExtention.WriteLineSucces(Strings.ResourceManager.GetObject("SelectedLanguage").ToString());
        }

        /// <summary>
        /// Liste les langue disponibles
        /// </summary>
        public void ListLanguage()
        {
            ConsoleExtention.WriteTitle(Title);
            foreach (KeyValuePair<int,string> lLangue in _LangueVm.Langue.Languages)
            {
                Console.WriteLine($"{lLangue.Key} - {lLangue.Value} ");
            }
            Console.WriteLine();
        }
    }
}
