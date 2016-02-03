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
        public string PartName { get; set; }
        public string CreateDate { get; set; }
        public string dataFolder { get; set; }
        public string cadFileName { get; set; }
        public string robotScriptName { get; set; }
        public string inspectScriptName { get; set; }
        public string newPieceScriptName { get; set; }
        public string tolerance { get; set; }
        public string ScanOption { get; set; }

        public ObservableCollection<FeatureItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        public AutoPart(XNamespace ns, int index = 0)
        {
            NS = ns;
            PartName = "AutoPart" + index.ToString();
            CreateDate = DateTime.Now.ToString();
        }

        public AutoPart(XElement node, XNamespace ns)
        {
            try
            {
                NS = ns;
                PartName = (string)node.Element(NS + "PartName");
                CreateDate = (string)node.Element(NS + "CreateDate");
                foreach (XElement item in node.Element(NS + "Items").Elements())
                {
                    items.Add(new FeatureItem((string)item.Element("ItemName"), (string)item.Element("Description"), (string)item.Element("tolerance")));
                }
                dataFolder = (string)node.Element(NS + "dataFolder");
                cadFileName = (string)node.Element(NS + "cadFileName");
                robotScriptName = (string)node.Element(NS + "robotScriptName");
                inspectScriptName = (string)node.Element(NS + "inspectScriptName");
                newPieceScriptName = (string)node.Element(NS + "newPieceScriptName");
                tolerance = (string)node.Element(NS + "tolerance");
                ScanOption = (string)node.Element(NS + "ScanOption");
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
                int newIndex = Parts.Count;
                parts.Add(new AutoPart(NS, newIndex));
            }
        }

        public void DeletePart(int index)
        {
            if (rootNode != null)
            {
                parts.RemoveAt(index);
            }
        }
    }
}
