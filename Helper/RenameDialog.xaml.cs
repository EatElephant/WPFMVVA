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
using BackendGUI.ViewModels;

namespace BackendGUI.Helper
{
    /// <summary>
    /// Interaction logic for RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog : Window
    {
        public string Label { get; set; }
        public string Value { get; set; }

        public RenameDialog(string label, bool canCancel = false, string defaultVal = "")
        {
            if (canCancel)
                cancelBtn.IsEnabled = false;
            Label = label;
            Value = defaultVal;
            InitializeComponent();
            InputTxt.Focus();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
