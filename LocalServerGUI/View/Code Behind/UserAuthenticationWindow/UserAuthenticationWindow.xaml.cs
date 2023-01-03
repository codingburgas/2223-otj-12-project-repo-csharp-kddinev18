using LocalServerGUI.Code_Behind.XAML.UserAuthenticationWindow.Pages;
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
    public partial class UserAuthenticationWindow : Window
    {
        public LogInPage LogInPage { get; set; }
        public UserAuthenticationWindow()
        {
            InitializeComponent();
            LogInPage=new LogInPage();
            ShowPage(LogInPage);
        }

        public void ShowPage(Page page)
        {
            // Change the content of the form with the specific page
            Forms.Content = page;
        }
        public void ShowMainWindow()
        {

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
