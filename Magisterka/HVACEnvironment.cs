using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACEnvironment : PlottableObject, IResetableObject
    {
        public Air Air;
        public string Name { get; set; } = "Środowisko zewnętrzne";
        private GlobalParameters GlobalParameters = GlobalParameters.Instance;

        public HVACEnvironment()
        {
            SetInitialValuesParameters();
            InitializePlotDataList();
        }

        protected override void InitializePlotDataList()
        {
            PlotData tempPlotData = new PlotData(EDataType.temperature, "Czas [s]", "Temperatura *C", "Temperatura zewnętrzna"); //TODO stopnie
            PlotDataList.Add(tempPlotData);
            PlotData humidPlotData = new PlotData(EDataType.humidity, "Czas [s]", "Wilgotność [%RH]", "Wilgotność zewnętrzna");
            PlotDataList.Add(humidPlotData);
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();
            Air = new Air(-5, 40, EAirHum.relative);

        }

        public void AddDataPointsFromAir()
        {
            PlotData plotData = PlotDataList.Single(item => item.DataType == EDataType.temperature);
            DataPoint newPoint = new DataPoint(GlobalParameters.SimulationTime, Air.Temperature);
            plotData.AddPointWithEvent(newPoint);

            plotData = PlotDataList.Single(item => item.DataType == EDataType.humidity);
            newPoint = new DataPoint(GlobalParameters.SimulationTime, Air.RelativeHumidity);
            plotData.AddPointWithEvent(newPoint);
        }
    }
}
