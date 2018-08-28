using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace HVACSimulator
{
    public class SeriesViewModel :IResetableObject
    {
        public ObservableCollection<DataPoint> ActualPoints { get; private set; }
        public string PlotTitle { get; set; }
        public string SeriesTitle { get; set; }
        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }
        public ObservableCollection<PlottableObject> PresentObjects { get; set; } 
        public List<string> ParametersList { get; set; }
        private PlotData ActualDataPointer { get; set; }

        public SeriesViewModel()
        {
            this.PlotTitle = "Wykres";
            this.SeriesTitle = "Wartość";
            this.XAxisTitle = "Oś X";
            this.YAxisTitle = "Oś Y";
            this.ActualPoints = new ObservableCollection<DataPoint>();
            this.PresentObjects = new ObservableCollection<PlottableObject>(); 
        }

        public void AddPlottableObjectsFromHVACObjects(ObservableCollection<HVACObject> inList)
        {
            if (!inList.Any(item => item.IsPresent)) return;
            foreach (var obj in inList) PresentObjects.Add(obj);
        }

        public void AddPlottableObject(PlottableObject plottableObject)
        {
            PresentObjects.Add(plottableObject);
        }

        public void ResetModel()
        {
            ActualPoints = new ObservableCollection<DataPoint>();
            if(ActualDataPointer != null ) ActualDataPointer.NewPointCreated -= OnNewPointCreated;
            
        }

        public void SetObjectToDrawPlot(PlottableObject obj, EDataType dataType)
        {
            PlotData plotData = obj.GetPlotData(dataType);
            ActualPoints = plotData.PointsList;
            PlotTitle = plotData.PlotTitle;
            XAxisTitle = plotData.XAxisTitle;
            YAxisTitle = plotData.YAxisTitle;
            plotData.NewPointCreated += OnNewPointCreated;
            ActualDataPointer = plotData;
            //obj.NewPointCreated += OnNewPointCreated;
        }

        private void OnNewPointCreated(object sender, DataPoint e)
        {
            ActualPoints.Add(e); 
        }

        public void SetInitialValuesParameters()
        {
            ActualPoints.Clear();
            PresentObjects.Clear();
        }
    }
}
