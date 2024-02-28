using EasySaveGUI.ViewModels;
using Gtk;
using Models.Backup;
using OpenDialog;
using Pango;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static System.Reflection.Metadata.BlobBuilder;
using Button = System.Windows.Controls.Button;
using MenuItem = System.Windows.Controls.MenuItem;
using Window = System.Windows.Window;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for JobRunningControl.xaml
    /// </summary>
    public partial class JobRunningControl : UserControl
    {
        private MainViewModel _MainVm;
        private MainWindow _MainWindow;
        public JobRunningControl()
        {
            InitializeComponent();
            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
        }

        private void OpenTargetFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string lTargetDirectory = (button.DataContext as CJob).TargetDirectory;
            CDialog.ReadFile("", null, lTargetDirectory, true);
        }

        private void SelectAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItems.Count == DataGrid.Items.Count)
                DataGrid.UnselectAll();
            else
                DataGrid.SelectAll();

            _MainWindow.MenuPage.ShowValidation();
        }

        private void StartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItems.Count > 0)
            {
                Button? lButton = sender as Button;
                lButton.IsEnabled = false;
                IList lItems = DataGrid.SelectedItems;
                List<CJob> lSelectedJobs = lItems.Cast<CJob>().ToList();
                _MainVm.JobVm.Resume(lSelectedJobs);
                
                lButton.IsEnabled = true;
                _MainWindow.MenuPage.ShowValidation();
                StartButton.Visibility = Visibility.Visible;
            }
        }

        private void StopMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IList lItems = DataGrid.SelectedItems;
            List<CJob> lSelectedJobs = lItems.Cast<CJob>().ToList();
            _MainVm.JobVm.Stop(lSelectedJobs);
  
            ClearSelectedJobRunningList(lSelectedJobs);
            _MainWindow.MenuPage.ShowValidation();
        }

        private void ClearSelectedJobRunningList(List<CJob> pJobs)
        {
            foreach (CJob lItem in pJobs)
            {
                _MainVm.JobVm.JobsRunning.Remove(lItem);
            }
        }

        private void PauseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            StartButton.Visibility = Visibility.Visible;

            IList lItems = DataGrid.SelectedItems;
            List<CJob> lSelectedJobs = lItems.Cast<CJob>().ToList();
            _MainVm.JobVm.Pause(lSelectedJobs);
            _MainWindow.MenuPage.ShowValidation();
        }
    }
}
