﻿using EasySaveGUI.ViewModels;
using Ressources;
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
            if (lButton.Content == Strings.Config)
                _MainVm.LayoutVm.ElementsContent.Content = new ConfigMenuControl();
            if (lButton.Content == Strings.Settings)
                _MainVm.LayoutVm.ElementsContent.Content = new OptionsMenuControl();
            if (lButton.Content == Strings.Jobs)
                _MainVm.LayoutVm.ElementsContent.Content = new JobMenuControl();
            _MainWindow.MenuPage.ListElements.Show();
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
