using LocalServer.BLL.DataManipulation.BLL;
using LocalServer.BLL.Server.BLL;
using LocalServer.GUI.Models;
using LocalServerGUI.View.Code_Behind.MainWindow.Pages;
using LocalServerGUI.View.Code_Behind.UserAuthenticationWindow;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalServer.GUI.View.Code_Behind.UserAuthenticationWindow.Pages
{
    /// <summary>
    /// Interaction logic for BridgeAPILoginPage.xaml
    /// </summary>
    public partial class BridgeAPILoginPage : Page
    {
        private UsersAuthenticationWindow _userAuthenticationWindow;
        public BridgeAPILoginPage(UsersAuthenticationWindow userAuthenticationWindow)
        {
            _userAuthenticationWindow = userAuthenticationWindow;
            InitializeComponent();
        }

        //Event handlers
        private void OpenRegistrationFormButton_Click(object sender, RoutedEventArgs e)
        {
            // Shows RegistrationPage
            // show url
        }
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BridgeAPIHandlingLogic.SetUpConnection(UserName.TextBox.Text, PasswordTextBox.Password);
                Task.Run(() => BridgeAPIHandlingLogic.AwaitServerCall());
                _userAuthenticationWindow.ShowMainWindow();
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
