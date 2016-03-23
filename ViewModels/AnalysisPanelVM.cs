using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using BackendGUI.Models;

namespace BackendGUI.ViewModels
{
    class AnalysisPanelVM : WithNotification
    {
        private int selectedIndex = -1;
        private string selectedItemName = "";
        private DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        private DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        private BitmapImage imageSource;

        private AutoParts autoParts;
        private string autoPartsPath = @"D:\API\Testing\WpfNITCameraTester\AutoParts.xml";

        private ObservableCollection<AutoPart> filteredParts;

        private DelegateCommand analyzeCommand;
        private DelegateCommand refreshCommand;

        public AnalysisPanelVM()
        {
            autoParts = new AutoParts();

            filteredParts = new ObservableCollection<AutoPart>();

            if (!autoParts.Open(autoPartsPath))
            {
                MessageBox.Show("Loading File " + autoPartsPath + " Fails!");
            }

            analyzeCommand = new DelegateCommand(Analyze);
            refreshCommand = new DelegateCommand(Refresh);
            imageSource = new BitmapImage(new Uri("pack://application:,,,/Images/imgNotAvailable.gif"));
        }

        #region Methods

        string ExcelPath = @"D:\SPC\Analysis\";
        string SPCFolder = @"D:\SPC";

        public void Analyze()
        {
            MessageBox.Show("Please make sure all the csv data file under the folder " + SPCFolder + " are not opened in some other process and then click ok!", "BackendGUI");

            string preName = autoParts.Parts[selectedIndex].CustomerPartNumber;

            if (preName == "")
            {
                MessageBox.Show("Can't find CustomerPartNumber for " + autoParts.Parts[SelectedIndex].PartName + "! The part being analyzed needs valid customerPartName!", "BackendGUI");
                return;
            }

            string[] files = Directory.GetFiles(SPCFolder, preName+"*.csv");

            if (files.Length == 0)
            {
                MessageBox.Show("Haven't found any features files using the customer part name as prefix and using given time span to filter! Analysis data file will not be generated!", "BackendGUI");
                return;
            }

            List<fileDate> selectedList = FilterAndSort(files);

            List<Inspection> inspectionList = ReadFeatures(selectedList);


            if (!Directory.Exists(ExcelPath))
                Directory.CreateDirectory(ExcelPath);
            //Norminal Data
            using(StreamWriter csv = new StreamWriter(ExcelPath + "AnalysisData_Nominal.csv"))
            {
                csv.WriteLine("DateFrom, " + StartDate.ToString() + ", " + "DateTo, " + EndDate.ToString());
                csv.WriteLine(",,,,,,");

                if (inspectionList.Count <= 0)
                    return;
                if (inspectionList[0].Features.Count <= 0)
                    return;

                Inspection headerFeature = inspectionList[0];
                foreach (InspectionFeature feature in headerFeature.Features)
                {
                    csv.Write(feature.Feature + ", ");
                }
                csv.WriteLine("");
                foreach (InspectionFeature feature in headerFeature.Features)
                {
                    csv.Write(feature.Control + ", ");
                }
                csv.WriteLine("");

                foreach (Inspection inspection in inspectionList)
                {
                    foreach (InspectionFeature feature in inspection.Features)
                    {
                        csv.Write(feature.Norm + ",");
                    }
                    csv.WriteLine("");
                }
            }

            //Measure Data
            using (StreamWriter csv = new StreamWriter(ExcelPath + "AnalysisData_Measure.csv"))
            {
                csv.WriteLine("DateFrom, " + StartDate.ToString() + ", " + "DateTo, " + EndDate.ToString());
                csv.WriteLine(",,,,,,");

                if (inspectionList.Count <= 0)
                    return;
                if (inspectionList[0].Features.Count <= 0)
                    return;

                Inspection headerFeature = inspectionList[0];
                foreach (InspectionFeature feature in headerFeature.Features)
                {
                    csv.Write(feature.Feature + ", ");
                }
                csv.WriteLine("");
                foreach (InspectionFeature feature in headerFeature.Features)
                {
                    csv.Write(feature.Control + ", ");
                }
                csv.WriteLine("");

                foreach (Inspection inspection in inspectionList)
                {
                    foreach (InspectionFeature feature in inspection.Features)
                    {
                        csv.Write(feature.Measure + ",");
                    }
                    csv.WriteLine("");
                }
            }

            //Measure Data
            using (StreamWriter csv = new StreamWriter(ExcelPath + "AnalysisData_Deviation.csv"))
            {
                csv.WriteLine("DateFrom, " + StartDate.ToString() + ", " + "DateTo, " + EndDate.ToString());
                csv.WriteLine(",,,,,,");

                if (inspectionList.Count <= 0)
                    return;
                if (inspectionList[0].Features.Count <= 0)
                    return;

                Inspection headerFeature = inspectionList[0];
                foreach (InspectionFeature feature in headerFeature.Features)
                {
                    csv.Write(feature.Feature + ", ");
                }
                csv.WriteLine("");
                foreach (InspectionFeature feature in headerFeature.Features)
                {
                    csv.Write(feature.Control + ", ");
                }
                csv.WriteLine("");

                foreach (Inspection inspection in inspectionList)
                {
                    foreach (InspectionFeature feature in inspection.Features)
                    {
                        csv.Write((feature.Measure - feature.Norm).ToString() + ",");
                    }
                    csv.WriteLine("");
                }
            }

            //Tolerance Data
            using (StreamWriter csv = new StreamWriter(ExcelPath + "AnalysisData_Tolerance.csv"))
            {
                csv.WriteLine("DateFrom, " + StartDate.ToString() + ", " + "DateTo, " + EndDate.ToString());
                csv.WriteLine(",,,,,,");

                if (inspectionList.Count <= 0)
                    return;
                if (inspectionList[0].Features.Count <= 0)
                    return;

                Inspection headerFeature = inspectionList[0];
                foreach (InspectionFeature feature in headerFeature.Features)
                {
                    csv.Write(feature.Feature + ", ");
                }
                csv.WriteLine("");
                foreach (InspectionFeature feature in headerFeature.Features)
                {
                    csv.Write(feature.Control + ", ");
                }
                csv.WriteLine("");

                foreach (Inspection inspection in inspectionList)
                {
                    foreach (InspectionFeature feature in inspection.Features)
                    {
                        csv.Write(feature.Tolerance + ",");
                    }
                    csv.WriteLine("");
                }
            }

            //TestResult Data
            using (StreamWriter csv = new StreamWriter(ExcelPath + "AnalysisData_TestResult.csv"))
            {
                csv.WriteLine("DateFrom, " + StartDate.ToString() + ", " + "DateTo, " + EndDate.ToString());
                csv.WriteLine(",,,,,,");

                if (inspectionList.Count <= 0)
                    return;
                if (inspectionList[0].Features.Count <= 0)
                    return;

                Inspection headerFeature = inspectionList[0];
                foreach (InspectionFeature feature in headerFeature.Features)
                {
                    csv.Write(feature.Feature + ", ");
                }
                csv.WriteLine("");
                foreach (InspectionFeature feature in headerFeature.Features)
                {
                    csv.Write(feature.Control + ", ");
                }
                csv.WriteLine("");

                foreach (Inspection inspection in inspectionList)
                {
                    foreach (InspectionFeature feature in inspection.Features)
                    {
                        csv.Write(feature.TestResult + ",");
                    }
                    csv.WriteLine("");
                }
            }


            MessageBox.Show("Analysis files are generated successfully. The CSV file needs to be open in excel and then you could create chart in it. The analysis files could be found at "
                + ExcelPath + "AnalysisData_Nominal.csv, " + ExcelPath + "AnalysisData_Measure.csv, " + ExcelPath + "AnalysisData_Deviation.csv, " + ExcelPath + "AnalysisData_Tolerance.csv and " + ExcelPath + "AnalysisData_TestResult.csv", "BackendGUI");
 
        }

