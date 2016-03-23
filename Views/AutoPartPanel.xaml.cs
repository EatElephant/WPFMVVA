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
using BackendGUI;
using BackendGUI.ViewModels;

namespace BackendGUI.Views
{
    /// <summary>
    /// Interaction logic for AutoPartPanel.xaml
    /// </summary>
    public partial class AutoPartPanel : UserControl
    {
        private AutoPartPanelVM ViewModel;

        public AutoPartPanel()
        {
            InitializeComponent();
            ViewModel = new AutoPartPanelVM();
            this.DataContext = ViewModel;
            DataList.DataContext = ViewModel.AutoParts;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.InitialDirectory = PathText.Text.Substring(0,(PathText.Text.LastIndexOf(@"\") + 1));
            dlg.FileName = "AutoParts";
            dlg.DefaultExt = "xml";
            dlg.Filter = "Xml Files (.xml)|*.xml";

            if (dlg.ShowDialog() == true)
            {
                PathText.Text = dlg.FileName;
                PathText.Focus();
            }
        }

        private void DelPart_Click(object sender, RoutedEventArgs e)
        {
            if (DataList.SelectedIndex >= 0)
                ViewModel.DelSelPart(DataList.SelectedIndex);
        }

    }
}
