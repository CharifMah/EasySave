using AvalonDock.Layout;
using EasySaveGUI.ViewModels;
using Gtk;
using Models;
using OpenDialog;
using Ressources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Grid = System.Windows.Controls.Grid;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for ConfigMenuControl.xaml
    /// </summary>
    public partial class ConfigMenuControl : UserControl
    {
        private MainWindow _MainWindow;
        private MainViewModel _MainVm;
        public ConfigMenuControl()
        {
            InitializeComponent();
            _MainWindow = System.Windows.Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
        }

        private void LoadConfigDefaultFileButton_Click(object sender, RoutedEventArgs e)
        {
            CSettings.Instance.ResetJobConfigPath();
            _MainVm.JobVm.LoadJobs();
        }

        private void LoadConfigFileButton_Click(object sender, RoutedEventArgs e)
        {
            string lConfigPath = CDialog.ReadFile($"\n{Strings.ResourceManager.GetObject("SelectConfigurationFile")}", new Regex("^.*\\.(json | JSON)$"), Models.CSettings.Instance.JobConfigFolderPath);
            if (lConfigPath != "-1")
            {
                CSettings.Instance.SetJobConfigPath(lConfigPath);
                _MainVm.JobVm.LoadJobs(false, lConfigPath);
            } 
        }

        private void SaveConfigFileButton_Click(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.SaveJobs();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid lGrid = sender as Grid;

            if (lGrid.ActualWidth >= lGrid.ActualHeight)
            {
                HorizontalMenu.Visibility = Visibility.Visible;
                VerticalMenu.Visibility = Visibility.Hidden;
            }
            else
            {
                VerticalMenu.Visibility = Visibility.Visible;
                HorizontalMenu.Visibility = Visibility.Hidden;
            }
        }
    }
}
