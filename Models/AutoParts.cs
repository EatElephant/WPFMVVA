using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Collections.ObjectModel;
using BackendGUI.Helper;



namespace BackendGUI.Models
{
    public class FeatureItem
    {
        private string _ItemName;
        private string _Description;
        private string _tolerance;
        public string ItemName { get { return _ItemName; } set { _ItemName = value; } }
        public string Description { get { return _Description; } set { _Description = value; } }
        public string tolerance { get { return _tolerance; } set { _tolerance = value; } }

        public FeatureItem(string name, string desc, string toler)
        {
            _ItemName = name;
            _Description = desc;
            _tolerance = toler;
        }
    }

    /// <summary>
    /// Class of Single Part
    /// </summary>
    class AutoPart
    {
        private ObservableCollection<FeatureItem> items = new ObservableCollection<FeatureItem>();

        public XNamespace NS;

        private string _partName;
        private string _partDescription;
        private string _customerPartName;
        private string _customerPartNumber;
        private string _createDate;
        private string _dataFolder;
        private string _cadFileName;
        private string _robotScriptName;
        private string _inspectScriptName;
        private string _newPieceScriptName;
        private string _tolerance;
        private string _scanOption;

        public ObservableCollection<FeatureItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        public AutoPart(XNamespace ns, int index = 0)
        {
            NS = ns;
            _partName = "Part" + index.ToString();
            _createDate = DateTime.Now.ToString();
        }

        public AutoPart(XNamespace ns, string PartName, string PartDescription, string CustomerPartName, string CustomerPartNumber, string cadFileName,
            string robotScriptName, string inspectScriptName, string newPieceScriptName, string dataFolder, string tolerance, string ScanOption)
        {
            NS = ns;
            _partName = PartName;
            _createDate = DateTime.Now.ToString();

            _partDescription = PartDescription;
            _customerPartName = CustomerPartName;
            _customerPartNumber = CustomerPartNumber;
            _cadFileName = cadFileName;
            _robotScriptName = robotScriptName;
            _inspectScriptName = inspectScriptName;
            _newPieceScriptName = newPieceScriptName;
            _dataFolder = dataFolder;
            _tolerance = tolerance;
            _scanOption = ScanOption;
        }

