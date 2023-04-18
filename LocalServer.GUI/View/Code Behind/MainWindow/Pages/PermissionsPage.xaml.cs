using LocalServer.BLL.DataManipulation.BLL;
using LocalServer.DTO.Models;
using LocalServer.GUI.Models;
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
    /// Interaction logic for PermissionsPage.xaml
    /// </summary>
    public partial class PermissionsPage : Page
    {
        // A collection that updates both ways (form the view and code behind)
        private ObservableCollection<PermissionBindingInformation> _permissionsInformation;
        // The count of the vacations in the database
        private int _permissionsCount;
        // The paging size
        private int _pagingSize = 10;
        // The number of pages
        private int _numberOfPages;
        // The page we are on
        private int _pageIndex = 0;
        // The amount of viewed vacations
        private int _sikpAmount = 0;
        public PermissionsPage()
        {
            InitializeComponent();

            // Get the count of the users
            _permissionsCount = PermissionModifierLogic.GetPermissionsCount();
            // Devide the teams count to the paging size to see how many pages are there
            _numberOfPages = (int)Math.Ceiling((double)_permissionsCount / _pagingSize);

            // Updates the grid
            UpdateDataGrid(0);

            // Disable the PrevButton
            PrevButton.IsEnabled = false;
            // If the number of pagis is less or equal to 1 disable the NextButton
            if (_numberOfPages <= 1)
            {
                NextButton.IsEnabled = false;
            }
        }
        public void UpdateDataGrid(int i)
        {
            // Canges the count of the teams based on the argument i {-1;0;1}
            _permissionsCount += i;
            // Devide the vacations count to the paging size to see how many pages are there
            _numberOfPages = (int)Math.Ceiling((double)_permissionsCount / _pagingSize);

            // Get the users from the database
            List<PermissionInformation> permissionsInformation = PermissionModifierLogic.GetPermissionInformation(_pagingSize, _sikpAmount);
            _permissionsInformation = new ObservableCollection<PermissionBindingInformation>();
            Random r = new Random();
            foreach (PermissionInformation permissionInformation in permissionsInformation)
            {
                _permissionsInformation.Add(new PermissionBindingInformation(permissionInformation)
                {
                    // If the user is admin enable the edit button, otherwise disable it
                    EditButton = CurrentUserInformation.IsAdmin,
                });
            }
            // Assign the datagrid the collection
            PermissionsDataGrid.ItemsSource = _permissionsInformation;
        }
        // Event handlers
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Update the datagrid
            UpdateDataGrid(0);
        }

        // Invoked every time the PrevButton is clicked
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            // Enable the NextButton
            NextButton.IsEnabled = true;

            // Decrease the page index
            _pageIndex--;
            // If the page index is 0 diable the PrevButton
            if (_pageIndex == 0)
                PrevButton.IsEnabled = false;

            // Decease the amount of skippings
            _sikpAmount -= _pagingSize;
            // Update the datagrid
            UpdateDataGrid(0);
        }
        // Invoked every time the NextButton is clicked
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Enable the PrevButton
            PrevButton.IsEnabled = true;

            // Increase the page index 
            _pageIndex++;
            // If the page index is equal to the amount of pages disable NextButton
            if (_pageIndex == _numberOfPages - 1)
                NextButton.IsEnabled = false;

            // Increase the amount of skippings
            _sikpAmount += _pagingSize;
            // Update the datagrid
            UpdateDataGrid(0);
        }

        // Invoked every time the EditButton is clicked
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            LocalServerMainWindow.ShowAuthenticationWindow();

            // Get the row the user clickd on
            PermissionBindingInformation dataRow = (PermissionBindingInformation)PermissionsDataGrid.SelectedItem;
            // Edit a uesr
            PermissionModifierLogic.EditPermission(dataRow.RoleName, dataRow.DeviceName, dataRow.CanCreate, dataRow.CanRead, dataRow.CanUpdate, dataRow.CanDelete);

            // Update the grid
            UpdateDataGrid(0);

        }

        // Invoked every time the RemoveButton is clicked
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            LocalServerMainWindow.ShowAuthenticationWindow();

            // Get the row the user clickd on
            PermissionBindingInformation dataRow = (PermissionBindingInformation)PermissionsDataGrid.SelectedItem;

            // Remove the user
            PermissionModifierLogic.RemovePermission(dataRow.RoleName, dataRow.DeviceName);

            // Update the grid
            UpdateDataGrid(-1);
        }
    }
}
