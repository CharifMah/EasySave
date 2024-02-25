using AvalonDock;
using AvalonDock.Layout.Serialization;
using EasySaveGUI.UserControls;
using Models;
using Stockage.Save;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.ViewModels
{
    public class LayoutViewModel : BaseViewModel
    {
        private ContentControl _ElementsContent;
        private ObservableCollection<string> _LayoutNames;
        public ObservableCollection<string> LayoutNames { get => _LayoutNames; set => _LayoutNames = value; }
        public ContentControl ElementsContent { get => _ElementsContent; set { _ElementsContent = value; NotifyPropertyChanged(); } }


        public LayoutViewModel()
        {
            DirectoryInfo lDirInfo = new DirectoryInfo(CSettings.Instance.LayoutDefaultFolderPath);
            if (lDirInfo.Exists)
            {
                FileInfo[] lFiles = lDirInfo.GetFiles();
                if (lFiles.Length > 0)

                    _LayoutNames = new ObservableCollection<string>(lFiles.Select(f => f.Name));
            }
            else
                _LayoutNames = new ObservableCollection<string>();

            _ElementsContent = new JobMenuControl();
        }

        public void SaveLayout(DockingManager pDock, ETheme pTheme, string pLayoutName = "Layout")
        {
            if (!_LayoutNames.Contains(pLayoutName))
                _LayoutNames.Add(pLayoutName);
            if (CSettings.Instance.Theme.LayoutsTheme.ContainsKey(pLayoutName))
            {
                CSettings.Instance.Theme.LayoutsTheme[pLayoutName] = pTheme;
            }
            else
                CSettings.Instance.Theme.LayoutsTheme.Add(pLayoutName, pTheme);

            CSettings.Instance.Theme.CurrentTheme = pTheme;

            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(pDock);
            new SauveCollection(CSettings.Instance.LayoutDefaultFolderPath);
            layoutSerializer.Serialize(Path.Combine(CSettings.Instance.LayoutDefaultFolderPath, pLayoutName));
            CSettings.Instance.SaveSettings();
        }

        public void LoadLayout(DockingManager pDock, string pLayoutName = "Layout")
        {
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(pDock);
            string lPath = Path.Combine(CSettings.Instance.LayoutDefaultFolderPath, pLayoutName);

            if (File.Exists(lPath))
            {
                layoutSerializer.Deserialize(lPath);

                NotifyPropertyChanged("DockLayout");
            }
        }
    }
}
