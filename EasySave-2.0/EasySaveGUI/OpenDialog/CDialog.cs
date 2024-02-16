using Gtk;
using Ressources;
using System.Text.RegularExpressions;

namespace OpenDialog
{
    public static class CDialog
    {
        private static FileChooserDialog _FileDialog = null;
        private readonly static object _lock = new object();
        /// <summary>
        /// Read a file with GTK CrossPlatform interface if it fail open classic Console Interface
        /// </summary>
        /// <param name="pDescription">Description for the interface</param>
        /// <returns>return the selected file full path</returns>
        public static string ReadFile(string pDescription, Regex pRegexExtentions = null, string pCurrentFolder = null)
        {

            string lSelectedFile = null;

            try
            {
                if (_FileDialog != null)
                    _FileDialog.Destroy();

                Application.Init();

                _FileDialog = new FileChooserDialog(
                       title: pDescription,
                       parent: null,
                       action: FileChooserAction.Open);

                if (pRegexExtentions != null && pRegexExtentions.ToString().Contains("json"))
                {
                    FileFilter lFilter = new FileFilter();
                    lFilter.Name = pDescription;
                    lFilter.AddPattern("*.json");
                    _FileDialog.AddFilter(lFilter);
                }
                _FileDialog.AddButton(Strings.ResourceManager.GetObject("Cancel").ToString(), ResponseType.Cancel);
                _FileDialog.AddButton(Strings.ResourceManager.GetObject("Open").ToString(), ResponseType.Ok);
                _FileDialog.SetCurrentFolder(pCurrentFolder);


                if (_FileDialog.Run() == (int)ResponseType.Ok)
                {
                    lSelectedFile = _FileDialog.Filename;
                }
                else
                    lSelectedFile = "-1";
                _FileDialog.Destroy();

            }
            catch (Exception)
            {

            }

            return lSelectedFile;
        }

        public static string ReadFolder(string pDescription)
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