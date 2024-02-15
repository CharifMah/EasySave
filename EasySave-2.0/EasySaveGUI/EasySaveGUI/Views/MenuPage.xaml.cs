using System.Windows.Controls;
using ViewModels;

namespace EasySaveGUI.Views
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage(MainViewModel pMainVm)
        {
            InitializeComponent();

            JobDataGrid.DataContext = pMainVm.JobVm;
        }
    }
}
