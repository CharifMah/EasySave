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
        private MainWindow? _MainWindow;
        private MainViewModel _MainVm;
        public JobMenuControl()
        {
            InitializeComponent();

            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
        }

        private async void RunJobsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_MainWindow.MenuPage.JobUsr.JobsList.SelectedItems.Count > 0)
            {
                Button? lButton = sender as Button;
                lButton.IsEnabled = false;
                System.Collections.IList lJobs = _MainWindow.MenuPage.JobUsr.JobsList.SelectedItems;

                List<CJob> lSelectedJobs = lJobs.Cast<CJob>().ToList();
                _MainWindow.MenuPage.ClearLists();

                _MainWindow.MenuPage.JobsRunningDocument.IsActive = true;
 
                // S'abonne à l'événement pour la detection d'un logiciel métier
                _MainWindow.MainVm.JobVm.OnBusinessSoftwareDetected += ShowError;

                await _MainWindow.MainVm.JobVm.RunJobs(lSelectedJobs);

                // Se désabonne de l'événement pour la detection du logiciel métier
                _MainWindow.MainVm.JobVm.OnBusinessSoftwareDetected -= ShowError;

                lButton.IsEnabled = true;
                _MainWindow.MenuPage.ShowValidation();
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
            _MainWindow.MenuPage.ShowValidation();
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

        private void ShowError(string pMessage)
        {
            _MainVm.PopupVm.Message = pMessage;
            _MainWindow.MenuPage.PopupError.Show();
        }
    }
}
