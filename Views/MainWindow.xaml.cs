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
using BackendGUI.Helper;

namespace BackendGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.MainWinVM();

            Login_Click(this, new RoutedEventArgs());
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginDialog loginDlg = new LoginDialog();
            if (true == loginDlg.ShowDialog())
            {
                TabControl.IsEnabled = true;
                LoginInfo.Content = "Admin is logined in";
            }
        }
    }
}
