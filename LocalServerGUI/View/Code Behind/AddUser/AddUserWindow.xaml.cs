using LocalServerGUI.View.Code_Behind.MainWindow.Pages;
using LocalServerLogic;
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

namespace LocalServerGUI.View.Code_Behind.AddUser
{
    /// <summary>
    /// Interaction logic for AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        private bool _isMaximized = false;
        private UsersPage _usersPage;

        public static bool isOpened = false;
        public AddUserWindow(UsersPage usersPage)
        {
            _usersPage = usersPage;
            InitializeComponent();
            isOpened = true;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Register the user into the database
                UserAuthenticationLogic.RegisterMember(UserName.TextBox.Text, Email.TextBox.Text, PasswordTextBox.Password, Role.TextBox.Text);
                _usersPage.UpdateDataGrid(1);
                isOpened = false;
                this.Close();
            }
            catch (Exception exception)
            {
                // Show error message box
                MessageBox.Show(exception.Message, "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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
