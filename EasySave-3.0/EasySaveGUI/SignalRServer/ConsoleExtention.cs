using System.Text.RegularExpressions;
namespace SignalRServer
{
    /// <summary>
    /// Console extension class adds additional display functionality
    /// </summary>
    public static class ConsoleExtention
    {
        private static string _Input = string.Empty;
        /// <summary>
        /// Write line a error in red
        /// </summary>
        /// <param name="pMessage">message to write</param>
        public static void WriteLineError(string pMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine('\n' + pMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// Write line a success in green
        /// </summary>
        /// <param name="pMessage">message to write</param>
        public static void WriteLineSucces(string pMessage)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine('\n' + pMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// WriteLine the message Warning in DarkYellow
        /// </summary>
        /// <param name="pMessage">message to write</param>
        public static void WriteLineWarning(string pMessage)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine('\n' + pMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// Write Path with UNC Format in yellow
        /// </summary>
        /// <param name="pPath">path to write</param>
        public static void WritePath(string pPath)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(pPath.Replace(@"\", @"\\") + '\n');
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Write a personalized Title with separator
        /// </summary>
        /// <param name="pTitle">Title to write</param>
        public static void WriteTitle(string pTitle, ConsoleColor pColor = ConsoleColor.White)
        {
            int consoleWidth = Console.WindowWidth;
            // cm - Create a separator with dynamic width
            string lSeparator = new string('-', consoleWidth);
            // cm - Title
            string lTitle = pTitle;
            // Centrer le titre
            string lTitleFormatted = lTitle.PadLeft((consoleWidth + lTitle.Length) / 2).PadRight(consoleWidth);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(lSeparator);
            Console.ForegroundColor = pColor;
            Console.WriteLine(lTitleFormatted);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(lSeparator);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static string GetDate()
        {
            var lNow = DateTime.Now;

            return $"{lNow:hh}h:{lNow:mm}m:{lNow:ss}s";
        }
        /// <summary>
        /// WriteSubTitle
        /// </summary>
        /// <param name="pSubtitle">subtitle</param>
        /// <param name="pColor">couleur du subtitle</param>
        public static void WriteSubtitle(string pSubtitle, ConsoleColor pColor = ConsoleColor.DarkGray)
        {
            int lWidth = Console.WindowWidth;
            // Séparation
            string lSeparator = new string('=', lWidth);
            // Centrer le sous-titre 
            string lFormattedSubtitle =
              pSubtitle.PadLeft((lWidth + pSubtitle.Length) / 2)
                     .PadRight(lWidth);
            Console.ForegroundColor = pColor;
            Console.WriteLine(lFormattedSubtitle);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(lSeparator);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Clear the console and set the input to -1
        /// </summary>
        public static void Clear()
        {
            Console.Clear();
            _Input = "-1";
        }
    }
}