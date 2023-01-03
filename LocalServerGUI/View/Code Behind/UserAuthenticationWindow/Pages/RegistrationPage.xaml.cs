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
        private UsersAuthenticationWindow _userAuthentication;
        public RegistrationPage(UsersAuthenticationWindow userAuthentication)
        {
            _userAuthentication = userAuthentication;
            InitializeComponent();
        }

        private void OpenLogInFormButton_Click(object sender, RoutedEventArgs e)
        {
            // Show LogInPage
            _userAuthentication.ShowPage(_userAuthentication.LogInPage);
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Register and set the CurrentUserId
            // UserAuthentocationLogic.Register(_userAuthentication, UserName.TextBox.Text, Email.TextBox.Text, PasswordTextBox.Password);
        }
    }
}
