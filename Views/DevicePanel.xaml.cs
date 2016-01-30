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
    /// Interaction logic for DevicePanel.xaml
    /// </summary>
    public partial class DevicePanel : UserControl
    {
        CameraSettingPanel cameraPanel = null;

        public DevicePanel()
        {
            InitializeComponent();
            cameraPanel = new CameraSettingPanel();
            this.DeviceSelection.SelectedIndex = 0;
        }

        private void Device_Selected(object sender, SelectionChangedEventArgs e)
        {
            switch (DeviceSelection.SelectedIndex)
            {
                case 0:
                    this.contentPanel.Content = cameraPanel;
                    break;
                default:
                    this.contentPanel.Content = null;
                    break;
            }
        }
    }
}
