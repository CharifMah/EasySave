using EasySaveDraft.Resources;
using Gtk;
using System.Text.RegularExpressions;

namespace EasySave.Views
{
    public static class ConsoleExtention
    {
        private static string _Input = string.Empty;

        public static void WriteLineError(string pMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine('\n' + pMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteLineSucces(string pMessage)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine('\n' + pMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteLineWarning(string pMessage)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine('\n' + pMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteLinePath(string pPath)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(pPath.Replace(@"\", @"\\") + '\n');
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteLineSelected(string pInput)
        {
            Console.WriteLine($"\n{Strings.ResourceManager.GetObject("YouSelected")} " + pInput + '\n');
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

        /// <summary>
        /// Read user input
        /// </summary>
        /// <param name="pMessage">Message to loop through if the user makes an input error</param>
        /// <returns>user input</returns>
        /// <remarks>Mahmoud Charif - 05/02/2024 - Création</remarks>
        public static string ReadResponse(string pMessage, Regex? pRegex = null)
        {
            ConsoleKeyInfo lsInput = default;
            _Input = string.Empty;
            // cm - Allow Control detection
            Console.TreatControlCAsInput = true;

            if (pRegex == null)
                pRegex = new Regex("");

            do
            {
                if (string.IsNullOrEmpty(_Input) && (lsInput.Key != ConsoleKey.V && lsInput.Modifiers != ConsoleModifiers.Control))
                    Console.Write(pMessage);

                // cm - CTRL+V for past
                if ((lsInput.Key == ConsoleKey.V && lsInput.Modifiers == ConsoleModifiers.Control))
                {
                    _Input += TextCopy.ClipboardService.GetText();
                    Console.Write(_Input);
                }
          
                while (Console.KeyAvailable)
                    Console.ReadKey(false); // cm - skips previous inputs

                lsInput = Console.ReadKey(); // cm - wait to read a new character

                // cm - If the user enters CTRL+C cancel the loop and clear the console
                if ((lsInput.Modifiers & ConsoleModifiers.Control) != 0 && lsInput.Key == ConsoleKey.C)
                {
                    Clear();
                    break;
                }

                // cm - Concatenate inputs to obtain the final output
                if (lsInput.Key != ConsoleKey.Enter && lsInput.Key != ConsoleKey.Backspace && lsInput.Modifiers == 0)
                    _Input += lsInput.KeyChar;

                if (lsInput.Key == ConsoleKey.Backspace)   // cm - If user press Backspace delete 1 caratere in the console
                {
                    RemoveLastChar();
                    if (_Input.Length > 0)
                        _Input = _Input[0..^1].ToString();
                }
                else if (lsInput.Key == ConsoleKey.Delete)  // cm - If user press Delete delete 1 caratere in the console
                {
                    RemoveLastChar();
                    if (_Input.Length > 0)
                        _Input = _Input[0..^1];
                }

            } while (lsInput.KeyChar != (char)ConsoleKey.Enter); // cm - Until the user presses the Enter key, the console waits to read a new character.

            // cm - Don't show Succes message if the user cancel the action
            if (_Input != "-1" || String.IsNullOrEmpty(_Input))
                WriteLineSelected(_Input);

            if (lsInput.KeyChar == (char)ConsoleKey.Enter && !pRegex.IsMatch(_Input))
            {
                WriteLineError(Strings.ResourceManager.GetObject("InvalideSelection").ToString());
                ReadResponse(pMessage, pRegex);
            }

            return _Input;
        }

        public static void Clear()
        {
            Console.Clear();
            _Input = "-1";
        }

        private static void RemoveLastChar()
        {
            Console.SetCursorPosition(Console.GetCursorPosition().Left + 1, Console.GetCursorPosition().Top);
            Console.Write("\b \b");
        }

        public static string ReadFolder(string pDescription)
        {
            string lSelectedFolder = null;

            try
            {
                Console.WriteLine(pDescription);

                try
                {
                    Application.Init();
                }
                catch (Exception)
                {
                    return ReadFolderConsole();
                }

                var lDialog = new FileChooserDialog(
                    title: pDescription,
                    parent: null,
                    action: FileChooserAction.SelectFolder);

                lDialog.AddButton(Strings.ResourceManager.GetObject("Cancel").ToString(), ResponseType.Cancel);
                lDialog.AddButton(Strings.ResourceManager.GetObject("Select").ToString(), ResponseType.Ok);

                lDialog.SetCurrentFolder(Directory.GetCurrentDirectory());

                if (lDialog.Run() == (int)ResponseType.Ok)
                {
                    lSelectedFolder = lDialog.Filename;
                }
                else
                    lSelectedFolder = "-1";

                lDialog.Destroy();

                WriteLineSelected(lSelectedFolder);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return lSelectedFolder;
        }

        public static string ReadFile(string pDescription)
        {
            string lSelectedFile = null;

            try
            {
                Console.WriteLine(pDescription);

                try
                {
                    Application.Init();

                }
                catch (Exception)
                {
                    return ReadFileConsole();
                }

                var lDialog = new FileChooserDialog(
                    title: pDescription,
                    parent: null,
                    action: FileChooserAction.Open);

                lDialog.AddButton(Strings.ResourceManager.GetObject("Cancel").ToString(), ResponseType.Cancel);
                lDialog.AddButton(Strings.ResourceManager.GetObject("Open").ToString(), ResponseType.Ok);

                lDialog.SetCurrentFolder(Directory.GetCurrentDirectory());

                if (lDialog.Run() == (int)ResponseType.Ok)
                {
                    lSelectedFile = lDialog.Filename;
                }
                else
                    lSelectedFile = "-1";

                lDialog.Destroy();

                WriteLineSelected(lSelectedFile);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return lSelectedFile;
        }

        private static string ReadFileConsole()
        {
            string lFilePath = String.Empty;

            do
            {
                lFilePath = ReadResponse("Enter file path: ");
            } while (!File.Exists(lFilePath));

            return lFilePath;
        }

        private static string ReadFolderConsole()
        {

            string lFolderPath = String.Empty;

            do
            {
                lFolderPath = ReadResponse("Enter folder path: ");
            } while (!Directory.Exists(lFolderPath));


            return lFolderPath;
        }
    }
}