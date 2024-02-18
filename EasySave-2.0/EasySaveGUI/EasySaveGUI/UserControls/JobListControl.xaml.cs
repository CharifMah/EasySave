using Models.Backup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModels;

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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.SelectedJob = ((sender as CheckBox).Content as ContentPresenter).Content as CJob;
            _MainWindow.MenuPage.JobPropertyUsr.PropertyComboBox.SelectedIndex = (int)_MainVm.JobVm.SelectedJob.BackupType;
        }
    }
}
