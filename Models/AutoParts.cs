using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;



namespace BackendGUI.Models
{
    public struct FeatureItem
    {
        public string ItemName, Description, tolerance;

        public FeatureItem(string name, string desc, string toler)
        {
            ItemName = name;
            Description = desc;
            tolerance = toler;
        }
    }

    class AutoPart
    {
        public XNamespace NS;
        public string PartName;
        public string CreateDate;
        public List<FeatureItem> Items = new List<FeatureItem>();
        public string dataFolder;
        public string cadFileName;
        public string robotScriptName;
        public string inspectScriptName;
        public string newPieceScriptName;
        public string tolerance;

        public AutoPart(XElement node, XNamespace ns)
        {
            NS = ns;
            PartName = (string)node.Element(NS + "PartName");
            CreateDate = (string)node.Element(NS + "CreateDate");
            foreach (XElement item in node.Element(NS + "Items").Elements())
            {
                Items.Add(new FeatureItem((string)item.Element("ItemName"), (string)item.Element("Description"), (string)item.Element("tolerance")));
            }
            dataFolder = (string)node.Element(NS + "dataFolder");
            cadFileName = (string)node.Element(NS + "cadFileName");
            robotScriptName = (string)node.Element(NS + "robotScriptName");
            inspectScriptName = (string)node.Element(NS + "inspectScriptName");
            newPieceScriptName = (string)node.Element(NS + "newPieceScriptName");
            tolerance = (string)node.Element(NS + "tolerance");
        }

        public XElement GetNode()
        {
            XElement res = new XElement("AutoPart");

            res.Add(new XElement(NS + "PartName", PartName));
            res.Add(new XElement(NS + "CreateDate", CreateDate));
            
            XElement ItemsNode = new XElement(NS + "Items");
            foreach (FeatureItem item in Items)
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

            return res;
        }
    }

    class AutoParts
    {
        public XNamespace NS;

        public List<AutoPart> Parts = new List<AutoPart>();

        private XElement rootNode;

        public AutoParts()
        {
            NS = "http://www.apoisensor.com";
        }

        public bool Open(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            rootNode = XElement.Load(path);

            foreach (XElement node in rootNode.Elements("AutoPart"))
            {
                Parts.Add(ParseNode(node));
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
    }
}
