using System;
using System.IO;
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
            if (CheckDuplicate())
            {
                MessageBox.Show("Save AutoParts.xml fails, because of duplicated Part Names!", "BackendGUI");
                return;
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.InitialDirectory = FilePath.Substring(0, (FilePath.LastIndexOf(@"\") + 1));
            dlg.FileName = "AutoParts";
            dlg.DefaultExt = "xml";
            dlg.Filter = "Xml Files (.xml)|*.xml|All Files|*.*";

            if (dlg.ShowDialog() == true)
            {
                autoParts.SaveFile(dlg.FileName);
            }

            //Check non-existing dataFolder and create them
            foreach (AutoPart part in autoParts.Parts)
            {
                if (!System.IO.Directory.Exists(part.dataFolder))
                {
                    if (DialogResult.Yes == MessageBox.Show(part.PartName + " 's setting dataFolder " + part.dataFolder + 
                        " is non-existing, do you want to create it and copy template into this folder?", "BackendGUI", System.Windows.Forms.MessageBoxButtons.YesNo))
                    {
                        System.IO.Directory.CreateDirectory(part.dataFolder);

                        string newDir = part.dataFolder.Substring(0, part.dataFolder.Length - 1);

                        //Copy new piece and inspection script template
                        if (File.Exists(Directory.GetCurrentDirectory() + @"\Templates\InspectionScripts"))
                        {
                            //File.Copy(Directory.GetCurrentDirectory() + @"\Templates\InspectionScripts", newDir + @"\InspectionScripts");
                            AutoParts.UseTemplate(Directory.GetCurrentDirectory() + @"\Templates\InspectionScripts", newDir + @"\InspectionScripts", newDir, part.PartName);
                        }
                        if (File.Exists(Directory.GetCurrentDirectory() + @"\Templates\NewPieceScripts"))
                        {
                            //File.Copy(Directory.GetCurrentDirectory() + @"\Templates\NewPieceScripts", newDir + @"\NewPieceScripts");
                            AutoParts.UseTemplate(Directory.GetCurrentDirectory() + @"\Templates\NewPieceScripts", newDir + @"\NewPieceScripts", newDir, part.PartName);
                        }
                        if (File.Exists(Directory.GetCurrentDirectory() + @"\Templates\PartAlignment.txt"))
                        {
                            //File.Copy(Directory.GetCurrentDirectory() + @"\Templates\NewPieceScripts", newDir + @"\NewPieceScripts");
                            AutoParts.UseTemplate(Directory.GetCurrentDirectory() + @"\Templates\PartAlignment.txt", newDir + @"\" + part.PartName + "Alignment.txt", newDir, part.PartName);
                        }
                    }
                }
            }

           

        }

        private bool CheckDuplicate()
        {
            //Check duplicated part name
            int totalnum = autoParts.Parts.Count;

            for (int i = 0; i < totalnum; i++)
            {
                for (int j = i+1; j < totalnum; j++)
                {
                    if (autoParts.Parts[i].PartName == autoParts.Parts[j].PartName)
                        return true;
                }
            }

            //no duplicate
            return false;
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
