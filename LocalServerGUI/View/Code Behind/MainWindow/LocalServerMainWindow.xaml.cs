using LocalServerBusinessLogic;
using LocalServerGUI.Models;
using LocalServerGUI.View.Code_Behind.MainWindow.Pages;
using LocalServerGUI.View.Code_Behind.UserAuthenticationWindow;
using LocalServerLogic;
using LocalServerModels;
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

namespace LocalServerGUI.View.Code_Behind.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LocalServerMainWindow : Window
    {
        private bool _isMaximized = false;
        public ServerLogic Server { get; set; }

        public Lazy<UsersPage> UsersPage { get; set; }
        public Lazy<DevicesPage> DevicesPage { get; set; }
        public LocalServerMainWindow(ServerLogic server)
        {
            Server = server;
            InitializeComponent();
            UserBindingInformation userBindingInformation = new UserBindingInformation
                (UserModifierLogic.GetCurrentUserInformation(CurrentUserInformation.UserId));
            Random r = new Random();
            IconColor.Background = userBindingInformation.BgColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));
            IconText.Text = userBindingInformation.UserName.Substring(0, 1);
            Username.Text = userBindingInformation.UserName;
            Role.Text = userBindingInformation.Role;
            // Loading the members page intpo the memory and showing it
            UsersPage = new Lazy<UsersPage>();
            DevicesPage = new Lazy<DevicesPage>();
            ShowPage(UsersPage.Value);
        }
        // Shows a page
        public void ShowPage(Page page)
        {
            MainWindowFrame.Content = page;
        }
        // Event handlers
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove the server connection
            Server.ServerShutDown();
            // Shutdown the application
            Application.Current.Shutdown();
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
        // Invoked every time MembersButton is clicked
        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(UsersPage.Value);
        }

        // Invoked every time ProjectsButton is clicked
        private void DevicesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(DevicesPage.Value);
        }

        // Invoked every time TeamsButton is clicked
        private void TeamsButton_Click(object sender, RoutedEventArgs e)
        {
            //ShowPage(TeamsPage.Value);
        }

        // Invoked every time VacationsButton is clicked
        private void VacationsButton_Click(object sender, RoutedEventArgs e)
        {
            //ShowPage(VacationsPage.Value);
        }

        // Invoked every time LogOutButton is clicked
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            UsersAuthenticationWindow window = new UsersAuthenticationWindow();
            window.Show();
            this.Close();
        }
    }
}
