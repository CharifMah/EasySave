using EasySave.ViewModels;
using EasySaveDraft.Resources;

namespace EasySave.Views
{
    public class LangueView : BaseView
    {
        private LangueViewModel _LangueVm;

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

            string? lInput = ConsoleExtention.ReadResponse(Strings.ResourceManager.GetObject("SelectChoice").ToString());
            // cm - if the input is correct printe 
            if (_LangueVm.SetLanguage(lInput))
                ConsoleExtention.WriteLineSucces(Strings.ResourceManager.GetObject("SelectedLanguage").ToString());
            else
                ConsoleExtention.WriteLineError(Strings.ResourceManager.GetObject("InvalidLangSelection").ToString());
        }

        /// <summary>
        /// Liste les langue disponibles
        /// </summary>
        public void ListLanguage()
        {
            ConsoleExtention.WriteTitle(Strings.ResourceManager.GetObject("Lang").ToString());
            Console.WriteLine("Saisir 1 pour la langue française");
            Console.WriteLine("Select 2 for English");
        }
    }
}
