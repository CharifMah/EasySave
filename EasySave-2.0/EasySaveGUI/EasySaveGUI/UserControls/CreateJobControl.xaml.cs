using Models.Backup;
using OpenDialog;
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
    /// Interaction logic for CreateJobControl.xaml
    /// </summary>
    public partial class CreateJobControl : UserControl
    {
        private MainViewModel _MainVm;
        public CreateJobControl()
        {   
            InitializeComponent();
            _MainVm = (Window.GetWindow(App.Current.MainWindow) as MainWindow).MainVm;
        }

        #region CreateJob

        private void CreateJobButton_Click(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.CreateBackupJob(new CJob(TextBoxJobName.Text,
                TextBoxJobSourceDirectory.Text, TextBoxJobTargetDirectory.Text, (ETypeBackup)ComboboxCreateJob.SelectedIndex));
        }
        private void FolderSourcePropertyButton_Click(object sender, RoutedEventArgs e)
        {
            TextBoxJobSourceDirectory.Text = CDialog.ReadFolder("SourceDir");
        }

        private void FolderTargetPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            TextBoxJobTargetDirectory.Text = CDialog.ReadFolder("TargetDir");
        }

        private void SaveConfigFileButton_Click(object sender, RoutedEventArgs e)
        {
            _MainVm.JobVm.SaveJobs();
        }

        #endregion

    }
}
