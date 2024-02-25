using EasySaveGUI.ViewModels;
using Models.Backup;
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
        public JobListControl()
        {
            InitializeComponent();
            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
        }

        private void JobsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _MainVm.JobVm.SelectedJob = e.AddedItems[0] as CJob;
                _MainWindow.MenuPage.JobPropertyUsr.PropertyComboBox.SelectedIndex = (int)_MainVm.JobVm.SelectedJob.BackupType;
            }
        }
    }
}
