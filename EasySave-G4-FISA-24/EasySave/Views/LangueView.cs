using Ressources;
using System.Text.RegularExpressions;
using ViewModels;
namespace EasySave.Views
{
    /// <summary>
    /// Vue des langues
    /// </summary>
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
            string? lInput = ConsoleExtention.ReadResponse(Strings.ResourceManager.GetObject("SelectChoice").ToString(), new Regex("^[" + _LangueVm.Langue.Languages.First().Key + "-" + _LangueVm.Langue.Languages.Last().Key + "]$"));
            if (lInput == "-1")
            {
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("SelectedLanguage").ToString());
                return;
            }

            int lLangue = int.Parse(lInput);

            // cm - if the input is correct printe 
            if (_LangueVm.SetLanguage(_LangueVm.Langue.Languages[lLangue]))
                ConsoleExtention.WriteLineSucces(Strings.ResourceManager.GetObject("SelectedLanguage").ToString());
        }
        /// <summary>
        /// Liste les langue disponibles
        /// </summary>
        public void ListLanguage()
        {
            ConsoleExtention.WriteTitle(Title);
            foreach (KeyValuePair<int, string> lLangue in _LangueVm.Langue.Languages)
            {
                Console.WriteLine($"{lLangue.Key} - {lLangue.Value} ");
            }
            Console.WriteLine();
        }
    }
}
