using EasySaveGUI.Views;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region DLL
        // Required imports for window dragging
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            frame.Navigate(new MenuPage());
        }

        // Event handler for title bar mouse left button down event
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Start dragging the window
                ReleaseCapture();
                SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, 0xA1, 0x2, 0);
            }
        }

        // Event handler for title bar mouse move event
        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Continue dragging the window
                ReleaseCapture();
                SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, 0x112, 0xF012, 0);
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }
        }
    }
}
