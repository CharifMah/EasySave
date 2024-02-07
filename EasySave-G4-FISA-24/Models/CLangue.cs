using System.Globalization;

namespace Models
{
    /// <summary>
    /// Classe langue
    /// </summary>
    public class CLangue
    {


        /// <summary>
        /// Constructeur de la classe Clangue Init the language with the installed culture of the operating system
        /// </summary>
        public CLangue()
        {
            // cm - set le culture a la culture de l'OS
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
        }

        /// <summary>
        /// Set the language
        /// </summary>
        /// <param name="pLanguageChoice">give a number</param>
        public bool SetLanguage(string pLanguageChoice)
        {
            bool lIsLangChanged = true;

            switch (pLanguageChoice)
            {
                case "1":
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
                    break;
                case "2":
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    break;
                default:
                    lIsLangChanged = false;
                    break;
            }

            return lIsLangChanged;
        }
    }
}
