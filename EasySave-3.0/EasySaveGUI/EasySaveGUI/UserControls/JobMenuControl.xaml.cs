using EasySaveGUI.ViewModels;
using Gtk;
using Models.Backup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;
using Grid = System.Windows.Controls.Grid;
using Window = System.Windows.Window;

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
                await _MainWindow.MenuPage.JobUsr.RunSelectJobs(lButton);
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


    }
}
