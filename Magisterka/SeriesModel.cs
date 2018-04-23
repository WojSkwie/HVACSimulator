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
        public List<HVACObject> PresentObjects { get; set; } 
        public List<string> ParametersList { get; set; } 

        public SeriesViewModel()
        {
            this.PlotTitle = "Wykres";
            this.SeriesTitle = "Wartość";
            this.ActualPoints = new List<DataPoint>();
            //this.AllDataPointsTempLists = new List<List<DataPoint>>();
            //this.AllDataPointsHumidLists = new List<List<DataPoint>>();
            //this.DescriptionsLists = new List<List<string>>();
            this.PresentObjects = new List<HVACObject>();
            this.ParametersList = new List<string>
            {
                "Temperatura",
                "Wilgotność"
            };
        }

        public void InitializeModelFromList(ObservableCollection<HVACObject> inList)
        {
            if (!inList.Any(item => item.IsPresent)) return;
            PresentObjects.AddRange(inList.Where(item => item.IsPresent));
        }

        /*public void AddPointsFromObjects()
        {
            for(int i = 0; i < PresentObjects.Count; i++)
            {
                double actualTime = GlobalParameters.SimulationTime;
            }
        }*/
        public void ResetModel()
        {
            ActualPoints.Clear();
            PresentObjects.Clear();
        }

        public void SetObjectToDrawPlot(HVACObject obj, EDataType dataType)
        {
            //int index = PresentObjects.IndexOf(obj);
            //ActualPoints = AllDataPointsLists[index];
        }
        
    }
}
