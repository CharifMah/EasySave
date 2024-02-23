using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Themes;
using EasySaveGUI.ViewModels;
using EasySaveGUI.Views;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        #region TitleBarReleaseCapture
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
        #endregion

        #region TitleBarEvents
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
            BlueTheme();
        }

        private void Vs2013LightThemeButton_Click(object sender, RoutedEventArgs e)
        {
            _MenuPage.Dock.Theme = new Vs2013LightTheme();
            LightTheme();
        }

        private void GenericThemeButton_Click(object sender, RoutedEventArgs e)
        {
            _MenuPage.Dock.Theme = new AvalonDock.Themes.Vs2013DarkTheme();
            DarkTheme();
        }
        #endregion


        private void BlueTheme()
        {
            _MenuPage.Dock.Background = Brushes.White;
            _MenuPage.Resources["TextColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2D30"));
            _MenuPage.Resources["GenericBackground"] = Brushes.White;
            _MenuPage.Resources["LightGray"] = Brushes.LightBlue;
            _MenuPage.Resources["ButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CEE6FD"));
            _MenuPage.Resources["LightDark"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4D4D4D"));
            _MenuPage.Resources["HoverColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
        }

        private void DarkTheme()
        {
            _MenuPage.Dock.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2D30"));
            _MenuPage.Resources["TextColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ebeef2")); 
            _MenuPage.Resources["GenericBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2D30"));
            _MenuPage.Resources["LightGray"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#111112"));
            _MenuPage.Resources["ButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#424242"));
            _MenuPage.Resources["LightDark"] = Brushes.White;
            _MenuPage.Resources["HoverColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757f86"));
        }

        private void LightTheme()
        {
            _MenuPage.Dock.Background = Brushes.White;
            _MenuPage.Resources["TextColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2D30"));
            _MenuPage.Resources["GenericBackground"] = Brushes.White;
            _MenuPage.Resources["LightGray"] = Brushes.LightGray;
            _MenuPage.Resources["LightDark"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4D4D4D"));
            _MenuPage.Resources["ButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
            _MenuPage.Resources["HoverColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
        }

        public void RefreshMenu(DockingManager pDockingManager = null)
        {
            _MenuPage = new MenuPage(_MainVm);
            frame.NavigationService.Navigate(_MenuPage, pDockingManager);
        }

        private void ComboBoxLayout_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox lComboBox = sender as ComboBox;
           _MainVm.LayoutVm.LoadLayout(_MenuPage.Dock,lComboBox.SelectedValue.ToString());
        }
    }
}