        public AutoPart(XElement node, XNamespace ns)
        {
            try
            {
                NS = ns;
                _partName = (string)node.Element(NS + "PartName");
                _partDescription = (string)node.Element(NS + "PartDescription");
                _customerPartName = (string)node.Element(NS + "CustomerPartName");
                _customerPartNumber = (string)node.Element(NS + "CustomerPartNumber");
                _createDate = (string)node.Element(NS + "CreateDate");
                foreach (XElement item in node.Element(NS + "Items").Elements())
                {
                    items.Add(new FeatureItem((string)item.Element("ItemName"), (string)item.Element("Description"), (string)item.Element("tolerance")));
                }
                _dataFolder = (string)node.Element(NS + "dataFolder");
                _cadFileName = (string)node.Element(NS + "cadFileName");
                _robotScriptName = (string)node.Element(NS + "robotScriptName");
                _inspectScriptName = (string)node.Element(NS + "inspectScriptName");
                _newPieceScriptName = (string)node.Element(NS + "newPieceScriptName");
                _tolerance = (string)node.Element(NS + "tolerance");
                _scanOption = (string)node.Element(NS + "ScanOption");
                if (ScanOption == null)
                    ScanOption = "0";
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public XElement GetNode()
        {
            XElement res = new XElement("AutoPart");

            res.Add(new XElement(NS + "PartName", PartName));
            res.Add(new XElement(NS + "PartDescription", PartDescription));
            res.Add(new XElement(NS + "CustomerPartName", CustomerPartName));
            res.Add(new XElement(NS + "CustomerPartNumber", CustomerPartNumber));
            res.Add(new XElement(NS + "CreateDate", CreateDate));
            
            XElement ItemsNode = new XElement(NS + "Items");
            foreach (FeatureItem item in items)
            {
                ItemsNode.Add(new XElement("FeatureItem",
                    new XElement("ItemName", item.ItemName), new XElement("Description", item.Description), new XElement("tolerance", item.tolerance)));
            }
            res.Add(ItemsNode);

            res.Add(new XElement(NS + "dataFolder", dataFolder));
            res.Add(new XElement(NS + "cadFileName", cadFileName));
            res.Add(new XElement(NS + "robotScriptName", robotScriptName));
            res.Add(new XElement(NS + "inspectScriptName", inspectScriptName));
            res.Add(new XElement(NS + "newPieceScriptName", newPieceScriptName));
            res.Add(new XElement(NS + "tolerance", tolerance));
            res.Add(new XElement(NS + "ScanOption", ScanOption));

            return res;
        }


        #region Attributes
        public string PartName
        {
            get
            {
                return _partName;
            }
            set
            {
                if (_partName != value)
                {
                    MessageBox.Show("Warning:the setting PartName has been changed, This is a very important index parameter in AutoParts.xml parameters, so changing this setting may break down some other configurations!", "BackendGUI");
                    _partName = value;
                }
            }
        }
        public string PartDescription 
        {
            get
            {
                return _partDescription;
            }
            set
            {
                if (_partDescription != value)
                    _partDescription = value;
            }
        }
        public string CustomerPartName 
        {
            get
            {
                return _customerPartName;
            }
            set
            {
                if (_customerPartName != value)
                    _customerPartName = value;
            }
        }
        public string CustomerPartNumber 
        {
            get
            {
                return _customerPartNumber;
            }
            set
            {
                if (_customerPartNumber != value)
                    _customerPartNumber = value;
            }
        }
        public string CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                if (_createDate != value)
                    _createDate = value;
            }
        }
        public string dataFolder
        {
            get
            {
                return _dataFolder;
            }
            set
            {
                if (_dataFolder != value)
                {
                    string pre_folder = _dataFolder;
                    _dataFolder = value;
                    try
                    {
                        if(Directory.Exists(pre_folder))
                        {
                            if (DialogResult.Yes == MessageBox.Show("Warning:the setting dataFolder has been changed, some of the AutoParts.xml configuration may break down! " +
                                "Please check to make sure everything works! Do you want to rename the folder " + pre_folder + " to " + _dataFolder + "?", "BackendGUI", System.Windows.Forms.MessageBoxButtons.YesNo))
                            {
                                Directory.Move(pre_folder, _dataFolder);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Rename Folder Exception: " + ex.Message, "BackendGUI");
                    }

                }
            }
        }
        public string cadFileName
        {
            get
            {
                return _cadFileName;
            }
            set
            {
                if (_cadFileName != value)
                    _cadFileName = value;
            }
        }
        public string robotScriptName 
        {
            get
            {
                return _robotScriptName;
            }
            set
            {
                if (_robotScriptName != value)
                    _robotScriptName = value;
            }
        }
        public string inspectScriptName 
        {
            get
            {
                return _inspectScriptName;
            }
            set
            {
                if (_inspectScriptName != value)
                    _inspectScriptName = value;
            }
        }
        public string newPieceScriptName
        {
            get
            {
                return _newPieceScriptName;
            }
            set
            {
                if (_newPieceScriptName != value)
                    _newPieceScriptName = value;
            }
        }
        public string tolerance 
        {
            get
            {
                return _tolerance;
            }
            set
            {
                if (_tolerance != value)
                    _tolerance = value;
            }
        }
        public string ScanOption 
        {
            get
            {
                return _scanOption;
            }
            set
            {
                if (_scanOption != value)
                    _scanOption = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// Class of AutoParts.xml file
    /// </summary>
    class AutoParts
    {
        public XNamespace NS;
        public ObservableCollection<AutoPart> Parts
        {
            get { return parts; }
            set { parts = value; }
        }

        private ObservableCollection<AutoPart> parts = new ObservableCollection<AutoPart>();
        private XElement rootNode = null;

        public AutoParts()
        {
            NS = "http://www.apoisensor.com";
        }

        public bool Open(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                rootNode = XElement.Load(path);

                parts.Clear();

                foreach (XElement node in rootNode.Elements("AutoPart"))
                {
                    parts.Add(ParseNode(node));
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public AutoPart ParseNode(XElement node)
        {
            if (node.Name != "AutoPart")
                return null;

            AutoPart res = new AutoPart(node, NS);

            return res;
        }

        public XElement UpdateXMLTree()
        {
            if(rootNode != null)
                rootNode.RemoveNodes();

            foreach (AutoPart part in parts)
            {
                rootNode.Add(part.GetNode());
            }

            return rootNode;
        }

        public void SaveFile(string path)
        {
            if (UpdateXMLTree() != null)
            {
                if (File.Exists(path))
                    File.Delete(path);
                rootNode.Save(path);
            }
        }

        public void NewPart()
        {
            if (rootNode != null)
            {
                int newIndex = GetNextIndex();

                string partName = "Part" + newIndex.ToString();

                RenameDialog inputDlg = new RenameDialog("PartName: ", false, partName);

                if (false == inputDlg.ShowDialog())
                    return;
                else
                    partName = inputDlg.Value;
                

                string newDir = @"D:\API\Testing\" + partName;

                AutoPart newPart = new AutoPart(NS, partName, "Description For This Part", "Part Name From Customer", "Part Number From Customer", partName + ".stp", partName.ToUpper(),
                    "InspectionScripts", "NewPieceScripts", newDir + @"\", "1", "0");
                parts.Add(newPart);

                //if (Directory.Exists(@"D:\API\Testing"))
                //{
                //    if (!Directory.Exists(newDir))
                //    {
                //        if (DialogResult.Yes == MessageBox.Show("Do you want to create directory " + newDir + " ?", "BackendGUI", System.Windows.Forms.MessageBoxButtons.YesNo))
                //        {
                //            Directory.CreateDirectory(newDir);

                //            //Copy new piece and inspection script template
                //            if (File.Exists(Directory.GetCurrentDirectory() + @"\Templates\InspectionScripts"))
                //            {
                //                //File.Copy(Directory.GetCurrentDirectory() + @"\Templates\InspectionScripts", newDir + @"\InspectionScripts");
                //                UseTemplate(Directory.GetCurrentDirectory() + @"\Templates\InspectionScripts", newDir + @"\InspectionScripts", newDir, partName);
                //            }
                //            if (File.Exists(Directory.GetCurrentDirectory() + @"\Templates\NewPieceScripts"))
                //            {
                //                //File.Copy(Directory.GetCurrentDirectory() + @"\Templates\NewPieceScripts", newDir + @"\NewPieceScripts");
                //                UseTemplate(Directory.GetCurrentDirectory() + @"\Templates\NewPieceScripts", newDir + @"\NewPieceScripts", newDir, partName);
                //            }
                //            if (File.Exists(Directory.GetCurrentDirectory() + @"\Templates\PartAlignment.txt"))
                //            {
                //                //File.Copy(Directory.GetCurrentDirectory() + @"\Templates\NewPieceScripts", newDir + @"\NewPieceScripts");
                //                UseTemplate(Directory.GetCurrentDirectory() + @"\Templates\PartAlignment.txt", newDir + @"\" + partName + "Alignment.txt", newDir, partName);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        MessageBox.Show("Path: " + newDir + " already exists, new folder will not be created!","BackendGUI");
                //    }
                //}
            }
        }

        static public void UseTemplate(string source, string destination, string realPath, string realName)
        {
            if (File.Exists(source))
            {
                string template = File.ReadAllText(source);
                string temp = template.Replace("_UNDEFINEPATH_", realPath);
                string finalfile = temp.Replace("_PARTNAME_", realName);
                File.WriteAllText(destination, finalfile);
            }
        }

        private int GetNextIndex()
        {
            int index = 1;
            bool bFoundIndex = false;

            do
            {
                bFoundIndex = false;
                foreach (AutoPart part in parts)
                {
                    if (part.dataFolder == @"D:\API\Testing\Part" + index.ToString() + @"\"||part.PartName == "Part" + index.ToString())
                    {
                        bFoundIndex = true;
                        break;
                    }
                }

                if(bFoundIndex)
                    index++;

            } while (bFoundIndex);

            return index;
        }

        public void DeletePart(int index)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you confirm to delete the part " + parts[index].PartName + " ?", "BackendGUI", System.Windows.Forms.MessageBoxButtons.YesNo))
            {
                string delPath = parts[index].dataFolder;
                if (rootNode != null)
                {
                    parts.RemoveAt(index);
                }

                if (Directory.Exists(delPath))
                {
                    if (DialogResult.Yes == MessageBox.Show("Do you also want to delete the directory " + delPath + " ?", "BackendGUI", System.Windows.Forms.MessageBoxButtons.YesNo))
                    {
                        Directory.Delete(delPath, true);
                    }
                }
            }

        }
    }
}
