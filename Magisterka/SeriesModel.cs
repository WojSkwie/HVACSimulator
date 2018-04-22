using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace HVACSimulator
{
    public class SeriesViewModel
    {
        public List<DataPoint> ActualPoints { get; private set; }
        public string PlotTitle { get; set; }
        public string SeriesTitle { get; set; }
        public List<List<DataPoint>> AllDataPointsLists { get; set; } 
        public List<List<string>> DescriptionsLists { get; set; }
        public List<HVACObject> PresentObjects { get; set; } 
        public List<string> ParametersList { get; set; }
         

        public SeriesViewModel()
        {
            this.PlotTitle = "Wykres";
            this.SeriesTitle = "Wartość";
            this.ActualPoints = new List<DataPoint>();
            this.AllDataPointsLists = new List<List<DataPoint>>();
            this.DescriptionsLists = new List<List<string>>();
            this.PresentObjects = new List<HVACObject>();
            this.ParametersList = new List<string>
            {
                "Temperatura",
                "Wilgotność"
            };
        }

        public void CreateDataListsForObjects(ObservableCollection<HVACObject> inList)
        {
            if (!inList.Any(item => item.IsPresent)) return;
            List<string> temperatureDescriptions = new List<string>
            {
                "Temperatura",
                "*C"
            };
            List<string> humidityDescriptions = new List<string>
            {
                "Wilgotność",
                "%RH"
            };
            PresentObjects.AddRange(inList.Where(item => item.IsPresent));
            foreach (HVACObject obj in PresentObjects)
            {
                AllDataPointsLists.Add(new List<DataPoint>());
                AllDataPointsLists.Add(new List<DataPoint>());
                temperatureDescriptions.Add(obj.Name);
                humidityDescriptions.Add(obj.Name);
                DescriptionsLists.Add(temperatureDescriptions);
                DescriptionsLists.Add(humidityDescriptions);
            }
        }

        public void AddPointsFromObjects()
        {
            for(int i = 0; i < PresentObjects.Count; i++)
            {
                double actualTime = GlobalParameters.SimulationTime;
                AllDataPointsLists[2 * i].Add(new DataPoint(actualTime, PresentObjects[i].OutputAir.Temperature));
                AllDataPointsLists[2 * i + 1].Add(new DataPoint(actualTime, PresentObjects[i].OutputAir.Temperature));
            }
        }
        public void ResetModel()
        {
            ActualPoints.Clear();
            DescriptionsLists.Clear();
            AllDataPointsLists.Clear();
            PresentObjects.Clear();
        }
        
    }
}
