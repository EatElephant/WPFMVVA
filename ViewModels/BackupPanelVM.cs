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
    class BackupItem
    {
        public string Path { get; set; }
        public string Title { get; set; }

        public BackupItem(string title, string path)
        {
            Title = title;
            Path = path;
        }
    }

    class BackupPanelVM : WithNotification
    {
        private const string TRACKTREE_PATH = @"TrackTree.xml";
        private const string BACKUPLIST_PATH = @"BackupList.xml";
        private const string BACKUP_PATH = @"\Backup\"; 

        public static BackupPanelVM curVM;

        private BackupVM _backupRoot;
        private ObservableCollection<BackupVM> _rootCollection = new ObservableCollection<BackupVM>();
        private XElement _treeRoot;

        private ObservableCollection<BackupItem> _backupList = new ObservableCollection<BackupItem>();
        private int _selectedBackupIndex = -1;

        private DelegateCommand backupCommand;
        private DelegateCommand useCommand;
        private DelegateCommand deleteCommand;

        public BackupPanelVM()
        {
            curVM = this;

            backupCommand = new DelegateCommand(Backup);
            useCommand = new DelegateCommand(Use);
            deleteCommand = new DelegateCommand(Delete);

            _backupRoot = new BackupVM(new Backup("Track Tree"));

            _rootCollection.Add(_backupRoot);

            LoadBackupList();
            LoadTrackTree();
        }

        public void Use()
        {
            if (_selectedBackupIndex > -1 && _backupList.Count > 0)
            {
                UseBackup(_backupList[_selectedBackupIndex]);
                return;
            }

            MessageBox.Show("No backup selected!","BackendGUI");
        }

        public void UseBackup(BackupItem backupRecord)
        {
            DialogResult result = MessageBox.Show("We are going to use this backup to override all tracked files!!! Do you Confirm?", "Important Message", MessageBoxButtons.YesNo);

            if(result == DialogResult.No)
                return;

            XElement backup = XElement.Load(backupRecord.Path);

            foreach (XElement dir in backup.Elements("Directory"))
            {
                foreach (XElement file in dir.Elements("File"))
                {
                    File.Copy((string)file.Attribute("Src"), (string)file.Attribute("Dst"), true);
                }
            }
        }

        public void Backup()
        {
            InputDialog dlg = new InputDialog("Fill in the information for this backup and click confirm button to backup!");

            if (false == dlg.ShowDialog())
                return;

            if (dlg.BackupTitle == "")
            {
                MessageBox.Show("Invalid Backup Input! No backup title!");
                return;
            }

            BackupFiles(dlg.BackupTitle, dlg.Log);

        }

        public void Delete()
        {
            if (_selectedBackupIndex > -1 && _backupList.Count > 0)
            {
                DialogResult result = MessageBox.Show("We are going to delete this backup!!! Do you Confirm?", "Important Message", MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    return;

                DeleteBackup(_backupList[_selectedBackupIndex]);
                return;
            }

            MessageBox.Show("No backup selected!", "BackendGUI");
        }

        private void BackupFiles(string title, string log)
        {
            XElement backup = new XElement("Backup", new XAttribute("Title", title), new XAttribute("Date", DateTime.Now.ToString()), new XAttribute("Log", log));

            foreach (DirVM dir in _backupRoot.Children)
            {
                XElement dirNode = new XElement("Directory", new XAttribute("Name", dir.Name));
                foreach (FileVM file in dir.Children)
                {
                    //Src is the path of backup file
                    //Dst is the path of tracked file, which the backup file will override
                    string srcDir = Directory.GetCurrentDirectory() + BACKUP_PATH + title + @"\" + dir.Name + @"\";
                    string srcPath = srcDir + file.Name;
                    XElement fileNode = new XElement("File", new XAttribute("Name", file.Name), new XAttribute("Src", srcPath), new XAttribute("Dst", file.Path));
                    dirNode.Add(fileNode);

                    if (!Directory.Exists(srcDir))
                        Directory.CreateDirectory(srcDir);
                    File.Copy(file.Path, srcPath, true);
                }
                backup.Add(dirNode);
            }

            string backupFilePath = Directory.GetCurrentDirectory() + BACKUP_PATH + title + @"\Backup.xml";
            backup.Save(backupFilePath);

            AddBackup(title, backupFilePath);
        }

        private void LoadBackupList()
        {
            XElement list = XElement.Load(BACKUPLIST_PATH);

            foreach (XElement backup in list.Elements("Backup"))
            {
                _backupList.Add(new BackupItem((string)backup.Attribute("Title"), (string)backup.Attribute("Path")));
            }
        }

        private void UpdateBackupListFile()
        {
            XElement list = new XElement("BackupList");

            foreach (BackupItem backup in _backupList)
            {
                list.Add(new XElement("Backup", new XAttribute("Title", backup.Title), new XAttribute("Path", backup.Path)));
            }

            list.Save(BACKUPLIST_PATH);
        }

        private void AddBackup(string title, string path)
        {
            _backupList.Add(new BackupItem(title, path));

            UpdateBackupListFile();
        }

        private void DeleteBackup(BackupItem backup)
        {
            string backupDir = backup.Path.Substring(0, backup.Path.LastIndexOf(@"\"));
            Directory.Delete(backupDir, true);

            _backupList.Remove(backup);

            UpdateBackupListFile();
        }

        public void UpdateTrackTree()
        {
            _treeRoot = new XElement("TrackTree", new XAttribute("Title", _backupRoot.Title));

            foreach (DirVM dir in _backupRoot.Children)
            {
                XElement dirNode = new XElement("Directory", new XAttribute("Name",dir.Name));
                foreach (FileVM file in dir.Children)
                {
                    XElement fileNode = new XElement("File", new XAttribute("Name",file.Name), new XAttribute("Path", file.Path));
                    dirNode.Add(fileNode);
                }
                _treeRoot.Add(dirNode);
            }

            _treeRoot.Save(TRACKTREE_PATH);
        }

        private void LoadTrackTree()
        {
            _treeRoot = XElement.Load(TRACKTREE_PATH);
            _backupRoot.Children.Clear();
            _backupRoot.Title = (string)_treeRoot.Attribute("Title");

            foreach (XElement dir in _treeRoot.Elements("Directory"))
            {
                DirVM dirVM = new DirVM(new BackupDir((string)dir.Attribute("Name")));
                _backupRoot.Add(dirVM);

                foreach (XElement file in dir.Elements("File"))
                {
                    FileVM fileVM = new FileVM(new BackupFile((string)file.Attribute("Name"), (string)file.Attribute("Path")));
                    dirVM.Add(fileVM);
                }

            }
        }



        public ObservableCollection<BackupVM> RootCollection
        {
            get { return _rootCollection; }
        }

        public ObservableCollection<BackupItem> BackupList
        {
            get { return _backupList; }
        }

        public int SelectedIndex
        {
            get { return _selectedBackupIndex; }
            set
            {
                _selectedBackupIndex = value;
            }
        }

        public ICommand BackupCommand
        {
            get { return backupCommand; }
        }

        public ICommand UseCommand
        {
            get { return useCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return deleteCommand; }
        }
    }
}
