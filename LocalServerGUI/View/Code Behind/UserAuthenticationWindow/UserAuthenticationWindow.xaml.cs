using LocalServerBusinessLogic;
using LocalServerGUI.Code_Behind.XAML.UserAuthenticationWindow.Pages;
using LocalServerGUI.View.Code_Behind.UserAuthenticationWindow.Pages;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalServerGUI.View.Code_Behind.UserAuthenticationWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UsersAuthenticationWindow : Window
    {
        public LogInPage LogInPage { get; set; }
        public RegistrationPage RegistrationPage { get; set; }

        public DatabaseIntialiser DatabaseIntialiser { get; set; }
        public ServerLogic Server { get; set; }
        public UserAuthenticationLogic UserAuthentication { get; set; }
        public UsersAuthenticationWindow()
        {
            try
            {
                DatabaseIntialiser = new DatabaseIntialiser(200 * 60 * 1000);
                Server = new ServerLogic(5400, new ClientHandlingLogic(DatabaseIntialiser));
                Server.ServerSetUp(200 * 60 * 1000);
                UserAuthentication = new UserAuthenticationLogic(DatabaseIntialiser);

                LogInPage = new LogInPage(this);
                RegistrationPage = new RegistrationPage(this);

                InitializeComponent();
                ShowPage(LogInPage);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
        }
        public void ShowPage(Page page)
        {
            // Change the content of the form with the specific page
            Forms.Content = page;
        }
        public void ShowMainWindow()
        {
            // Instantiates a new window
            
            // Show the new window

            // Close this window
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Drag the window of the button is hold
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Shutdown the application
            Application.Current.Shutdown();
        }
    }
}
