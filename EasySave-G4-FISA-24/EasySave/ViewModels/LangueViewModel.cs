using Models;

namespace EasySave.ViewModels
{
    public class LangueViewModel : BaseViewModel
    {
        private CLangue _Langue;
        public CLangue Langue { get => _Langue; set => _Langue = value; }
        // Constructor
        public LangueViewModel()
        {
            _Langue = Settings.Instance.Langue;
        }
        /// <summary>
        /// Set the current language
        /// </summary>
        /// <param name="pLanguageChoice">give a number</param>
        /// <returns>true if the language was changed</returns>
        public bool SetLanguage(string pCultureInfo)
        {
            bool result = _Langue.SetLanguage(pCultureInfo);
            Settings.Instance.SaveSettings();
            return result;
        }
    }
}
