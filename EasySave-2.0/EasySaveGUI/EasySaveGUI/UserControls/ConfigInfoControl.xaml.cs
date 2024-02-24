using EasySaveGUI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for ConfigInfoControl.xaml
    /// </summary>
    public partial class ConfigInfoControl : UserControl
    {
        private MainViewModel _MainVm;
        private MainWindow _MainWindow;
        public ConfigInfoControl()
        {
            InitializeComponent();
            _MainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            _MainVm = _MainWindow.MainVm;
        }

        private void ComboBoxFormatLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _MainVm.FormatLogVm.SetFormatLog(e.AddedItems[0].ToString());
            }
        }
    }
}
