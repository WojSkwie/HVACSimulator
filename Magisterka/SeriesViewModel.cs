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
        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }
        public List<HVACObject> PresentObjects { get; set; } 
        public List<string> ParametersList { get; set; } 

        public SeriesViewModel()
        {
            this.PlotTitle = "Wykres";
            this.SeriesTitle = "Wartość";
            this.XAxisTitle = "Oś X";
            this.YAxisTitle = "Oś Y";
            this.ActualPoints = new List<DataPoint>();
            this.PresentObjects = new List<HVACObject>(); 
        }

        public void InitializeModelFromList(ObservableCollection<HVACObject> inList)
        {
            if (!inList.Any(item => item.IsPresent)) return;
            PresentObjects.AddRange(inList.Where(item => item.IsPresent));
        }

        public void ResetModel()
        {
            ActualPoints = new List<DataPoint>();
            PresentObjects.Clear();
        }

        public void SetObjectToDrawPlot(HVACObject obj, EDataType dataType)
        {
            PlotData plotData = ((IReturnsPlotData)obj).GetPlotData(dataType);
            ActualPoints = plotData.PointsList;
            PlotTitle = plotData.PlotTile;
            XAxisTitle = plotData.XAxisTitle;
            YAxisTitle = plotData.YAxisTitle;
            obj.NewPointCreated += OnNewPointCreated;
        }

        private void OnNewPointCreated(object sender, DataPoint e)
        {
            ActualPoints.Add(e); ;
        }
    }
}
