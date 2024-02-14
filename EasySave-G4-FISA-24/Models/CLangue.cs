using System.Globalization;
using System.Runtime.Serialization;

namespace Models
{
    /// <summary>
    /// Classe de la langue de l'application
    /// </summary>
    [DataContract]
    public class CLangue
    {
        private Dictionary<int, string> _Languages;
        [DataMember]
        private string _SelectedCulture;
        /// <summary>
        /// Dictionnaire de langues disponnible dans l'application
        /// </summary>
        public Dictionary<int, string> Languages { get => _Languages; set => _Languages = value; }
        public string SelectedCulture { get => _SelectedCulture; set => _SelectedCulture = value; }

        /// <summary>
        /// Constructeur de la classe Clangue Init the language with the installed culture of the operating system
        /// </summary>
        public CLangue()
        {
            _Languages = new Dictionary<int, string>()
            {
              {1, "fr"},
              {2, "en-US"}
            };
            _SelectedCulture = CultureInfo.InstalledUICulture.ToString();
        }

        /// <summary>
        /// Set the current UI culture
        /// </summary>
        /// <param name="pCultureInfo">give a number</param>
        /// <returns>true if the language was changed</returns>
        public bool SetLanguage(string pCultureInfo)
        {
            bool result = false;
            _SelectedCulture = pCultureInfo;
            CultureInfo lCultureInfo = CultureInfo.GetCultureInfo(pCultureInfo);
            if (Thread.CurrentThread.CurrentUICulture != lCultureInfo)
            {
                Thread.CurrentThread.CurrentUICulture = lCultureInfo;
                result = true;
            }
            return result;
        }
    }
}
