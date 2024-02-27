using System.Globalization;
using System.Runtime.Serialization;

namespace Models.Langue
{
    /// <summary>
    /// Classe de la langue de l'application
    /// </summary>
    [DataContract]
    public class CLangue
    {
        private Dictionary<int, string> _Languages;
        [DataMember]
        private KeyValuePair<int, string> _SelectedCulture;
        /// <summary>
        /// Dictionnaire de langues disponible dans l'application
        /// </summary>
        public Dictionary<int, string> Languages { get => _Languages; set => _Languages = value; }
        public KeyValuePair<int, string> SelectedCulture { get => _SelectedCulture; set => _SelectedCulture = value; }

        /// <summary>
        /// Initialize the language with the installed culture of the operating system
        /// </summary>
        public CLangue()
        {
            _Languages = new Dictionary<int, string>()
            {
              {0, "fr"},
              {1, "en-US"}
            };
            _SelectedCulture = _Languages.First(l => l.Value.Contains(CultureInfo.InstalledUICulture.ToString()[0..2]));
        }

        /// <summary>
        /// Set the current UI culture
        /// </summary>
        /// <param name="pCultureInfo">give a number</param>
        /// <returns>true if the language was changed</returns>
        public bool SetLanguage(string pCultureInfo)
        {
            bool result = false;
            _SelectedCulture = _Languages.First(l => l.Value.Contains(pCultureInfo));
            CultureInfo lCultureInfo = CultureInfo.GetCultureInfo(_SelectedCulture.Value);
            if (Thread.CurrentThread.CurrentUICulture != lCultureInfo)
            {
                Thread.CurrentThread.CurrentUICulture = lCultureInfo;
                result = true;
            }
            return result;
        }
    }
}
