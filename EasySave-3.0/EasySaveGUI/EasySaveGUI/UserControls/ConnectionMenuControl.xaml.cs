﻿using AvalonDock.Layout;
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
        private MainWindow? _MainWindow;
        private MainViewModel _MainVm;
        public ConnectionMenuControl()
        {
            InitializeComponent();

            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
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


                if (UserViewModel.Instance.ClientViewModel.Client.ConnectionId == pClients[i].Client.ConnectionId)
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
            ClientViewModel lClientViewModel = UserViewModel.Instance.Clients.First(cl => cl.Client.ConnectionId == button.Content.ToString());

            LayoutDocument layoutDocumentPane = new LayoutDocument();

            LayoutAnchorablePane jobsPaneGroup = new LayoutAnchorablePane();
            jobsPaneGroup.FloatingHeight = 10;
            var jobsListAnchor = new LayoutAnchorable();
            jobsListAnchor.Title = Strings.Jobs + " " + button.Content.ToString();

            jobsListAnchor.ContentId = "JobsListDocument";
            JobListControl lJobListControl = new JobListControl();
            lJobListControl.DataContext = lClientViewModel;
            jobsListAnchor.Content = new JobListControl();
            jobsListAnchor.CanClose = true;
            jobsPaneGroup.Children.Add(jobsListAnchor);

            var jobsRunningAnchor = new LayoutAnchorable();
            jobsRunningAnchor.Title = Strings.JobsRunning + " " + button.Content.ToString();
            jobsRunningAnchor.ContentId = "JobsRunningDocument";
            JobRunningControl lJobRunningControl = new JobRunningControl();
            lJobRunningControl.DataContext = lClientViewModel;
            jobsRunningAnchor.Content = lJobRunningControl;
            jobsRunningAnchor.CanClose = true;
            jobsPaneGroup.Children.Add(jobsRunningAnchor);

            layoutDocumentPane.Content = jobsPaneGroup;
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
