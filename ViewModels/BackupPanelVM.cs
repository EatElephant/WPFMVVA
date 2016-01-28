using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using BackendGUI.Models;
using BackendGUI.Helper;
using System.Xml.Linq;
using System.IO;

namespace BackendGUI.ViewModels
{
    class BackupPanelVM : WithNotification
    {
        private BackupVM _backupRoot;
        private ObservableCollection<BackupVM> _rootCollection = new ObservableCollection<BackupVM>();

        private DelegateCommand backupCommand;

        public BackupPanelVM()
        {
            backupCommand = new DelegateCommand(Backup);

            _backupRoot = new BackupVM(new Backup("Track Tree"));

            _rootCollection.Add(_backupRoot);

            DirVM defaultDir = new DirVM(new BackupDir("Default"), _backupRoot);

            FileVM testfile = new FileVM(new BackupFile("param.xml", @"d:\API\Testing\param.xml"), _backupRoot.Children[0] as DirVM);
        }

        void Backup()
        {
            InputDialog dlg = new InputDialog("Fill in the information for this backup and click confirm button to backup!");

            if (false == dlg.ShowDialog())
                return;

            if (dlg.BackupTitle == "")
            {
                MessageBox.Show("Invalid Backup Input! No backup title!");
                return;
            }

        }

        public ObservableCollection<BackupVM> RootCollection
        {
            get { return _rootCollection; }
        }

        public ICommand BackupCommand
        {
            get { return backupCommand; }
        }
    }
}
