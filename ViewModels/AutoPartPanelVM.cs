using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendGUI.Models;
using System.Windows.Input;
using System.Windows.Forms;

namespace BackendGUI.ViewModels
{
    class AutoPartPanelVM : WithNotification
    {
        private AutoParts autoParts;
        private string filePath;

        private DelegateCommand loadCommand;
        private DelegateCommand saveToCommand;

        private DelegateCommand newPartCommand;
        private DelegateCommand delPartCommand;

        public ICommand LoadCommand
        {
            get { return loadCommand; }
        }

        public ICommand SaveCommand
        {
            get { return saveToCommand; }
        }

        public ICommand NewPartCommand
        {
            get { return newPartCommand; }
        }

        public ICommand DelPartCommand
        {
            get { return delPartCommand; }
        }


        public AutoPartPanelVM()
        {
            autoParts = new AutoParts();
            filePath = @"D:\API\Testing\WpfNITCameraTester\AutoParts.xml";

            loadCommand = new DelegateCommand(Load);
            saveToCommand = new DelegateCommand(SaveTo);
            newPartCommand = new DelegateCommand(AddNewPart);
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

        public AutoParts AutoParts
        {
            get { return autoParts; }
            set
            {
                autoParts = value;
                RaisedPropertyChanged("AutoParts");
            }
        }

        public void Load()
        {
            if (!autoParts.Open(FilePath))
            {
                MessageBox.Show("Loading File " + FilePath + " Fails!");
            }
        }

        public void SaveTo()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.InitialDirectory = FilePath.Substring(0, (FilePath.LastIndexOf(@"\") + 1));
            dlg.FileName = "AutoPart";
            dlg.DefaultExt = "xml";
            dlg.Filter = "Xml Files (.xml)|*.xml|All Files|*.*";

            if (dlg.ShowDialog() == true)
            {
                autoParts.SaveFile(dlg.FileName);
            }

        }

        public void AddNewPart()
        {
            autoParts.NewPart();
        }

        public void DelSelPart(int index)
        {
            autoParts.DeletePart(index);
        }

    }
}
