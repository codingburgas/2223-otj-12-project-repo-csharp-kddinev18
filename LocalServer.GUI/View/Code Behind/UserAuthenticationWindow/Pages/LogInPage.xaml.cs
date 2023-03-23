using LocalServer.BLL.DataManipulation.BLL;
using LocalServer.GUI.Models;
using LocalServerGUI.View.Code_Behind.UserAuthenticationWindow;
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

namespace LocalServerGUI.Code_Behind.XAML.UserAuthenticationWindow.Pages
{
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        private UsersAuthenticationWindow _userAuthenticationWindow;
        public LogInPage(UsersAuthenticationWindow userAuthenticationWindow)
        {
            _userAuthenticationWindow = userAuthenticationWindow;
            InitializeComponent();
        }

        //Event handlers
        private void OpenRegistrationFormButton_Click(object sender, RoutedEventArgs e)
        {
            // Shows RegistrationPage
            _userAuthenticationWindow.ShowPage(_userAuthenticationWindow.RegistrationPage);
        }
        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            // Log in and sets CurrentUserId to the logged user id
            CurrentUserInformation.UserId = UserAuthenticationLogic.LogIn(UserName.TextBox.Text, PasswordTextBox.Password);
            CurrentUserInformation.IsAdmin = UserAuthenticationLogic.IsAdmin(CurrentUserInformation.UserId);
            _userAuthenticationWindow.ShowMainWindow();
        }
    }
}
