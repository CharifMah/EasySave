﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for ValidationAnimationControl.xaml
    /// </summary>
    public partial class ValidationAnimationControl : UserControl
    {
        public ValidationAnimationControl()
        {
            InitializeComponent();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {

            Hide();

        }

        public void Hide()
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                MainGrid.Visibility = Visibility.Hidden;
            });
        }

        public void Show()
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                MainGrid.Visibility = Visibility.Visible;
                StoryBoard.Begin();
            });
        }
    }
}
