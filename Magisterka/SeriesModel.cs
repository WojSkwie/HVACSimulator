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
        public List<List<DataPoint>> AllDataPointsLists { get; set; } = new List<List<DataPoint>>();
        public List<List<string>> DescriptionsLists { get; set; } = new List<List<string>>();
         

        public SeriesViewModel()
        {
            this.PlotTitle = "Wykres";
            this.SeriesTitle = "Seria 1";
            this.ActualPoints = new List<DataPoint>();
        }

        public void CreateDataListsForObjects(ObservableCollection<HVACObject> inList)
        {
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
            foreach (HVACObject obj in inList)
            {
                AllDataPointsLists.Add(new List<DataPoint>());
                AllDataPointsLists.Add(new List<DataPoint>());
                DescriptionsLists.Add(temperatureDescriptions);
                DescriptionsLists.Add(humidityDescriptions);
            }
        }

        public void AddPointsFromObjects(ObservableCollection<HVACObject> inList)
        {
            for(int i = 0; i < inList.Count; i++)
            {
                double actualTime = GlobalParameters.SimulationTime;
                AllDataPointsLists[2 * i].Add(new DataPoint(actualTime, inList[i].OutputAir.Temperature));
                AllDataPointsLists[2 * i + 1].Add(new DataPoint(actualTime, inList[i].OutputAir.Temperature));
            }
        }

        /*public void addPoint(double x, double y)
        {
            this.Points.Add(new DataPoint(x, y));
        }

        public void clearPlot()
        {
            this.Points = null;
        }*/

        
    }
}