        private List<Inspection> ReadFeatures(List<fileDate> selectedList)
        {
            List<Inspection> inspectionList = new List<Inspection>();


            foreach (fileDate file in selectedList)
            {
                string[] lines;
                try //Try Read Inspection Feature Data Files
                {
                    lines = File.ReadAllLines(file.filepath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception: " + ex.Message, "BackendGUI");
                    continue;
                }


                Inspection inspection = new Inspection(file.createDate);

                for (int i = 6; i < lines.Length; i++)
                {
                    string[] contents = lines[i].Split(',');

                    if (contents.Length >= 7)
                        inspection.Features.Add(new InspectionFeature(contents[0], contents[1], contents[2], contents[3], contents[4], contents[5], contents[6]));
                }
                inspectionList.Add(inspection);
            }


            return inspectionList;

        }

        struct Inspection
        {
            public Inspection(DateTime time)
            {
                InspectionTime = time;
                Features = new List<InspectionFeature>();
            }

            public DateTime InspectionTime;
            public List<InspectionFeature> Features;
        }
        

        struct InspectionFeature
        {
            public InspectionFeature(string feature, string control, string norm, string measure, string tolerance, string deviation, string result)
            {
                Feature = feature;
                Control = control;

                if (!Double.TryParse(norm, out Norm))
                    Norm = Double.NegativeInfinity;

                if (!Double.TryParse(measure, out Measure))
                    Measure = Double.NegativeInfinity;

                if (!Double.TryParse(deviation, out Deviation))
                    Deviation = Double.NegativeInfinity;

                TestResult = result;

                if(tolerance.StartsWith("±"))
                {
                    if (!Double.TryParse(tolerance.Substring(1), out Tolerance))
                        Tolerance = Double.NegativeInfinity;
                }
                else
                {
                    if (!Double.TryParse(tolerance, out Tolerance))
                        Tolerance = Double.NegativeInfinity;
                }
            }


            public string Feature;
            public string Control;
            public double Norm;
            public double Measure;
            public double Tolerance;
            public double Deviation;
            public string TestResult;
        }


        struct fileDate
        {
            public fileDate(string path, DateTime date)
            {
                filepath = path;
                createDate = date;
            }

            public string filepath;
            public DateTime createDate;
        }


        //Filter and sort files based on dates
        private List<fileDate> FilterAndSort(string[] files)
        {
            string pattern = @"\d{2}-\d{2}-\d{4}_\d+-\d+-\D{2}";
            Regex rgx = new Regex(pattern);
            
            List<fileDate> fileDateList = new List<fileDate>();

            foreach (string file in files)
            {
                Match match = rgx.Match(file);
                if (match.Success)
                {
                    string strDate = file.Substring(match.Index, match.Length);
                    string[] MDY_HrMin = strDate.Split('_');
                    string[] MDY = MDY_HrMin[0].Split('-'); //MDY_HrMin[0] is in the format of MM-DD-YYYY, so MDY[0] is Month, MDY[1] is Day, MDY[2] is Year
                    string[] HrMin = MDY_HrMin[1].Split('-'); //MDY_HrMin[1] is in the format of Hr-Min-AM/PM, so HrMin[0] is hour, HrMin[1] is minute, HrMin[2] is AM/PM

                    DateTime date;

                    if(HrMin[2] == "PM" && HrMin[0] != "12")
                        date = new DateTime(Convert.ToInt32(MDY[2]), Convert.ToInt32(MDY[0]), Convert.ToInt32(MDY[1]), Convert.ToInt32(HrMin[0]) + 12, Convert.ToInt32(HrMin[1]),0);
                    else if(HrMin[2] == "AM" && HrMin[0] == "12")
                        date = new DateTime(Convert.ToInt32(MDY[2]), Convert.ToInt32(MDY[0]), Convert.ToInt32(MDY[1]), Convert.ToInt32(HrMin[0]) - 12, Convert.ToInt32(HrMin[1]), 0);
                    else
                        date = new DateTime(Convert.ToInt32(MDY[2]), Convert.ToInt32(MDY[0]), Convert.ToInt32(MDY[1]), Convert.ToInt32(HrMin[0]), Convert.ToInt32(HrMin[1]), 0);

                    if (DateTime.Compare(date, StartDate) >= 0 && DateTime.Compare(date, EndDate) <= 0)
                        fileDateList.Add(new fileDate(file, date));
                }
            }
            //Sort the files based on date, here the time is not used, assuming the time 
            List<fileDate> sortedFileDateList = fileDateList.OrderBy(x => x.createDate.Ticks).ToList();

            return sortedFileDateList;
        }

        public void Refresh()
        {
            if (!autoParts.Open(autoPartsPath))
            {
                MessageBox.Show("Loading File " + autoPartsPath + " Fails!");
            }
        }

        private void ShowImage(int index)
        {
            string folder = autoParts.Parts[index].dataFolder;

            string[] bmps = Directory.GetFiles(folder, "*.bmp");
            string[] jpgs = Directory.GetFiles(folder, "*.jpg");

            foreach (string image in bmps)
            {
                if (File.Exists(image))
                {
                    ImageSource = new BitmapImage(new Uri(image));
                    return;
                }
            }

            foreach (string image in jpgs)
            {
                if (File.Exists(image))
                {
                    ImageSource = new BitmapImage(new Uri(image));
                    return;
                }
            }

            ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/imgNotAvailable.gif"));

        }


        private List<int> SearchByProperty(string property,string name)
        {
            List<int> res = new List<int>();

            for(int i = 0; i < autoParts.Parts.Count; i++)
            {
                if (autoParts.Parts[i].GetType().GetProperty(property).GetValue(autoParts.Parts[i]).ToString().StartsWith(name))
                {
                    res.Add(i);
                    filteredParts.Add(autoParts.Parts[i]);
                }
            }

            return res;
        }


        #endregion

        #region Attribute

        public AutoParts AutoParts
        {
            get { return autoParts; }
            set
            {
                autoParts = value;
                RaisedPropertyChanged("AutoParts");
            }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    RaisedPropertyChanged("SelectedIndex");
                    if (selectedIndex >= 0)
                    {
                        SelectedItemName = autoParts.Parts[selectedIndex].CustomerPartName;
                        ShowImage(selectedIndex);
                    }
                }
            }
        }

