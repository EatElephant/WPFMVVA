using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BackendGUI.Models;

namespace BackendGUI.ViewModels
{
    class BackupVM : TreeItemVM
    {
        private Backup _backup;

        public BackupVM(Backup backup) : base(null)
        {
            _backup = backup;
        }

        public void Add(DirVM node)
        {
            this.Children.Add(node);
        }

        public bool Remove(DirVM node)
        {
            if (node.Parent == this)
                return this.Children.Remove(node);
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

    }


    class DirVM : TreeItemVM
    {
        private BackupDir _dir;

        public DirVM(BackupDir dir, BackupVM parent)
            : base(parent)
        {
            _dir = dir;
            parent.Add(this);
        }

        public void Add(FileVM node)
        {
            this.Children.Add(node);
        }

        public bool Remove(FileVM node)
        {
            if (node.Parent == this)
                return this.Children.Remove(node);
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
            }
        }
    }


    class FileVM : TreeItemVM
    {
        private BackupFile _file;

        public FileVM(BackupFile file, DirVM parent)
            : base(parent)
        {
            _file = file;
            parent.Add(this);
        }

        public bool Detach()
        {
            if (this.Parent != null)
            {
                bool res = this.Parent.Children.Remove(this);
                this.Parent = null;

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

    }


    //Base class for TreeViewItem
    class TreeItemVM : WithNotification
    {
        #region Data

        private ObservableCollection<TreeItemVM> _children;
        private TreeItemVM _parent;

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
                }
            }
        }

        public TreeItemVM Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        #endregion

    }
}
