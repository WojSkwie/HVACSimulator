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
        public ObservableCollection<DataPoint> ActualPoints { get; private set; }
        public string PlotTitle { get; set; }
        public string SeriesTitle { get; set; }
        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }
        public List<PlotableObject> PresentObjects { get; set; } 
        public List<string> ParametersList { get; set; }
        private PlotData ActualDataPointer { get; set; }

        public SeriesViewModel()
        {
            this.PlotTitle = "Wykres";
            this.SeriesTitle = "Wartość";
            this.XAxisTitle = "Oś X";
            this.YAxisTitle = "Oś Y";
            this.ActualPoints = new ObservableCollection<DataPoint>();
            this.PresentObjects = new List<PlotableObject>(); 
        }

        public void InitializeModelFromList(ObservableCollection<HVACObject> inList)
        {
            if (!inList.Any(item => item.IsPresent)) return;
            PresentObjects.AddRange(inList.Where(item => item.IsPresent));
        }

        public void AddAirChannel(AirChannel airChannel)
        {
            PresentObjects.Add(airChannel);
        }

        public void ResetModel()
        {
            ActualPoints = new ObservableCollection<DataPoint>();
            //PresentObjects.Clear();
            if(ActualDataPointer != null ) ActualDataPointer.NewPointCreated -= OnNewPointCreated;
            
        }

        public void SetObjectToDrawPlot(PlotableObject obj, EDataType dataType)
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
    }
}
