using AvalonDock.Themes;
using EasySaveGUI.ViewModels;
using EasySaveGUI.Views;
using LogsModels;
using Models.Settings;
using Models.Settings.Theme;
using Stockage.Logs;
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

        #region Attributes
        private MainViewModel _MainVm;
        private MenuPage _MenuPage;
        #endregion

        #region Property
        public MainViewModel MainVm { get => _MainVm; set => _MainVm = value; }
        public MenuPage MenuPage { get => _MenuPage; set => _MenuPage = value; }
        #endregion

        #region CTOR
        public MainWindow()
        {
            InitializeComponent();
            _MainVm = new MainViewModel();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DataContext = _MainVm;
            RefreshMenu();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Rafraîchie le menu avec le layout sélectionnée
        /// </summary>
        /// <param name="pSetLayout">false pour reset le layout</param>
        public void RefreshMenu(bool pSetLayout = true)
        {
            _MenuPage = new MenuPage(_MainVm);
            frame.NavigationService.Navigate(_MenuPage);
            if (pSetLayout)
                SetLayout(_MainVm.SettingsVm.CurrentLayout);
            else
                _MainVm.SettingsVm.ResetCurrentLayout();
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
            {
                WindowState = WindowState.Maximized;
            }
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
                RefreshMenu();
            }
        }

        private void Vs2013BlueThemeButton_Click(object sender, RoutedEventArgs e)
        {

            BlueTheme();

        }

        private void Vs2013LightThemeButton_Click(object sender, RoutedEventArgs e)
        {

            LightTheme();

        }

        private void GenericThemeButton_Click(object sender, RoutedEventArgs e)
        {

            DarkTheme();

        }
        #endregion

        #region Theme/Layout
        private void ComboBoxLayout_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox lComboBox = sender as ComboBox;
            if (lComboBox.SelectedValue != null)
            {
                string lSelectedValue = lComboBox.SelectedValue.ToString();
                SetLayout(lSelectedValue);
            }
        }

        private void SetLayout(string pSelectedValue)
        {
            if (!string.IsNullOrEmpty(pSelectedValue))
            {
                _MainVm.LayoutVm.LoadLayout(_MenuPage.Dock, pSelectedValue);

                _MainVm.SettingsVm.CurrentLayout = pSelectedValue;

                if (CSettings.Instance.Theme.LayoutsTheme.ContainsKey(pSelectedValue))
                {
                    SetTheme(CSettings.Instance.Theme.LayoutsTheme[pSelectedValue]);
                }

                CSettings.Instance.SaveSettings();
            }
        }

        private void SetTheme(ETheme pTheme)
        {
            switch (pTheme)
            {
                case ETheme.LIGHT:
                    LightTheme();
                    break;
                case ETheme.DARK:
                    DarkTheme();
                    break;
                case ETheme.BLUE:
                    BlueTheme();
                    break;
            }

            CSettings.Instance.Theme.CurrentTheme = pTheme;
        }

        private void BlueTheme()
        {
            try
            {
                _MenuPage.Dock.Theme = new Vs2013BlueTheme();
                CSettings.Instance.Theme.CurrentTheme = ETheme.BLUE;
                _MenuPage.Dock.Background = Brushes.White;
                _MenuPage.Resources["TextColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2D30"));
                _MenuPage.Resources["GenericBackground"] = Brushes.White;
                _MenuPage.Resources["LightGray"] = Brushes.LightBlue;
                _MenuPage.Resources["ButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CEE6FD"));
                _MenuPage.Resources["LightDark"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4D4D4D"));
                _MenuPage.Resources["HoverColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                _MenuPage.Resources["LoadingBarColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9FFF73"));
                _MenuPage.Resources["DarkPinkColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B86"));
                _MenuPage.Resources["PinkColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E1BFFF"));
                _MenuPage.Resources["LightBlue"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B1D4F6"));
                _MenuPage.Resources["LightGreenColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7DC777"));
                CSettings.Instance.SaveSettings();
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }
        }

        private void LightTheme()
        {
            try
            {
                _MenuPage.Dock.Theme = new Vs2013LightTheme();
                CSettings.Instance.Theme.CurrentTheme = ETheme.LIGHT;
                _MenuPage.Dock.Background = Brushes.White;
                _MenuPage.Resources["TextColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2D30"));
                _MenuPage.Resources["GenericBackground"] = Brushes.White;
                _MenuPage.Resources["LightGray"] = Brushes.LightGray;
                _MenuPage.Resources["LightDark"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4D4D4D"));
                _MenuPage.Resources["ButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                _MenuPage.Resources["HoverColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                _MenuPage.Resources["LoadingBarColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9FFF73"));
                _MenuPage.Resources["DarkPinkColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B86"));
                _MenuPage.Resources["PinkColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E1BFFF"));
                _MenuPage.Resources["LightBlue"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B1D4F6"));
                _MenuPage.Resources["LightGreenColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7DC777"));
                CSettings.Instance.SaveSettings();
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }
        }

        private void DarkTheme()
        {
            try
            {
                _MenuPage.Dock.Theme = new AvalonDock.Themes.Vs2013DarkTheme();
                CSettings.Instance.Theme.CurrentTheme = ETheme.DARK;
                _MenuPage.Dock.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2D30"));
                _MenuPage.Resources["TextColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ebeef2"));
                _MenuPage.Resources["GenericBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F2D30"));
                _MenuPage.Resources["LightGray"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#111112"));
                _MenuPage.Resources["ButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#424242"));
                _MenuPage.Resources["LightDark"] = Brushes.White;
                _MenuPage.Resources["HoverColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757f86"));
                _MenuPage.Resources["LoadingBarColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D8A1D"));
                _MenuPage.Resources["DarkPinkColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6E1946"));
                _MenuPage.Resources["PinkColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C76F9D"));
                _MenuPage.Resources["LightBlue"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#245889"));
                _MenuPage.Resources["LightGreenColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#248955"));

                CSettings.Instance.SaveSettings();
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }
        }
        #endregion

        #endregion
    }
}
