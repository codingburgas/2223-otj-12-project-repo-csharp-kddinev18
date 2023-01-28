using LocalServerBusinessLogic;
using LocalServerGUI.Models;
using LocalServerModels;
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

namespace LocalServerGUI.View.Code_Behind.MainWindow.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        ObservableCollection<UserInformation> _usersInformation;
        ObservableCollection<DeviceInformation> _deviceInformation;
        public HomePage()
        {
            InitializeComponent();
            UpdateDataGrid();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void UpdateDataGrid()
        {
            // Get the users from the database
            _usersInformation = new ObservableCollection<UserInformation>(UserModifierLogic.GetUsersInformation(5, 0));
            _deviceInformation = new ObservableCollection<DeviceInformation>(DeviceModificationLogic.GetDevicesInformation(5, 0));
            UsersDataGrid.ItemsSource = _usersInformation;
            DevicesDataGrid.ItemsSource = _deviceInformation;
        }
    }
}
