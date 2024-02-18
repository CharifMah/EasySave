using EasySaveGUI.UserControls;
using LogsModels;
using Models;
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

        public MenuPage(MainViewModel pMainVm)
        {
            InitializeComponent();
            _MainVm = pMainVm;
            ListElements.IsVisible = false;
            LayoutAnchorableCreateJob.ToggleAutoHide();
            DataContext = _MainVm;
            DockPanelListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
            DockPanelListDailyLogs.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;
        }

        #region Events

        #region Button

        #region PropertyPane
        private async void RunJobsButton_Click(object sender, RoutedEventArgs e)
        {
            if (JobsList.SelectedItems.Count > 0)
            {
                ButtonRunJobs.IsEnabled = false;
                System.Collections.IList lJobs = JobsList.SelectedItems;

                List<CJob> lSelectedJobs = lJobs.Cast<CJob>().ToList();
                ClearList();

                JobsPaneGroup.SelectedContentIndex = JobsPaneGroup.Children.IndexOf(JobsRunningDocument);
                await _MainVm.JobVm.RunJobs(lSelectedJobs);

                ButtonRunJobs.IsEnabled = true;
            }
        }

        private void ButtonDeletesJobs_Click(object sender, RoutedEventArgs e)
        {
            if (JobsList.SelectedItems.Count > 0)
            {
                ButtonRunJobs.IsEnabled = false;
                System.Collections.IList lJobs = JobsList.SelectedItems;

                List<CJob> lSelectedJobs = lJobs.Cast<CJob>().ToList();

                _MainVm.JobVm.DeleteJobs(lSelectedJobs);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearList();
        }
        #endregion

        #region ListElementsPane

        private void MenuButtons_MouseClick(object sender, RoutedEventArgs e)
        {
            ListElements.Show();
            Button lButton = sender as Button;
            if (lButton.Content == Strings.Config)
                ListElements.Content = new ConfigMenuControl();
            if (lButton.Content == Strings.Settings)
                ListElements.Content = new OptionsMenuControl();
        }

        #endregion

        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.SelectedJob = ((sender as CheckBox).Content as ContentPresenter).Content as CJob;
            JobPropertyUsr.PropertyComboBox.SelectedIndex = (int)_MainVm.JobVm.SelectedJob.BackupType;
        }

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
