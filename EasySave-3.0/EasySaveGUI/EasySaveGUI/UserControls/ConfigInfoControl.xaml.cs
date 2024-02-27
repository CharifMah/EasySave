using EasySaveGUI.ViewModels;
using OpenDialog;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for ConfigInfoControl.xaml
    /// </summary>
    public partial class ConfigInfoControl : UserControl
    {
        private MainViewModel _MainVm;
        private MainWindow _MainWindow;
        public ConfigInfoControl()
        {
            InitializeComponent();
            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
        }

        private void ComboBoxFormatLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _MainVm.FormatLogVm.SetFormatLog(e.AddedItems[0].ToString());
            }
        }

        private void LoadSoftwareFileButton_Click(object sender, RoutedEventArgs e)
        {
            string lApplicationPath = CDialog.ReadFile("");
            if (lApplicationPath != "-1")
            {
                string lApplicationName = System.IO.Path.GetFileName(lApplicationPath);

                _MainVm.BusinessSoftwareVm.AddBusinessSoftware(lApplicationName);
            }
        }

        private void RemoveSoftwareButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedSoftware = ListBoxBusinessSoftware.SelectedItems.Cast<string>().ToList();
            if (selectedSoftware.Any())
            {
                _MainVm.BusinessSoftwareVm.RemoveBusinessSoftwares(selectedSoftware);
            }
        }

        private void AddEncryptionExtension_Click(object sender, RoutedEventArgs e)
        {
            string lEncryptionExtension = txtEncryptionExtension.Text;
            _MainVm.FileExtensionVm.AddEncryptionExtension(lEncryptionExtension);
            txtEncryptionExtension.Text = string.Empty;
        }

        private void RemoveEncryptionExtensions_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedExtensions = ListBoxEncryptionExtensions.SelectedItems.Cast<string>().ToList();
            if (selectedExtensions.Any())
            {
                _MainVm.FileExtensionVm.RemoveEncryptionExtensions(selectedExtensions);
            }
        }

        private void AddPriorityFileExtension_Click(object sender, RoutedEventArgs e)
        {
            string lPriorityExtension = txtPriorityFileExtension.Text;
            _MainVm.FileExtensionVm.AddPriorityFileExtension(lPriorityExtension);
            txtPriorityFileExtension.Text = string.Empty;
        }

        private void RemovePriorityFileExtensions_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedExtensions = ListBoxPriorityFileExtensions.SelectedItems.Cast<string>().ToList();
            if (selectedExtensions.Any())
            {
                _MainVm.FileExtensionVm.RemovePriorityFileExtensions(selectedExtensions);
            }
        }
    }
}
