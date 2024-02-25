using AvalonDock;
using EasySaveGUI.UserControls;
using EasySaveGUI.ViewModels;
using LogsModels;
using Models;
using OpenDialog;
using Stockage.Logs;
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
            DataContext = _MainVm;
            _MainVm.LayoutVm.ElementsContent = new JobMenuControl();
            DockPanelListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
            DockPanelListDailyLogs.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;

            JobsListDocument.IsSelected = true;
        }

        public void ClearLists()
        {
            CLogger<CLogBase>.Instance.Clear();
            CLogger<CLogDaily>.Instance.Clear();
            _MainVm.JobVm.JobsRunning.Clear();
            DockPanelListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
            DockPanelListDailyLogs.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;
        }

        private void ListLogs_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            if (ListLogs.Items.Count > 0)
                ListLogs.ScrollIntoView(ListLogs.Items[ListLogs.Items.Count - 1]);
        }

        private void ListLogsDaily_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (ListLogsDaily.Items.Count > 0)
                ListLogsDaily.ScrollIntoView(ListLogsDaily.Items[ListLogsDaily.Items.Count - 1]);
        }

        private void OpenLogButton_Click(object sender, RoutedEventArgs e)
        {
            CDialog.ReadFile("", null, CSettings.Instance.LogDefaultFolderPath, true);
        }
    }
}
