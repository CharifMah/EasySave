﻿using EasySaveGUI.ViewModels;
using Models.Settings;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for OptionsMenuControl.xaml
    /// </summary>
    public partial class OptionsMenuControl : UserControl
    {
        private MainWindow _MainWindow;
        private MainViewModel _MainVm;
        public OptionsMenuControl()
        {
            InitializeComponent();
            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
        }

        private void ApplyDefaultStyleButton_Click(object sender, RoutedEventArgs e)
        {
            _MainWindow.RefreshMenu(false);
            _MainWindow.MenuPage.ShowValidation();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _MainWindow.MenuPage.ClearLists();
            _MainWindow.MenuPage.ShowValidation();
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

        private void SaveLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            _MainVm.LayoutVm.SaveLayout(_MainWindow.MenuPage.Dock, CSettings.Instance.Theme.CurrentTheme);
            _MainWindow.MenuPage.ShowValidation();
        }
    }
}