        public string SelectedItemName
        {
            get{ return selectedItemName;}
            set
            {
                if (value != selectedItemName)
                {
                    selectedItemName = value;
                    if (value != "")
                    {
                        List<int> sel1 = SearchByProperty("CustomerPartName", value);
                        List<int> sel2 = SearchByProperty("PartName", value);
                        if (sel1.Count > 0)
                            SelectedIndex = sel1[0];
                        else if (sel2.Count > 0)
                            SelectedIndex = sel2[0];
                    }
                        
                    RaisedPropertyChanged("SelectedItemName");
                }
            }
        }

        public ObservableCollection<AutoPart> FilteredParts
        {
            get { return filteredParts; }
            set
            {
                if (value != filteredParts)
                {
                    filteredParts = value;
                    RaisedPropertyChanged("FilteredPart");
                }
            }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                if (value != startDate)
                {
                    startDate = value;
                    RaisedPropertyChanged("StartDate");
                }
            }
        }

        public DateTime EndDate
        {
            get { return endDate.Add(new TimeSpan(23, 59, 59)); }
            set
            {
                if (value != endDate)
                {
                    endDate = value;
                    RaisedPropertyChanged("EndDate");
                }
            }
        }

        public BitmapImage ImageSource 
        {
            get
            {
                return imageSource;
            }
            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    RaisedPropertyChanged("ImageSource");
                }
            }
        }

        public ICommand AnalyzeCommand
        {
            get { return analyzeCommand; }
        }

        public ICommand RefreshCommand
        {
            get { return refreshCommand; }
        }

        #endregion
    }
}
