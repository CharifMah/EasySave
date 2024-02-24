using Gtk;
using Ressources;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace OpenDialog
{
    public static class CDialog
    {
        private readonly static object _lock = new object();
        /// <summary>
        /// Read a file with GTK CrossPlatform interface if it fail open classic Console Interface
        /// </summary>
        /// <param name="pDescription">Description for the interface</param>
        /// <returns>return the selected file full path</returns>
        public static string ReadFile(string pDescription, Regex pRegexExtentions = null, string pCurrentFolder = null, bool pOpenFile = false)
        {

            string lSelectedFile = null;

            try
            {
                Application.Init();

                FileChooserDialog lFileDialog = new FileChooserDialog(
                       title: pDescription,
                       parent: null,
                       action: FileChooserAction.Open);
                lFileDialog.FontOptions = null;
                if (pRegexExtentions != null && pRegexExtentions.ToString().Contains("json"))
                {
                    FileFilter lFilter = new FileFilter();
                    lFilter.Name = pDescription;
                    lFilter.AddPattern("*.json");
                    lFileDialog.AddFilter(lFilter);
                }
                lFileDialog.AddButton(Strings.ResourceManager.GetObject("Cancel").ToString(), ResponseType.Cancel);
                lFileDialog.AddButton(Strings.ResourceManager.GetObject("Open").ToString(), ResponseType.Ok);
                lFileDialog.SetCurrentFolder(pCurrentFolder);

                unsafe
                {
                    if (lFileDialog.Run() == (int)ResponseType.Ok)
                    {
                        lSelectedFile = lFileDialog.Filename;
                        if (pOpenFile)
                        {
                            // Ouvre le fichier avec l'application par défaut sur windows
                            RuntimeInformation.OSDescription.StartsWith("Windows");
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = lSelectedFile,
                                UseShellExecute = true
                            });
                        }
                    }
                    else
                        lSelectedFile = "-1";
                    lFileDialog.Dispose();
                    lFileDialog.Destroy();
                }
            }
            catch (Exception)
            {

            }

            return lSelectedFile;
        }

        public static string ReadFolder(string pDescription = "", string pPath = null)
        {
            string lSelectedFolder = null;

            try
            {
                Application.Init();
                FileChooserDialog lDialog = null;

                lDialog = new FileChooserDialog(
                   title: pDescription,
                   parent: null,
                   action: FileChooserAction.SelectFolder
                );
                lDialog.FontOptions = null;
                lDialog.AddButton(Strings.ResourceManager.GetObject("Cancel").ToString(), ResponseType.Cancel);
                lDialog.AddButton(Strings.ResourceManager.GetObject("Select").ToString(), ResponseType.Ok);

                if (pPath == null)
                    pPath = Directory.GetCurrentDirectory();
                lDialog.SetCurrentFolder(pPath);
                unsafe
                {
                    if (lDialog.Run() == (int)ResponseType.Ok)
                    {

                        lSelectedFolder = lDialog.Filename;
                    }
                    else
                        lSelectedFolder = "-1";
                    lDialog.Dispose();
                    lDialog.Destroy();
                }
            }
            catch (Exception)
            {

            }

            return lSelectedFolder;
        }

        /// <summary>
        /// Check if GTK can init GUI or not
        /// </summary>
        /// <returns>true if GTK can init the GUI</returns>
        public static bool CheckIfGuiExist()
        {
            string[] argrs = new string[] { };
            return Gtk.Application.InitCheck("", ref argrs);
        }
    }
}