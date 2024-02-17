using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            this.MainGrid.Visibility = Visibility.Hidden;
        }

        public void Show()
        {
            this.MainGrid.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
