using LocalServer.BLL.DataManipulation.BLL;
using LocalServer.GUI.Models;
using LocalServerGUI.View.Code_Behind.MainWindow.Pages;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace LocalServer.GUI.View.Code_Behind.Authenticate
{
    /// <summary>
    /// Interaction logic for AuthenticateWindow.xaml
    /// </summary>
    public partial class AuthenticateWindow : Window
    {
        private bool _isMaximized = false;

        public static bool isOpened = false;
        public AuthenticateWindow()
        {
            InitializeComponent();
            isOpened = true;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Register the user into the database
                Guid id = UserAuthenticationLogic.LogIn(UserName.TextBox.Text, PasswordTextBox.Password);
                if(id != CurrentUserInformation.UserId)
                {
                    MessageBox.Show("Wrong credentials", "Wrong credentials", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    isOpened = false;
                    this.Close();
                }
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
