using EasySaveGUI.UserControls;
using EasySaveGUI.ViewModels;
using LogsModels;
using Models.Settings;
using OpenDialog;
using Stockage.Logs;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.Views
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        #region Attributes
        private MainViewModel _MainVm;
        #endregion

        public MenuPage(MainViewModel pMainVm)
        {
            InitializeComponent();
            _MainVm = pMainVm;
            
            _MainVm.LayoutVm.ElementsContent = new JobMenuControl();

            SetDataContextLogs();
            JobsListDocument.IsSelected = true;

            ValidationAnimation.Hide();
        }

        public void ClearLists()
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                CLogger<CLogBase>.Instance.Clear();
                CLogger<CLogDaily>.Instance.Clear();
                CLogger<List<CLogState>>.Instance.Clear();
                _MainVm.JobVm.JobsRunning.Clear();
                SetDataContextLogs();
            });
        }

        private void SetDataContextLogs()
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                DockPanelListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
                DockPanelListDailyLogs.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;
                DataContext = _MainVm;
            });
        }

        private void OpenLogButton_Click(object sender, RoutedEventArgs e)
        {
            CDialog.ReadFile("", null, CSettings.Instance.LogDefaultFolderPath, true);
        }

        public void ShowValidation()
        {

            ValidationAnimation.Show();

        }
    }
}
