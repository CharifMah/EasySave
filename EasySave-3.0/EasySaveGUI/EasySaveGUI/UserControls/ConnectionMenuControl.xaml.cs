using AvalonDock.Controls;
using AvalonDock.Layout;
using EasySaveGUI.ViewModels;
using EasySaveGUI.Views;
using Gtk;
using Models;
using Ressources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Grid = System.Windows.Controls.Grid;
using Style = System.Windows.Style;
using Window = System.Windows.Window;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for ConnectionMenuControl.xaml
    /// </summary>
    public partial class ConnectionMenuControl : UserControl
    {
        private readonly object _lock = new object();
        private List<LayoutDocument> _Documents;
        private MainWindow? _MainWindow;
        private MainViewModel _MainVm;

        public ConnectionMenuControl()
        {
            InitializeComponent();

            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
            _Documents = new List<LayoutDocument>();
        }

        public void UpdateListClients(ObservableCollection<ClientViewModel> pClients)
        {
            VerticalMenu.Children.Clear();
            HorizontalMenu.Children.Clear();
            HorizontalMenu.ColumnDefinitions.Clear();
            VerticalMenu.RowDefinitions.Clear();
            for (int i = 0; i < pClients.Count; i++)
            {
                ColumnDefinition lCol = new ColumnDefinition();
                RowDefinition lRow = new RowDefinition();
                HorizontalMenu.ColumnDefinitions.Add(lCol);
                VerticalMenu.RowDefinitions.Add(lRow);

                Button lButtonHorizontal = new Button();
                lButtonHorizontal.Style = (Style)Application.Current.FindResource("CustomButtonJobs");
                Grid.SetColumn(lButtonHorizontal, i);
                lButtonHorizontal.Content = pClients[i].Client.ConnectionId;

                Button lButton = new Button();
                lButton.Style = (Style)Application.Current.FindResource("CustomButtonJobs");
                lButton.Content = pClients[i].Client.ConnectionId;

                Grid.SetRow(lButton, i);

                if (UserViewModel.Instance.Connection.ConnectionId == pClients[i].Client.ConnectionId)
                {
                    lButton.Background = (Brush)_MainWindow.MenuPage.Resources["LightGreenColor"];
                    lButtonHorizontal.Background = (Brush)_MainWindow.MenuPage.Resources["LightGreenColor"];
                }
                else
                {
                    lButtonHorizontal.Click += UpdateJobViewModelButton_Click;
                    lButton.Click += UpdateJobViewModelButton_Click;
                }

                VerticalMenu.Children.Add(lButton);
                HorizontalMenu.Children.Add(lButtonHorizontal);
            }
        }

        private void UpdateJobViewModelButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            OpenClientDocument(button.Content.ToString());
        }

        public void UpdateClientViewModel(string pConnectionId)
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                LayoutDocument? lLayoutDocument = _MainWindow.MenuPage.Dock.Layout.Descendents().OfType<LayoutDocument>().FirstOrDefault(l => l.ContentId.Contains(pConnectionId));

                if (lLayoutDocument != null)
                {
                    StackPanel stackPanel = (lLayoutDocument.Content as StackPanel);
                    ClientViewModel lClientVm = UserViewModel.Instance.Clients.First(c => c.Client.ConnectionId == pConnectionId);
                    foreach (var item in lClientVm.JobVm.JobsRunning)
                    {
                        item.SauveJobs.CancelationTokenSource = new CancellationTokenSource();
                        item.SauveJobs.PauseEvent = new ManualResetEventSlim(false);
                        if (item.SauveJobs.LogState.IsStopped)
                        {
                            item.Stop();
                        }
                        if (item.SauveJobs.LogState.IsPaused)
                        {
                            item.Pause();
                        }
                        if (item.SauveJobs.LogState.IsStarted)
                        {
                            item.Resume();
                        }
                    }
                    JobRunningControl lJobLlistCtrl = (stackPanel.Children[0] as JobRunningControl);

                    JobListControl lJobListCtrl = (stackPanel.Children[1] as JobListControl);
                    lJobLlistCtrl.DataContext = lClientVm;
                    lJobListCtrl.DataContext = lClientVm;
                    lLayoutDocument.IsActive = true;
                    lLayoutDocument.IsSelected = true;
                }
            });
        }

        private void OpenClientDocument(string pButtonContent)
        {
            LayoutDocumentPane? lLayoutDocumentPane = _MainWindow.MenuPage.Dock.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            lock (_lock)
            {
                if (_Documents.Count > 0)
                {
                    for (int i = 0; i < _Documents.Count; i++)
                    {
                        _Documents[i].Close();
                    }
                }
            }

            ClientViewModel lClientViewModel = UserViewModel.Instance.Clients.First(cl => cl.Client.ConnectionId == pButtonContent);

            // Crée un LayoutDocument
            LayoutDocument lLayoutDocument = new LayoutDocument
            {
                Title = "Jobs for " + pButtonContent,
                ContentId = pButtonContent
            };

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(new JobRunningControl(lClientViewModel));
            stackPanel.Children.Add(new JobListControl(lClientViewModel));
            // Affecte le content
            lLayoutDocument.Content = stackPanel;
            lLayoutDocument.IsActive = true;
            lLayoutDocument.IsSelected = true;
            lLayoutDocumentPane.Children.Add(lLayoutDocument);
            lLayoutDocument.Closed += LLayoutDocument_Closed;
            _Documents.Add(lLayoutDocument);
            lLayoutDocument.Float();

        }

        private void LLayoutDocument_Closed(object? sender, EventArgs e)
        {
            _Documents.Remove(sender as LayoutDocument);
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

    }
}
