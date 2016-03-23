using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendGUI.Models;
using System.Windows.Input;
using System.Windows.Forms;
using BackendGUI.Helper;

namespace BackendGUI.ViewModels
{
    class CameraSettingPanelVM : WithNotification
    {
        private CameraSettings settings;

        private string filePath;

        private DelegateCommand loadCommand;
        private DelegateCommand saveCommand;

        private DelegateCommand updateCommand;
        private DelegateCommand recoverCommand;

        public CameraSettingPanelVM()
        {
            settings = new CameraSettings();
            filePath = @"D:\API\Testing\WpfNITCameraTester\CameraSettings.xml";

            loadCommand = new DelegateCommand(Load);
            saveCommand = new DelegateCommand(SaveTo);

            updateCommand = new DelegateCommand(UpdateParam);
            recoverCommand = new DelegateCommand(RecoverParam);

        }

        public void Load()
        {
            //Add password authentication to this function. Use RenameDialog as password input
            RenameDialog pswdDlg = new RenameDialog("Password: ");

            if (false == pswdDlg.ShowDialog())
                return;

            if ("brian.li@apisensor.com" != pswdDlg.Value)
            {
                MessageBox.Show("Password is not correct, cannot load camera parameters!", "BackendGUI");
                return;
            }


            if (!settings.Open(FilePath))
            {
                MessageBox.Show("Loading File " + FilePath + " Fails!");
            }
        }

        public void SaveTo()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.InitialDirectory = FilePath.Substring(0, (FilePath.LastIndexOf(@"\") + 1));
            dlg.FileName = "CameraSettings";
            dlg.DefaultExt = "xml";
            dlg.Filter = "Xml Files (.xml)|*.xml|All Files|*.*";

            if (dlg.ShowDialog() == true)
            {
                settings.SaveFile(dlg.FileName);
            }

        }

        public void UpdateParam()
        {
            try
            {
                settings.SaveFile(settings.sourcePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update parametes fails! Error: " + ex.Message); 
            }
        }

        public void RecoverParam()
        {
            try
            {
                settings.Open(settings.sourcePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Recover parametes fails! Error: " + ex.Message);
            }
        }

        public ICommand LoadCommand
        {
            get { return loadCommand; }
        }

        public ICommand SaveCommand
        {
            get { return saveCommand; }
        }

        public ICommand UpdateCommand
        {
            get { return updateCommand; }
        }

        public ICommand RecoverCommand
        {
            get { return recoverCommand; }
        }

        public CameraSettings Settings
        {
            get { return settings; }
            set
            {
                settings = value;
            }
        }

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                RaisedPropertyChanged("FilePath");
            }
        }

    }
}
