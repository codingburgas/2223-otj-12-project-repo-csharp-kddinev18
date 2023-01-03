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

namespace LocalServerGUI
{
    /// <summary>
    /// Interaction logic for UserAuthenticationWindow.xaml
    /// </summary>
    public partial class UserAuthenticationWindow : Window
    {
        public UserAuthenticationWindow()
        {
            InitializeComponent();
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Drag the window of the button is hold
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove the server connection
            // Services.RemoveConnection();
            // Shutdown the application
            Application.Current.Shutdown();
        }
    }
}
