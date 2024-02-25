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
    /// Interaction logic for ValidationAnimationControl.xaml
    /// </summary>
    public partial class ValidationAnimationControl : UserControl
    {
        public ValidationAnimationControl()
        {
            InitializeComponent();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            Hide();
        }

        public void Hide()
        {
            MainGrid.Visibility = Visibility.Hidden; 
        }

        public void Show()
        {
            MainGrid.Visibility = Visibility.Visible;
            StoryBoard.Begin();
        }
    }
}
