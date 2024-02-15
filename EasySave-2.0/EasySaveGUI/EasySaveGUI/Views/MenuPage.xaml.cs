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

            DataContext = _MainVm;
            JobsList.DataContext = _MainVm.JobVm;
            ListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
            ListLogsDaily.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;
        }

        private async void RunJobsButton_Click(object sender, RoutedEventArgs e)
        {
            if (JobsList.SelectedItems.Count > 0)
            {
                System.Collections.IList lJobs = JobsList.SelectedItems;

                List<CJob> lSelectedJobs = lJobs.Cast<CJob>().ToList();
                CLogger<CLogBase>.Instance.Clear();
                await _MainVm.JobVm.RunJobs(lSelectedJobs);
            }
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



        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.SelectedJob = ((sender as CheckBox).Content as ContentPresenter).Content as CJob;
            PropertyComboBox.SelectedIndex = (int)_MainVm.JobVm.SelectedJob.BackupType;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            CLogger<CLogBase>.Instance.StringLogger.Log("Update failed", false);
            _MainVm.JobVm.SaveJobs();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (!ListLogs.Items.IsInUse)
                ListLogs.Items.Clear();

            CLogger<CLogBase>.Instance.GenericLogger.Clear();
        }
    }
}
