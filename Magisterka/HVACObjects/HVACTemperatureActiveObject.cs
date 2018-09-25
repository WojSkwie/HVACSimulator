using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACTemperatureActiveObject : HVACObject
    {
        public HVACTemperatureActiveObject() : base()
        {
            PlotData tempPlotData = new PlotData(EDataType.controlValue, "Czas [s]", "Procent wysterowania", Name); 
            PlotDataList.Add(tempPlotData);
        }
        private double _WaterFlowPercent;

        public double WaterFlowPercent
        {
            get { return _WaterFlowPercent; }
            set
            {
                _WaterFlowPercent = value;
                OnPropertyChanged("WaterFlowPercent");
            }
        }

        public double MaximalWaterFlow { get; set; }

        public double ActualWaterTemperature { get; set; }
        public double SetWaterTemperature { get; set; }
        private GlobalParameters GlobalParameters = GlobalParameters.Instance;

        protected bool ActivatePump;

        public void AddPointFromControlValue()
        {
            PlotData plotData = PlotDataList.Single(item => item.DataType == EDataType.controlValue);
            DataPoint newPoint = new DataPoint(GlobalParameters.SimulationTime, WaterFlowPercent);
            plotData.AddPointWithEvent(newPoint);
        }
    }
}
