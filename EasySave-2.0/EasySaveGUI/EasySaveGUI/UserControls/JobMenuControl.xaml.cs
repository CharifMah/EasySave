using EasySaveGUI.ViewModels;
using Models.Backup;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for JobMenuControl.xaml
    /// </summary>
    public partial class JobMenuControl : UserControl
    {
        private MainWindow _MainWindow;
        public JobMenuControl()
        {
            InitializeComponent();

            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
        }

        private async void RunJobsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_MainWindow.MenuPage.JobUsr.JobsList.SelectedItems.Count > 0)
            {
                Button lButton = sender as Button;
                lButton.IsEnabled = false;
                System.Collections.IList lJobs = _MainWindow.MenuPage.JobUsr.JobsList.SelectedItems;

                List<CJob> lSelectedJobs = lJobs.Cast<CJob>().ToList();
                _MainWindow.MenuPage.ClearLists();

                _MainWindow.MenuPage.JobsRunningDocument.IsActive = true;
                await _MainWindow.MainVm.JobVm.RunJobs(lSelectedJobs);

                lButton.IsEnabled = true;
            }
        }

        private void ButtonDeletesJobs_Click(object sender, RoutedEventArgs e)
        {
            if (_MainWindow.MenuPage.JobUsr.JobsList.SelectedItems.Count > 0)
            {
                System.Collections.IList lJobs = _MainWindow.MenuPage.JobUsr.JobsList.SelectedItems;

                List<CJob> lSelectedJobs = lJobs.Cast<CJob>().ToList();

                _MainWindow.MainVm.JobVm.DeleteJobs(lSelectedJobs);
            }
        }

        private void CreateJobButton_Click(object sender, RoutedEventArgs e)
        {
            _MainWindow.MenuPage.LayoutAnchorableCreateJob.Show();
            _MainWindow.MenuPage.LayoutAnchorableCreateJob.IsActive = true;
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
