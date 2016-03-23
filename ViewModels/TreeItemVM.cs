using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using BackendGUI.Models;
using BackendGUI.Helper;

namespace BackendGUI.ViewModels
{
    class BackupVM : TreeItemVM
    {
        private Backup _backup;

        //ContextMenu Command
        private DelegateCommand addDirCommand;
        private DelegateCommand backupCommand;

        public BackupVM(Backup backup) : base(null)
        {
            _backup = backup;

            addDirCommand = new DelegateCommand(AddDir);
            backupCommand = new DelegateCommand(BackupPanelVM.curVM.Backup);
        }

        public void Add(DirVM node)
        {
            this.Children.Add(node);
            node.Parent = this;

            if (BackupPanelVM.curVM != null)
                BackupPanelVM.curVM.UpdateTrackTree();
        }

        public bool Remove(DirVM node)
        {
            if (node.Parent == this)
            {
                if (BackupPanelVM.curVM != null)
                    BackupPanelVM.curVM.UpdateTrackTree();
                return this.Children.Remove(node);
            }
            else
                return false;
        }

        public string Title
        {
            get { return _backup.Title; }
            set 
            { 
                _backup.Title = value;
                RaisedPropertyChanged("Title");
            }
        }

        public string Date
        {
            get { return _backup.Title; }
        }

        public string Log
        {
            get { return _backup.Log; }
            set
            {
                _backup.Log = value;
                RaisedPropertyChanged("Log");
            }
        }

        public ICommand AddDirCommand
        {
            get { return addDirCommand; }
        }

        public ICommand BackupCommand
        {
            get { return backupCommand; }
        }


        void AddDir()
        {
            Add(new DirVM(new BackupDir("new Directory")));
        }

    }


    class DirVM : TreeItemVM
    {
        private BackupDir _dir;

        //ContextMenu Command
        private DelegateCommand addFileCommand;
        private DelegateCommand renameCommand;
        private DelegateCommand deleteCommand;

        public DirVM(BackupDir dir, BackupVM parent = null)
            : base(parent)
        {
            _dir = dir;

            addFileCommand = new DelegateCommand(AddTrackedFile);
            renameCommand = new DelegateCommand(Rename);
            deleteCommand = new DelegateCommand(Delete);
        }

        public void Add(FileVM node)
        {
            this.Children.Add(node);
            node.Parent = this;

            if (BackupPanelVM.curVM != null)
                BackupPanelVM.curVM.UpdateTrackTree();
        }

        public bool Remove(FileVM node)
        {
            if (node.Parent == this)
            {
                if (BackupPanelVM.curVM != null)
                    BackupPanelVM.curVM.UpdateTrackTree();
                return this.Children.Remove(node);
            }
            else
                return false;
        }

        public bool Detach()
        {
            if (this.Parent != null)
            {
                bool res = this.Parent.Children.Remove(this);
                this.Parent = null;

                if (BackupPanelVM.curVM != null)
                    BackupPanelVM.curVM.UpdateTrackTree();

                return res;
            }
            else
                return false;
        }

        public string Name
        {
            get { return _dir.Name; }
            set
            {
                _dir.Name = value;
                RaisedPropertyChanged("Name");

                if (BackupPanelVM.curVM != null)
                    BackupPanelVM.curVM.UpdateTrackTree();
            }
        }

        public ICommand AddFileCommand
        {
            get { return addFileCommand; }
        }

        public ICommand RenameCommand
        {
            get { return renameCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return deleteCommand; }
        }

        void AddTrackedFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "All files (*.*)|*.*|txt files (*.txt)|*.txt|xml files (*.xml)|*.xml|config files (*.config)|*.config";
            dlg.RestoreDirectory = true;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                string path = dlg.FileName;
                string filename = path.Substring(path.LastIndexOf(@"\") + 1, path.Length - path.LastIndexOf(@"\") - 1);
                Add(new FileVM(new BackupFile(filename, path)));
            }
            else
                return;
        }

        void Rename()
        {
            RenameDialog dlg = new RenameDialog("Name: ");

            if (false == dlg.ShowDialog())
                return;

            Name = dlg.Value;
        }

        void Delete()
        {
            Detach();
        }
    }


    class FileVM : TreeItemVM
    {
        private BackupFile _file;

        //ContextMenu Command
        private DelegateCommand deleteCommand;

        public FileVM(BackupFile file, DirVM parent = null)
            : base(parent)
        {
            _file = file;

            deleteCommand = new DelegateCommand(Delete);
        }

        public bool Detach()
        {
            if (this.Parent != null)
            {
                bool res = this.Parent.Children.Remove(this);
                this.Parent = null;

                if (BackupPanelVM.curVM != null)
                    BackupPanelVM.curVM.UpdateTrackTree();

                return res;
            }
            else
                return false;
        }

        public string Name
        {
            get { return _file.Name; }
            set
            {
                _file.Name = value;
                RaisedPropertyChanged("Name");
            }

        }

        public string Path
        {
            get { return _file.Path; }
            set
            {
                _file.Path = value;
                RaisedPropertyChanged("Path");
            }

        }

        public ICommand DeleteCommand
        {
            get { return deleteCommand; }
        }

        void Delete()
        {
            Detach();
        }
    }


    //Base class for TreeViewItem
    class TreeItemVM : WithNotification
    {
        #region Data

        private ObservableCollection<TreeItemVM> _children;
        private TreeItemVM _parent;

        private static object _selectedItem = null;
        
        bool _isExpanded;
        bool _isSelected;

        #endregion

        #region Constructors

        protected TreeItemVM(TreeItemVM parent)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeItemVM>();

        }

        private TreeItemVM()
        {
        }

        #endregion

        #region Properties

        public ObservableCollection<TreeItemVM> Children
        {
            get { return _children; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.RaisedPropertyChanged("IsExpanded");
                }

                //Expand all the way up to root
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.RaisedPropertyChanged("IsSelected");

                    _selectedItem = this;
                }
            }
        }

        public TreeItemVM Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public static object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
            }
        }

        #endregion

    }
}
