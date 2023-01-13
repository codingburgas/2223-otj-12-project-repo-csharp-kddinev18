using LocalServerBusinessLogic;
using LocalServerGUI.Models;
using LocalServerGUI.View.Code_Behind.AddRole;
using LocalServerGUI.View.Code_Behind.AddUser;
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
    /// Interaction logic for RolesPage.xaml
    /// </summary>
    public partial class RolesPage : Page
    {
        // A collection that updates both ways (form the view and code behind)
        private ObservableCollection<RoleBindingInformation> _rolesInformation;
        // The count of the vacations in the database
        private int _rolesCount;
        // The paging size
        private int _pagingSize = 10;
        // The number of pages
        private int _numberOfPages;
        // The page we are on
        private int _pageIndex = 0;
        // The amount of viewed vacations
        private int _sikpAmount = 0;
        public RolesPage()
        {
            InitializeComponent();
            // Checks if the user is admin, if he isn't disable the AddMembersButton
            if (!CurrentUserInformation.IsAdmin)
            {
                // Disable the AddMembersButton
                AddRolesButton.IsEnabled = false;
            }

            // Get the count of the users
            _rolesCount = RoleModificationLogic.GetRolesCount();
            // Devide the teams count to the paging size to see how many pages are there
            _numberOfPages = (int)Math.Ceiling((double)_rolesCount / _pagingSize);

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
            _rolesCount += i;
            // Devide the vacations count to the paging size to see how many pages are there
            _numberOfPages = (int)Math.Ceiling((double)_rolesCount / _pagingSize);

            // Get the users from the database
            List<RoleInformation> rolesInformation = RoleModificationLogic.GetRolesInformation(_pagingSize, _sikpAmount);
            _rolesInformation = new ObservableCollection<RoleBindingInformation>();
            Random r = new Random();
            foreach (RoleInformation roleInformation in rolesInformation)
            {
                _rolesInformation.Add(new RoleBindingInformation(roleInformation)
                {
                    // Assign the bachground color for the icon
                    BgColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255))),
                    // Assign the inital of the icon
                    Initials = roleInformation.Name.Substring(0, 1),
                    // If the user is admin enable the edit button, otherwise disable it
                    EditButton = CurrentUserInformation.IsAdmin && roleInformation.Name != "Admin",
                    // If the user is admin enable the remove button, otherwise disable it
                    RemoveButton = CurrentUserInformation.IsAdmin && roleInformation.Name != "Admin"
                });
            }
            // Assign the datagrid the collection
            RolesDataGrid.ItemsSource = _rolesInformation;
        }
        public void UpdateDataGrid(string filter)
        {
            // Canges the count of the teams based on the argument i {-1;0;1}
            _rolesCount = 1;
            // Devide the vacations count to the paging size to see how many pages are there
            _numberOfPages = (int)Math.Ceiling((double)_rolesCount / _pagingSize);

            // Get the users from the database
            List<RoleInformation> rolesInformation = RoleModificationLogic.GetRolesInformation(filter ,_pagingSize, _sikpAmount);
            _rolesInformation = new ObservableCollection<RoleBindingInformation>();
            Random r = new Random();
            foreach (RoleInformation roleInformation in rolesInformation)
            {
                _rolesInformation.Add(new RoleBindingInformation(roleInformation)
                {
                    // Assign the bachground color for the icon
                    BgColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255))),
                    // Assign the inital of the icon
                    Initials = roleInformation.Name.Substring(0, 1),
                    // If the user is admin enable the edit button, otherwise disable it
                    EditButton = CurrentUserInformation.IsAdmin && roleInformation.Name != "Admin",
                    // If the user is admin enable the remove button, otherwise disable it
                    RemoveButton = CurrentUserInformation.IsAdmin && roleInformation.Name != "Admin"
                });
            }
            // Assign the datagrid the collection
            RolesDataGrid.ItemsSource = _rolesInformation;
        }
        // Event handlers
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Update the datagrid
            UpdateDataGrid(0);
        }

        private void KeyDown_Filter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                UpdateDataGrid(Filter.TextBox.Text);
            }
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
        // Invoked every time the AddMembersButton is clicked
        private void AddRolesButton_Click(object sender, RoutedEventArgs e)
        {
            // If the AddMemberWindow isn't opened, oped it, otherwise do nothing
            if (AddRoleWindow.isOpened == false)
            {
                AddRoleWindow addRoleWindow = new AddRoleWindow(this);
                addRoleWindow.Show();
            }
        }

        // Invoked every time the EditButton is clicked
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the row the user clickd on
            RoleBindingInformation dataRow = (RoleBindingInformation)RolesDataGrid.SelectedItem;
            // Edit a uesr
            RoleModificationLogic.EditRole(dataRow.RoleId, dataRow.Name);

            // Update the grid
            UpdateDataGrid(0);

        }

        // Invoked every time the RemoveButton is clicked
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the row the user clickd on
            RoleBindingInformation dataRow = (RoleBindingInformation)RolesDataGrid.SelectedItem;
            // Remove the user
            RoleModificationLogic.RemoveRole(dataRow.RoleId);
            // Update the grid
            UpdateDataGrid(-1);
        }
    }
}
