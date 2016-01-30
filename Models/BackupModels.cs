using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Collections.ObjectModel;

namespace BackendGUI.Models
{
    class Backup
    {
        public string Title;
        public string Date;
        public string Log;

        public Backup(string title, DateTime date, string log = "")
        {
            Title = title;
            Date = date.ToString();
            Log = log;
        }

        public Backup(string title)
        {
            Title = title;
        }
    }

    class BackupDir
    {
        public string Name;

        public BackupDir(string name)
        {
            Name = name;
        }

    }

    class BackupFile
    {
        public string Name;
        public string Path;

        public BackupFile(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}