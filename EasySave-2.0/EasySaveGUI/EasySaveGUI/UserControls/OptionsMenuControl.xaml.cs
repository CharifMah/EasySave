using System.Windows;
using System.Windows.Controls;
using ViewModels;

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
            _MainWindow.MenuPage.Dock.UpdateLayout();
        }
    }
}
