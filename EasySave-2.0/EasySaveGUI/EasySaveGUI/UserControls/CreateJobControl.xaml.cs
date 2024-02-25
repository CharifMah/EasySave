using EasySaveGUI.ValidationRules;
using EasySaveGUI.ViewModels;
using Models.Backup;
using OpenDialog;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for CreateJobControl.xaml
    /// </summary>
    public partial class CreateJobControl : UserControl
    {
        private MainWindow _MainWindow;
        private MainViewModel _MainVm;
        public CreateJobControl()
        {
            InitializeComponent();
            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
        }

        #region CreateJob

        private void CreateJobButton_Click(object sender, RoutedEventArgs e)
        {
            string lCheckError = CheckInputs();
            if (lCheckError.Any())
            {
                _MainVm.PopupVm.Message = lCheckError;
                _MainWindow.MenuPage.PopupError.Show();
            }
            else
            {
                _MainVm.JobVm.CreateBackupJob(new CJob(TextBoxJobName.Text,
                    TextBoxJobSourceDirectory.Text, TextBoxJobTargetDirectory.Text, (ETypeBackup)ComboboxCreateJob.SelectedIndex));
            }
        }

        private string CheckInputs()
        {
            string lResult =
                string.Empty;

            if (String.IsNullOrEmpty(TextBoxJobName.Text))
            {
                lResult += "Le nom est vide\n";
            }

            if (String.IsNullOrEmpty(TextBoxJobSourceDirectory.Text))
            {
                lResult += "Le nom de la SourceDirectory est vide\n";
            }

            if (String.IsNullOrEmpty(TextBoxJobTargetDirectory.Text))
            {
                lResult += "Le nom de la TargetDirectory est vide\n";
            }

            if (ComboboxCreateJob.SelectedItem == null)
            {
                lResult += Ressources.Strings.BackupTypeEmpty + '\n';
            }
            return lResult;
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

        private void TextBoxJobDirectory_Error(object sender, ValidationErrorEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox lTextBox = sender as TextBox;
                lTextBox.Focus();
                lTextBox.CaretIndex = lTextBox.Text.Length - 1;

            }
            ShowError(e.Error.ErrorContent.ToString());
        }

        private void TextBoxJobSourceDirectory_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox lTextBox = sender as TextBox;

            if (lTextBox != null)
            {
                ValidationResult lValidationResult = new FolderPathValidator().Validate(lTextBox.Text, CultureInfo.CurrentCulture);
                if (!lValidationResult.IsValid)
                {
                    ShowError(lValidationResult.ErrorContent.ToString());
                }
            }
        }

        private void ShowError(string pMessage)
        {
            _MainVm.PopupVm.Message = pMessage;
            _MainWindow.MenuPage.PopupError.Show();
        }
    }
}
