using EasySaveGUI.ViewModels;
using Gtk;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Grid = System.Windows.Controls.Grid;
using Style = System.Windows.Style;

namespace EasySaveGUI.UserControls
{
    /// <summary>
    /// Interaction logic for ConnectionMenuControl.xaml
    /// </summary>
    public partial class ConnectionMenuControl : UserControl
    {
        public ConnectionMenuControl()
        {
            InitializeComponent();
        }

        public void UpdateListClients(ObservableCollection<CClient> pClients)
        {
            VerticalMenu.Children.Clear();
            HorizontalMenu.Children.Clear();
            
            for (int i = 0; i < pClients.Count; i++)
            {
                ColumnDefinition lCol = new ColumnDefinition();
                RowDefinition lRow = new RowDefinition();
                HorizontalMenu.ColumnDefinitions.Add(lCol);
                VerticalMenu.RowDefinitions.Add(lRow);

                Button lButtonHorizontal = new Button();
                lButtonHorizontal.Style = (Style)Application.Current.FindResource("CustomButtonJobs");
                Grid.SetColumn(lButtonHorizontal, i);
                lButtonHorizontal.Content = pClients[i].ConnectionId;

                Button lButton = new Button();
                lButton.Style = (Style)Application.Current.FindResource("CustomButtonJobs");
                lButton.Content = pClients[i].ConnectionId;
                Grid.SetRow(lButton, i);

                VerticalMenu.Children.Add(lButton);
                HorizontalMenu.Children.Add(lButtonHorizontal);
            }
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
