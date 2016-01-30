using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Collections.ObjectModel;
using BackendGUI.ViewModels;

namespace BackendGUI.Models
{
    class CameraSettings : WithNotification
    {
        public string sourcePath;
        private XElement rootNode = null;

        public Camera cameraL { get; set; }
        public Camera cameraR { get; set; }

        //Node IVision
        public string IVisionTimeout { get; set; }

        //Node Sycronization
        public string CaptureInterval { get; set; }
        public string CommWaitTime { get; set; }
        public string TriggerExpouseTime { get; set; }
        public string TriggerDelayTime { get; set; }
        public string TriggerStopWaitTime { get; set; }
        public string TriggerOption { get; set; }
        public string SynchronizationLEDIntensity { get; set; }

        //Node Log
        public string WriteLog { get; set; }

        //Node Capture
        public string CaptureDelay { get; set; }
        public string LEDIntensity { get; set; }
        public string LEDSoft { get; set; }

        public CameraSettings()
        {
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
            }
            catch (Exception ex)
            {
                return false;
            }

            sourcePath = path;
            return ParseNode(rootNode);
        }

        public bool ParseNode(XElement node)
        {
            if (node.Name != "catalog")
                return false;
            try
            {
                //Camera Object
                cameraL = new Camera(node.Element("cameraL"));
                cameraR = new Camera(node.Element("cameraR"));

                cameraL.UpdateProperty();
                cameraR.UpdateProperty();

                //IVision Node
                IVisionTimeout = (string)node.Element("IVision").Element("IVisionTimeout");

                //Synchronization Node
                CaptureInterval = (string)node.Element("Sycronization").Element("CaptureInterval");
                CommWaitTime = (string)node.Element("Sycronization").Element("CommWaitTime");
                TriggerExpouseTime = (string)node.Element("Sycronization").Element("TriggerExpouseTime");
                TriggerDelayTime = (string)node.Element("Sycronization").Element("TriggerDelayTime");
                TriggerStopWaitTime = (string)node.Element("Sycronization").Element("TriggerStopWaitTime");
                TriggerOption = (string)node.Element("Sycronization").Element("TriggerOption");
                SynchronizationLEDIntensity = (string)node.Element("Sycronization").Element("LEDIntensity");

                //Log Node
                WriteLog = (string)node.Element("Log").Element("WriteLog");

                //Capture Node
                CaptureDelay = (string)node.Element("Capture").Element("CaptureDelay");
                LEDIntensity = (string)node.Element("Capture").Element("LEDIntensity");
                LEDSoft = (string)node.Element("Capture").Element("LEDSoft");

                RaisedPropertyChanged(null);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public XElement UpdateXMLTree()
        {
            if (rootNode != null)
                rootNode.RemoveNodes();

            rootNode.Add(cameraL.GetNode());
            rootNode.Add(cameraR.GetNode());

            XElement IVisionNode = new XElement("IVision");
            IVisionNode.Add(new XElement("IVisionTimeout", IVisionTimeout));
            rootNode.Add(IVisionNode);

            XElement SynchronizationNode = new XElement("Sycronization");
            SynchronizationNode.Add(new XElement("CaptureInterval", CaptureInterval));
            SynchronizationNode.Add(new XElement("CommWaitTime", CommWaitTime));
            SynchronizationNode.Add(new XElement("TriggerExpouseTime", TriggerExpouseTime));
            SynchronizationNode.Add(new XElement("TriggerDelayTime", TriggerDelayTime));
            SynchronizationNode.Add(new XElement("TriggerStopWaitTime", TriggerStopWaitTime));
            SynchronizationNode.Add(new XElement("TriggerOption", TriggerOption));
            SynchronizationNode.Add(new XElement("LEDIntensity", SynchronizationLEDIntensity));
            rootNode.Add(SynchronizationNode);

            XElement LogNode = new XElement("Log");
            LogNode.Add(new XElement("WriteLog", WriteLog));
            rootNode.Add(LogNode);

            XElement CaptureNode = new XElement("Capture");
            CaptureNode.Add(new XElement("CaptureDelay", CaptureDelay));
            CaptureNode.Add(new XElement("LEDIntensity", LEDIntensity));
            CaptureNode.Add(new XElement("LEDSoft", LEDSoft));
            rootNode.Add(CaptureNode);

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
    }

    class Camera : WithNotification
    {
        private XElement node;

        public string id { get; set; }
        public string Vendor { get; set; }
        public string DeviceID { get; set; }
        public string Shutter { get; set; }
        public string Trigger { get; set; }
        public string DepthBits { get; set; }
        public string PixelClock { get; set; }
        public string Gain { get; set; }
        public string FPS { get; set; }
        public string ResX { get; set; }
        public string ResY { get; set; }
        public string Expouse { get; set; }
        public string Offset { get; set; }
        public string AutoHiLo {get; set;}
        public string PixSat {get; set;}
        public string High {get; set;}
        public string Low { get; set; }
        public string FlatFieldCorrection { get; set; }
        public string description { get; set; }

        public Camera(XElement inputNode)
        {
            if(inputNode.Name != "cameraL" && inputNode.Name != "cameraR")
            {
                node = null;
                return;
            }

            node = inputNode;
        }

        public bool UpdateProperty()
        {
            if (node == null)
                return false;

            id = (string)node.Attribute("id");

            Vendor = (string)node.Element("Vendor");
            DeviceID = (string)node.Element("DeviceID");
            Shutter = (string)node.Element("Shutter");
            Trigger = (string)node.Element("Trigger");
            DepthBits = (string)node.Element("DepthBits");
            PixelClock = (string)node.Element("PixelClock");
            Gain = (string)node.Element("Gain");
            FPS = (string)node.Element("FPS");
            ResX = (string)node.Element("ResX");
            ResY = (string)node.Element("ResY");
            Expouse = (string)node.Element("Expouse");
            Offset = (string)node.Element("Offset");
            AutoHiLo = (string)node.Element("AutoHiLo");
            PixSat = (string)node.Element("PixSat");
            High = (string)node.Element("High");
            Low = (string)node.Element("Low");
            FlatFieldCorrection = (string)node.Element("FlatFieldCorrection");
            description = (string)node.Element("description");

            RaisedPropertyChanged(null);
            
            return true;
        }

        public XElement GetNode()
        {
            XElement res = new XElement("camera");
            if (id == "Left")
            {
                res.Name = "cameraL";
                res.SetAttributeValue("id", "Left");
            }
            else if (id == "Right")
            {
                res.Name = "cameraR";
                res.SetAttributeValue("id", "Right");
            }

            res.Add(new XElement("Vendor", Vendor));
            res.Add(new XElement("DeviceID", DeviceID));
            res.Add(new XElement("Shutter", Shutter));
            res.Add(new XElement("Trigger", Trigger));
            res.Add(new XElement("DepthBits", DepthBits));
            res.Add(new XElement("PixelClock", PixelClock));
            res.Add(new XElement("Gain", Gain));
            res.Add(new XElement("FPS", FPS));
            res.Add(new XElement("ResX", ResX));
            res.Add(new XElement("ResY", ResY));
            res.Add(new XElement("Expouse", Expouse));
            res.Add(new XElement("Offset", Offset));
            res.Add(new XElement("AutoHiLo", AutoHiLo));
            res.Add(new XElement("PixSat", PixSat));
            res.Add(new XElement("High", High));
            res.Add(new XElement("Low", Low));
            res.Add(new XElement("FlatFieldCorrection", FlatFieldCorrection));
            res.Add(new XElement("description", description));

            return res;
        }
    }
}
