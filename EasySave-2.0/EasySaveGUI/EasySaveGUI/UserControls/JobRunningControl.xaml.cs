using Models.Backup;
using OpenDialog;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for JobRunningControl.xaml
    /// </summary>
    public partial class JobRunningControl : UserControl
    {
        public JobRunningControl()
        {
            InitializeComponent();
        }

        private void OpenTargetFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string lTargetDirectory = (button.DataContext as CJob).TargetDirectory;
            CDialog.ReadFile("", null, lTargetDirectory, true);
        }
    }
}
