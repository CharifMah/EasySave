using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for ValidationPopupControl.xaml
    /// </summary>
    public partial class ValidationPopupControl : UserControl
    {
        public ValidationPopupControl()
        {
            InitializeComponent();
        }

        public void Hide()
        {
            MainGrid.Visibility = Visibility.Hidden;
        }

        public void Show()
        {
            MainGrid.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
