using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for ValidationAnimationControl.xaml
    /// </summary>
    public partial class LoadingAnimationControl : UserControl
    {
        public LoadingAnimationControl()
        {
            InitializeComponent();
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
