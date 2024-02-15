using Gtk;
using LogsModels;
using Models.Backup;
using OpenDialog;
using Ressources;
using Stockage.Logs;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModels;
using ListBox = System.Windows.Controls.ListBox;

namespace EasySaveGUI.Views
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        #region Attributes
        private MainViewModel _MainVm;
        #endregion

        public MenuPage()
        {
            InitializeComponent();
            _MainVm = new MainViewModel();
            ListElements.IsVisible = false;
            JobsList.DataContext = _MainVm.JobVm;
            ListLogs.DataContext = CLogger<CLogBase>.StringLogger;
            CLogger<CLogBase>.StringLogger.Datas.CollectionChanged += Datas_CollectionChanged;
            ListLogsState.DataContext = CLogger<CLogBase>.GenericLogger;

            //CLogger<CLogBase>.StringLogger.Datas.CollectionChanged
        }

        private void Datas_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ListLogs.SelectedIndex = ListLogs.Items.Count - 1;
            ListLogs.ScrollIntoView(ListLogs.SelectedItem);
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ListElements.Show();
        }


        private void LoadConfigDefaultFileButton_Click(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.LoadJobs();
        }

        private void LoadConfigFileButton_Click(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.LoadJobs(false, CDialog.ReadFile($"\n{Strings.ResourceManager.GetObject("SelectConfigurationFile")}", new Regex("^.*\\.(json | JSON)$"), System.IO.Path.GetDirectoryName(Models.CSettings.Instance.JobConfigFolderPath)));
        }

        private void RunJobsButton_Click(object sender, RoutedEventArgs e)
        {
            if (JobsList.SelectedItems.Count > 0)
            {
                System.Collections.IList lJobs = JobsList.SelectedItems;
            
                List<CJob> lSelectedJobs = lJobs.Cast<CJob>().ToList();

                _MainVm.JobVm.RunJobs(lSelectedJobs);
            }
        }
    }
}
