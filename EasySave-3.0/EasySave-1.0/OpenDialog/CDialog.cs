using Gtk;
using Ressources;
using System.Text.RegularExpressions;

namespace OpenDialog
{
    public static class CDialog
    {
        /// <summary>
        /// Read a file with GTK CrossPlatform interface if it fail open classic Console Interface
        /// </summary>
        /// <param name="pDescription">Description for the interface</param>
        /// <returns>return the selected file full path</returns>
        public static string ReadFile(string pDescription, Regex pRegexExtentions = null, string pCurrentFolder = null)
        {
            string lSelectedFile = null;
            FileChooserDialog lDialog = null;
            lDialog = new FileChooserDialog(
                   title: pDescription,
                   parent: null,
                   action: FileChooserAction.Open);
            if (pRegexExtentions != null && pRegexExtentions.ToString().Contains("json"))
            {
                FileFilter lFilter = new FileFilter();
                lFilter.Name = pDescription;
                lFilter.AddPattern("*.json");
                lDialog.AddFilter(lFilter);
            }
            lDialog.AddButton(Strings.ResourceManager.GetObject("Cancel").ToString(), ResponseType.Cancel);
            lDialog.AddButton(Strings.ResourceManager.GetObject("Open").ToString(), ResponseType.Ok);
            lDialog.SetCurrentFolder(pCurrentFolder);
            if (lDialog.Run() == (int)ResponseType.Ok)
            {
                lSelectedFile = lDialog.Filename;
            }
            else
                lSelectedFile = "-1";
            lDialog.Destroy();

            return lSelectedFile;
        }

        public static string ReadFolder(string pDescription)
        {
            string lSelectedFolder = null;
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