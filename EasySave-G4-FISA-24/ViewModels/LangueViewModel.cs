using Models;

namespace ViewModels
{
    /// <summary>
    /// Classe View Model de la langue
    /// </summary>
    public class LangueViewModel : BaseViewModel
    {
        private CLangue _Langue;
        /// <summary>
        /// Classe model de la langue
        /// </summary>
        public CLangue Langue { get => _Langue; set => _Langue = value; }
        /// <summary>
        /// Constructeur de la LangueViewModel
        /// </summary>
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
