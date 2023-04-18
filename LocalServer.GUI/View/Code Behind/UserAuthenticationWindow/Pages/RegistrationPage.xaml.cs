using LocalServer.BLL.DataManipulation.BLL;
using LocalServer.GUI.Models;
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

namespace LocalServerGUI.View.Code_Behind.UserAuthenticationWindow.Pages
{
    /// <summary>
    /// Interaction logic for RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        private UsersAuthenticationWindow _userAuthenticationWindow;
        public RegistrationPage(UsersAuthenticationWindow userAuthenticationWindow)
        {
            _userAuthenticationWindow = userAuthenticationWindow;
            InitializeComponent();
        }

        private void OpenLogInFormButton_Click(object sender, RoutedEventArgs e)
        {
            // Show LogInPage
            _userAuthenticationWindow.ShowPage(_userAuthenticationWindow.LogInPage);
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Register and set the CurrentUserId
                CurrentUserInformation.UserId = UserAuthenticationLogic.Register(UserName.TextBox.Text, Email.TextBox.Text, PasswordTextBox.Password);
                CurrentUserInformation.IsAdmin = UserAuthenticationLogic.IsAdmin(CurrentUserInformation.UserId);
                _userAuthenticationWindow.ShowPage(_userAuthenticationWindow.BridgeAPILoginPage);
            }
            catch (Exception exception)
            {
                // Show error message box
                MessageBox.Show(exception.Message, "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
