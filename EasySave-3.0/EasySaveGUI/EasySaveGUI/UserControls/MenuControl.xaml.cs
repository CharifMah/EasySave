﻿using AvalonDock.Layout;
using EasySaveGUI.ViewModels;
using Ressources;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for MenuControl.xaml
    /// </summary>
    public partial class MenuControl : UserControl
    {
        private MainWindow _MainWindow;
        private MainViewModel _MainVm;
        public MenuControl()
        {
            InitializeComponent();
            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
        }

        private void MenuButtons_MouseClick(object sender, RoutedEventArgs e)
        {
            Button lButton = sender as Button;
            if (lButton.Content == Strings.Settings)
            {
                _MainVm.LayoutVm.ElementsContent.Content = new ConfigMenuControl();
                LayoutAnchorable? lLayoutAnchorable = _MainWindow.MenuPage.Dock.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(l => l.ContentId.Contains(Strings.Config));
                lLayoutAnchorable.IsSelected = true;
                lLayoutAnchorable.IsActive = true;
            }
            if (lButton.Content == Strings.Preference)
                _MainVm.LayoutVm.ElementsContent.Content = new OptionsMenuControl();
            if (lButton.Content == Strings.Jobs)
            {
                _MainVm.LayoutVm.ElementsContent.Content = new JobMenuControl();
                LayoutAnchorable? lLayoutAnchorable = _MainWindow.MenuPage.Dock.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(l => l.ContentId.Contains(Strings.Jobs));
                lLayoutAnchorable.IsSelected = true;
                lLayoutAnchorable.IsActive = true;
            }

            _MainWindow.MenuPage.ListElements.Show();
            _MainWindow.MenuPage.ListElements.IsActive = true;
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

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            _MainVm.LayoutVm.ElementsContent.Content = _MainVm.ConnectionMenuControl;

            await UserViewModel.Instance.ConnectLobby(_MainVm.JobVm);
            _MainVm.ConnectionMenuControl.UpdateListClients(UserViewModel.Instance.Clients);

            _MainWindow.MenuPage.ListElements.Show();
            _MainWindow.MenuPage.ListElements.IsActive = true;
        }
    }
}
