﻿using EasySaveDraft.Resources;
using Gtk;

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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine('\n' + pMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Write a personalized Title with separator
        /// </summary>
        /// <param name="pTitle">Title to write</param>
        public static void WriteTitle(string pTitle)
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
            Console.ForegroundColor = ConsoleColor.White;
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
        public static string ReadResponse(string pMessage)
        {
            ConsoleKeyInfo lsInput;
            _Input = string.Empty;
            // cm - Allow Control detection
            Console.TreatControlCAsInput = true;

            do
            {
                if (string.IsNullOrEmpty(_Input))
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
            if (_Input != "-1")
                Console.WriteLine($"\n{Strings.ResourceManager.GetObject("YouSelected")} " + _Input + '\n');

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
            try
            {
                Console.WriteLine(pDescription);

                Application.Init();

                var lDialog = new FileChooserDialog(
                    title: pDescription,
                    parent: null,
                    action: FileChooserAction.SelectFolder);

                lDialog.AddButton(Strings.ResourceManager.GetObject("Cancel").ToString(), ResponseType.Cancel);
                lDialog.AddButton(Strings.ResourceManager.GetObject("Select").ToString(), ResponseType.Ok);

                lDialog.SetCurrentFolder(Directory.GetCurrentDirectory());

                string lSelectedFolder = null;

                if (lDialog.Run() == (int)ResponseType.Ok)
                {
                    lSelectedFolder = lDialog.Filename;
                }

                lDialog.Destroy();

                Console.WriteLine($"\n{Strings.ResourceManager.GetObject("YouSelected")} " + lSelectedFolder + '\n');
                return lSelectedFolder;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string ReadFile(string pDescription)
        {
            try
            {
                Console.WriteLine(pDescription);

                Application.Init();

                var dialog = new FileChooserDialog(
                    title: pDescription,
                    parent: null,
                    action: FileChooserAction.Open);

                dialog.AddButton(Strings.ResourceManager.GetObject("Cancel").ToString(), ResponseType.Cancel);
                dialog.AddButton(Strings.ResourceManager.GetObject("Open").ToString(), ResponseType.Ok);

                dialog.SetCurrentFolder(Directory.GetCurrentDirectory());

                string selectedFile = null;

                if (dialog.Run() == (int)ResponseType.Ok)
                {
                    selectedFile = dialog.Filename;
                }

                dialog.Destroy();

                return selectedFile;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}