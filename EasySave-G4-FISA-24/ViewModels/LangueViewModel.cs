using Models;

namespace ViewModels
{
    public class LangueViewModel : BaseViewModel
    {
        private CLangue _Langue;
        public CLangue Langue { get => _Langue; set => _Langue = value; }
        // Constructor
        public LangueViewModel()
        {
            _Langue = CSettings.Instance.Langue;
            SetLanguage(_Langue.SelectedCulture);
        }
        /// <summary>
        /// Set the current language
        /// </summary>
        /// <param name="pCultureInfo">give a number</param>
        /// <returns>true if the language was changed</returns>
        public bool SetLanguage(string pCultureInfo)
        {
            bool result = _Langue.SetLanguage(pCultureInfo);
            Models.CSettings.Instance.SaveSettings();
            return result;
        }
    }
}
