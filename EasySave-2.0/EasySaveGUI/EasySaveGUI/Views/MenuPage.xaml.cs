using EasySaveGUI.UserControls;
using LogsModels;
using Models.Backup;
using Ressources;
using Stockage.Logs;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        public MenuPage(MainViewModel pMainVm)
        {
            InitializeComponent();
            _MainVm = pMainVm;
            DataContext = _MainVm;
            ListElements.IsVisible = false;
            LayoutAnchorableCreateJob.IsVisible = false;
            DockPanelListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
            DockPanelListDailyLogs.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;

            LayoutAnchorableCreateJob.ToggleAutoHide();
            ConfigInfoDocument.Close();
            JobsListDocument.IsSelected = true;
        }

        #region Events

        #region Button

        #region PropertyPane
        private async void RunJobsButton_Click(object sender, RoutedEventArgs e)
        {
            if (JobUsr.JobsList.SelectedItems.Count > 0)
            {
                ButtonRunJobs.IsEnabled = false;
                System.Collections.IList lJobs = JobUsr.JobsList.SelectedItems;

                List<CJob> lSelectedJobs = lJobs.Cast<CJob>().ToList();
                ClearList();

                JobsRunningDocument.IsActive = true;
                await _MainVm.JobVm.RunJobs(lSelectedJobs);

                ButtonRunJobs.IsEnabled = true;
            }
        }

        private void ButtonDeletesJobs_Click(object sender, RoutedEventArgs e)
        {
            if (JobUsr.JobsList.SelectedItems.Count > 0)
            {
                ButtonRunJobs.IsEnabled = false;
                System.Collections.IList lJobs = JobUsr.JobsList.SelectedItems;

                List<CJob> lSelectedJobs = lJobs.Cast<CJob>().ToList();

                _MainVm.JobVm.DeleteJobs(lSelectedJobs);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearList();
        }
        #endregion

        private void MenuButtons_MouseClick(object sender, RoutedEventArgs e)
        {
            ListElements.Show();
            Button lButton = sender as Button;
            if (lButton.Content == Strings.Config)
                ListElements.Content = new ConfigMenuControl();
            if (lButton.Content == Strings.Settings)
                ListElements.Content = new OptionsMenuControl();
        }

        private void MenuButtonsCreateJob_MouseClick(object sender, RoutedEventArgs e)
        {
            LayoutAnchorableCreateJob.Show();
        }

        #endregion

        #endregion

        private void ClearList()
        {
            CLogger<CLogBase>.Instance.Clear();
            CLogger<CLogDaily>.Instance.Clear();
            _MainVm.JobVm.JobsRunning.Clear();
            DockPanelListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
            DockPanelListDailyLogs.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;
        }
    }
}
