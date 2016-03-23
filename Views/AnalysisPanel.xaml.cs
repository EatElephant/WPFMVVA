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

namespace BackendGUI.Views
{
    /// <summary>
    /// Interaction logic for AnalysisPanel.xaml
    /// </summary>
    public partial class AnalysisPanel : UserControl
    {
        public AnalysisPanel()
        {
            InitializeComponent();
        }

        private void SelectedPart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectedPart.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void SelectedPart_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void SelectedPart_LostFocus(object sender, RoutedEventArgs e)
        {

        }

    }
}
