using EasySaveGUI.Views;
using System.Windows;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            frame.Navigate(new MenuPage(new ViewModels.MainViewModel()));
        }
    }
}
