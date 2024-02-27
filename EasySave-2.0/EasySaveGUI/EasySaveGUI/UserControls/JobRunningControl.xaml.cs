using LogsModels;
using Models.Backup;
using Models.Settings;
using OpenDialog;
using Stockage.Save;
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
