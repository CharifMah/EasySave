﻿using Stockage.Logs;
using System.Windows;
using System.Windows.Controls;
using ViewModels;

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
    }
}
