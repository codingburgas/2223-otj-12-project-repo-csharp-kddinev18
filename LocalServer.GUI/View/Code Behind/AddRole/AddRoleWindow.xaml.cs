using LocalServer.BLL;
using LocalServerGUI.View.Code_Behind.MainWindow.Pages;
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
using System.Windows.Shapes;

namespace LocalServerGUI.View.Code_Behind.AddRole
{
    /// <summary>
    /// Interaction logic for AddRoleWindow.xaml
    /// </summary>
    public partial class AddRoleWindow : Window
    {
        private bool _isMaximized = false;
        private RolesPage _rolesPage;

        public static bool isOpened = false;
        public AddRoleWindow(RolesPage rolesPage)
        {
            _rolesPage = rolesPage;
            InitializeComponent();
            isOpened = true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            RoleModificationLogic.AddRole(Role.TextBox.Text);
            _rolesPage.UpdateDataGrid(1);
        }
        // Invoke every time the CancelButton is clicked
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            isOpened = false;
            this.Close();
        }

        // Invoke every time the user clicks on the window
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Cecks if the button pressed is the left button
            if (e.ChangedButton == MouseButton.Left)
            {
                // Drag the window with the button
                this.DragMove();
            }
        }

        // Invoke every time the user clicks on the window
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Checks if the click count was 2
            if (e.ClickCount == 2)
            {
                // If the window is maximised, minimise it
                if (_isMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1080;
                    this.Height = 720;

                    _isMaximized = false;
                }
                // Otheewise maximise it
                else
                {
                    this.WindowState = WindowState.Maximized;

                    _isMaximized = true;
                }
            }
        }
    }
}
