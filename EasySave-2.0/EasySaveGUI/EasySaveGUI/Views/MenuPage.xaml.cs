using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using Gtk;
using LogsModels;
using Models.Backup;
using OpenDialog;
using Ressources;
using Stockage.Logs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            Dock.OverridesDefaultStyle = true;
            DataContext = _MainVm;
            JobsList.DataContext = _MainVm.JobVm;
            DockPanelListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
            DockPanelListDailyLogs.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;   
        }

        #region Events

        #region Button
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

        private void SaveConfigFileButton_Click(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.SaveJobs();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.SaveJobs();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearList();
        }

        private void CreateJobButton_Click(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.CreateBackupJob(new CJob(TextBoxJobName.Text,
                TextBoxJobSourceDirectory.Text, TextBoxJobTargetDirectory.Text, (ETypeBackup)ComboboxCreateJob.SelectedIndex));
            LayoutAnchorableCreateJob.ToggleAutoHide();
        }

        private void FolderSourcePropertyButton_Click(object sender, RoutedEventArgs e)
        {
            TextBoxJobSourceDirectory.Text = CDialog.ReadFolder("SourceDir");
        }

        private void FolderTargetPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            TextBoxJobTargetDirectory.Text = CDialog.ReadFolder("TargetDir");
        }

        private void ApplyDefaultStyle_Click(object sender, RoutedEventArgs e)
        {
            Dock.UpdateLayout();
        }

        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.SelectedJob = ((sender as CheckBox).Content as ContentPresenter).Content as CJob;
            PropertyComboBox.SelectedIndex = (int)_MainVm.JobVm.SelectedJob.BackupType;
        }

        #endregion

        private void ClearList()
        {
            CLogger<CLogBase>.Instance.Clear();
            CLogger<CLogDaily>.Instance.Clear();
            DockPanelListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
            DockPanelListDailyLogs.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;
        }

        private void TextBoxSourceDirectory_Error(object sender, ValidationErrorEventArgs e)
        {
            _MainVm.PopupVm.Message = e.Error.ErrorContent.ToString();
            PopupError.Show();
        }
    }
}
