using EasySaveGUI.UserControls;
using EasySaveGUI.ViewModels;
using LogsModels;
using Models;
using OpenDialog;
using Ressources;
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

            ListElements.Content = new JobMenuControl();
            LayoutAnchorableCreateJob.IsVisible = false;
            DockPanelListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
            DockPanelListDailyLogs.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;

            LayoutAnchorableCreateJob.ToggleAutoHide();
            ConfigInfoDocument.Close();
            JobsListDocument.IsSelected = true;
        }

        private void MenuButtons_MouseClick(object sender, RoutedEventArgs e)
        {
            ListElements.Show();
            Button lButton = sender as Button;
            if (lButton.Content == Strings.Config)
                ListElements.Content = new ConfigMenuControl();
            if (lButton.Content == Strings.Settings)
                ListElements.Content = new OptionsMenuControl();
            if (lButton.Content == Strings.Jobs)
                ListElements.Content = new JobMenuControl();
        }

        public void ClearLists()
        {
            CLogger<CLogBase>.Instance.Clear();
            CLogger<CLogDaily>.Instance.Clear();
            _MainVm.JobVm.JobsRunning.Clear();
            DockPanelListLogs.DataContext = CLogger<CLogBase>.Instance.StringLogger;
            DockPanelListDailyLogs.DataContext = CLogger<CLogDaily>.Instance.GenericLogger;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid lGrid = sender as Grid;

            if (lGrid.ActualWidth >= lGrid.ActualHeight)
            {
                HorizontalMenu.Visibility = Visibility.Visible;
                VerticalMenu.Visibility = Visibility.Hidden;
            }
            else
            {
                VerticalMenu.Visibility = Visibility.Visible;
                HorizontalMenu.Visibility = Visibility.Hidden;
            }
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
