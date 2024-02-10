using EasySaveDraft.Resources;
using Gtk;
using System;
using System.Text.RegularExpressions;

namespace EasySave.Views
{
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
        /// Write line a succes in green
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
        /// Write a default message + input
        /// </summary>
        /// <param name="pInput"></param>
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
        /// Read user input char by char
        /// </summary>
        /// <param name="pMessage">Message to loop through if the user makes an input error</param>
        /// <param name="pRegex">Regex permettant de validée l'entrée utilisateur</param>
        /// <param name="pIsValid">Fonction qui prend un string en parametre et valide l'entrée utilisateur</param>
        /// <returns>user input</returns>
        /// <remarks>Mahmoud Charif - 05/02/2024 - Création</remarks>
        /// <remarks>Mahmoud Charif - 09/02/2024 - Ajout d'une fonction anonyme de validation en paramètre</remarks>
        public static string ReadResponse(string pMessage, Regex? pRegex = null, Func<string, bool> pIsValid = null)
        {
            ConsoleKeyInfo lsInput = default;
            _Input = string.Empty;
            // cm - Allow Control detection
            Console.TreatControlCAsInput = true;

            if (pRegex == null)
                pRegex = new Regex("");
            if (pIsValid == null)
                pIsValid = lValidTxt => { return true; };

            do
            {
                // cm - Displays a message if certain shortcuts are not pressed.
                if (string.IsNullOrEmpty(_Input) && lsInput.Key != ConsoleKey.V && lsInput.Modifiers != ConsoleModifiers.Control)
                    Console.Write(pMessage);

                while (Console.KeyAvailable)
                    Console.ReadKey(false); // cm - skips previous inputs

                lsInput = Console.ReadKey(); // cm - wait to read a new character

                // cm - If the user enters CTRL+C cancel the loop and clear the console
                if ((lsInput.Modifiers & ConsoleModifiers.Control) != 0 && lsInput.Key == ConsoleKey.C)
                {
                    Clear();
                    break;
                }

                // cm - CTRL+V for past
                if (lsInput.Key == ConsoleKey.V && lsInput.Modifiers == ConsoleModifiers.Control)
                {
                    string lText = TextCopy.ClipboardService.GetText(); // cm - Get the text from clipboard
                    _Input += lText;
                    Console.Write(lText);
                }
                else if (lsInput.Key != ConsoleKey.Enter && lsInput.Key != ConsoleKey.Backspace) // cm - Concatenate inputs to obtain the final output
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

            if (lsInput.KeyChar == (char)ConsoleKey.Enter && (!pRegex.IsMatch(_Input) || !pIsValid(_Input)))
            {
                WriteLineError(Strings.ResourceManager.GetObject("InvalideSelection").ToString());

                ReadResponse(pMessage, pRegex, pIsValid);
            }

            return _Input;
        }
        /// <summary>
        /// Clear the console and set the input to -1
        /// </summary>
        public static void Clear()
        {
            Console.Clear();
            _Input = "-1";
        }
        /// <summary>
        /// Remove the last input in the console
        /// </summary>
        private static void RemoveLastChar()
        {
            Console.SetCursorPosition(Console.GetCursorPosition().Left + 1, Console.GetCursorPosition().Top);
            Console.Write("\b \b");
        }
        /// <summary>
        /// Read a folder with GTK CrossPlatform interface if it fail open classic Console Interface
        /// </summary>
        /// <param name="pDescription">Description for the interface</param>
        /// <returns>return the selected folder full path</returns>
        public static string ReadFolder(string pDescription)
        {
            string lSelectedFolder = null;
            FileChooserDialog lDialog = null;

            try
            {
                Console.WriteLine(pDescription);

                string[] argrs = new string[] { };

              
                if (Gtk.Application.InitCheck("", ref argrs))
                {
                    lDialog = new FileChooserDialog(
                       title: pDescription,
                       parent: null,
                       action: FileChooserAction.SelectFolder
                    );

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
                }
                else
                {
                    lSelectedFolder = ReadFolderConsole();
                }


            }
            catch (Exception ex)
            {
                WriteLineError(ex.Message);
                lSelectedFolder = ReadFolderConsole();
            }
            WriteLineSelected(lSelectedFolder);
            return lSelectedFolder;
        }
        /// <summary>
        /// Read a file with GTK CrossPlatform interface if it fail open classic Console Interface
        /// </summary>
        /// <param name="pDescription">Description for the interface</param>
        /// <returns>return the selected file full path</returns>
        public static string ReadFile(string pDescription,Regex pRegexExtentions = null)
        {
            string lSelectedFile = null;
            FileChooserDialog lDialog = null;
            try
            {
                Console.WriteLine(pDescription);

                string[] argrs = new string[] { };

                if (Gtk.Application.InitCheck("", ref argrs))
                {
                    lDialog = new FileChooserDialog(
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
                }
                else
                {
                    lSelectedFile = ReadFileConsole(pRegexExtentions);
                }
            }
            catch (Exception ex)
            {
                WriteLineError(ex.Message);
                lSelectedFile = ReadFileConsole(pRegexExtentions);
            }

            WriteLineSelected(lSelectedFile);

            return lSelectedFile;
        }
        /// <summary>
        /// Wait path from the console input
        /// </summary>
        /// <param name="pRegex">filter the file or the extention</param>
        /// <returns>file fullpath</returns>
        private static string ReadFileConsole(Regex pRegex = null)
        {
            string lFilePath = String.Empty;

            Func<string, bool> pValidator = lPath => File.Exists(lPath);

            do
            {
                lFilePath = ReadResponse("\nEnter file path: ", pRegex, pValidator);
                if (lFilePath == "-1")
                    return lFilePath;
            } while (!File.Exists(lFilePath));

            return lFilePath;
        }
        /// <summary>
        /// Wait path from the console input
        /// </summary>
        /// <returns>folder fullpath</returns>
        private static string ReadFolderConsole()
        {
            string lFolderPath = String.Empty;

            Func<string, bool> pValidator = lPath => Directory.Exists(lPath);

            do
            {
                lFolderPath = ReadResponse("\nEnter folder path: ", null, pValidator);
                if (lFolderPath == "-1")
                    return lFolderPath;
            } while (!Directory.Exists(lFolderPath));


            return lFolderPath;
        }      
    }
}