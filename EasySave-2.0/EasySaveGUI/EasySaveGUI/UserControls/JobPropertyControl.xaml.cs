using Models.Backup;
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
using ViewModels;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for JobProperty.xaml
    /// </summary>
    public partial class JobPropertyControl : UserControl
    {
        private MainViewModel _MainVm;
        private MainWindow _MainWindow;
        public JobPropertyControl()
        {
            InitializeComponent();
            _MainWindow = (Window.GetWindow(App.Current.MainWindow) as MainWindow);
            _MainVm = _MainWindow.MainVm;

        }

        private void TextBoxSourceDirectory_Error(object sender, ValidationErrorEventArgs e)
        {
            _MainVm.PopupVm.Message = e.Error.ErrorContent.ToString();
            _MainWindow.MenuPage.PopupError.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
