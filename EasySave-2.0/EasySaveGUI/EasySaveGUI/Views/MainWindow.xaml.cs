using AvalonDock.Layout;
using AvalonDock.Themes;
using EasySaveGUI.Views;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModels;

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
        private MainViewModel _MainVm;
        private MenuPage _MenuPage;
        public MainViewModel MainVm { get => _MainVm; set => _MainVm = value; }
        public MenuPage MenuPage { get => _MenuPage; set => _MenuPage = value; }


        public MainWindow()
        {
            InitializeComponent();
            _MainVm = new MainViewModel();
            DataContext = _MainVm;
            _MenuPage = new MenuPage(_MainVm);
            frame.Navigate(_MenuPage);
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

        private void ComboBox_MouseEnter(object sender, MouseEventArgs e)
        {
            ComboBox lComboBox = sender as ComboBox;

            lComboBox.IsDropDownOpen = true;
        }

        private void ComboBox_MouseLeave(object sender, MouseEventArgs e)
        {
            ComboBox lComboBox = sender as ComboBox;

            lComboBox.IsDropDownOpen = false;
        }

        private void ComboBoxLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _MainVm.LangueVm.SetLanguage(e.AddedItems[0].ToString()[0..2]);
                _MenuPage = new MenuPage(_MainVm);
                // cm - Recharger la page
                frame.Navigate(_MenuPage);
            }
        }

        private void Vs2013BlueThemeButton_Click(object sender, RoutedEventArgs e)
        {
            _MenuPage.Dock.Theme = new Vs2013BlueTheme();
        }

        private void Vs2013LightThemeButton_Click(object sender, RoutedEventArgs e)
        {
            _MenuPage.Dock.Theme = new Vs2013LightTheme();
        }

        private void GenericThemeButton_Click(object sender, RoutedEventArgs e)
        {
            _MenuPage.Dock.Theme = new AvalonDock.Themes.GenericTheme();
        }
    }
}
