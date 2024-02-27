using Models.Langue;
using Models.Settings;

namespace EasySaveGUI.ViewModels
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
        public CLangue Langue
        {
            get => _Langue;
            set { _Langue = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// Constructeur de la LangueViewModel
        /// </summary>
        public LangueViewModel()
        {
            _Langue = CSettings.Instance.Langue;
            SetLanguage(_Langue.SelectedCulture.Value);
            NotifyPropertyChanged("Langue");
        }
        /// <summary>
        /// Set the current language
        /// </summary>
        /// <param name="pCultureInfo">give a number</param>
        /// <returns>true if the language was changed</returns>
        public bool SetLanguage(string pCultureInfo)
        {
            bool result = _Langue.SetLanguage(pCultureInfo);
            CSettings.Instance.SaveSettings();
            NotifyPropertyChanged("Langue");
            return result;
        }
    }
}
