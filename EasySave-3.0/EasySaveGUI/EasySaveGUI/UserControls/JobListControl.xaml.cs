using AvalonDock.Layout;
using EasySaveGUI.ViewModels;
using Models.Backup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Window = System.Windows.Window;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for JobListControl.xaml
    /// </summary>
    public partial class JobListControl : UserControl
    {
        private MainViewModel _MainVm;
        private MainWindow _MainWindow;
        public JobListControl(ClientViewModel pClientVm = null)
        {
            InitializeComponent();
            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
            if (pClientVm != null)
                DataContext = pClientVm;
            else
                DataContext = _MainVm;
        }
        public JobListControl()
        {
            InitializeComponent();
            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
            DataContext = _MainVm;
        }

        private void JobsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _MainVm.JobVm.SelectedJob = e.AddedItems[0] as CJob;
                _MainWindow.MenuPage.JobPropertyUsr.PropertyComboBox.SelectedIndex = (int)_MainVm.JobVm.SelectedJob.BackupType;
            }
        }

        private void SelectAllMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (JobsList.SelectedItems.Count == JobsList.Items.Count)
                JobsList.UnselectAll();
            else
                JobsList.SelectAll();

            _MainWindow.MenuPage.ShowValidation();
        }

        private async void RunJobsMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_MainWindow.MenuPage.JobUsr.JobsList.SelectedItems.Count > 0)
            {
                await RunSelectJobs();
            }
        }

        public async Task RunSelectJobs(Button pButton = null)
        {
            RunButton.IsEnabled = false;
            if (pButton != null)
                pButton.IsEnabled = false;

            System.Collections.IList lJobs = JobsList.SelectedItems;

            List<CJob> lSelectedJobs = lJobs.Cast<CJob>().ToList();
            _MainWindow.MenuPage.ClearLists();

            LayoutAnchorable? lJobsRunningDocument = _MainWindow.MenuPage.Dock.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(lc => lc.ContentId == "JobsRunningDocument");
            if (lJobsRunningDocument != null)
                lJobsRunningDocument.IsActive = true;

            _MainVm.JobVm.OnBusinessSoftwareDetected += ShowError;

            await _MainVm.JobVm.RunJobs(lSelectedJobs);

            // Se désabonne de l'événement pour la detection du logiciel métier
            _MainVm.JobVm.OnBusinessSoftwareDetected -= ShowError;

            if (pButton != null)
                pButton.IsEnabled = true;
            RunButton.IsEnabled = true;

            _MainWindow.MenuPage.ShowValidation();
            await Task.Delay(2000);
        }

        private void ShowError(string pMessage)
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                _MainVm.PopupVm.Message = pMessage;
                _MainWindow.MenuPage.PopupError.Show();
            });
        }
    }
}
