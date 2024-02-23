using EasySaveGUI.ViewModels;
using EasySaveGUI.Views;
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
            _MainWindow.RefreshMenu();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _MainWindow.MenuPage.ClearLists();
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
