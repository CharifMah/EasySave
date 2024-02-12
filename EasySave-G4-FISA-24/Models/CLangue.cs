using System.Globalization;

namespace Models
{
    /// <summary>
    /// Classe langue
    /// </summary>
    public class CLangue
    {
        private Dictionary<int, string> _Languages;

        public Dictionary<int, string> Languages { get => _Languages; set => _Languages = value; }

        /// <summary>
        /// Constructeur de la classe Clangue Init the language with the installed culture of the operating system
        /// </summary>
        public CLangue()
        {
            _Languages = new Dictionary<int, string>()
            {
              {1, "Français"},
              {2, "English"}
            };

            // cm - set le culture a la culture de l'OS
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
        }

        /// <summary>
        /// Set the language
        /// </summary>
        /// <param name="pLanguageChoice">give a number</param>
        /// <returns>true if the language was changed</returns>
        public bool SetLanguage(string pLanguageChoice)
        {
            bool lIsLangChanged = true;

            switch (pLanguageChoice)
            {
                case "1":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
                    break;
                case "2":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    break;
                default:
                    lIsLangChanged = false;
                    break;
            }

            return lIsLangChanged;
        }
    }
}
