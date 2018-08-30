using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACEnvironment : PlottableObject, IResetableObject, IBindableAnalogOutput, INotifyErrorSimulation
    {
        public Air Air;
        public string Name { get; set; } = "Środowisko zewnętrzne";
        public List<BindableAnalogOutputPort> BindedOutputs { get; set; }

        private GlobalParameters GlobalParameters = GlobalParameters.Instance;

        public event EventHandler<string> SimulationErrorOccured;

        public HVACEnvironment()
        {
            SetInitialValuesParameters();
            InitializePlotDataList();
            GetGlobalErrorHandlerSubscription();
            InitializeParametersList();
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

        public void InitializeParametersList()
        {
            BindedOutputs = new List<BindableAnalogOutputPort>
            {
                new BindableAnalogOutputPort(40, -20, true, EAnalogOutput.environmentalAirTemperature)
            };
        }

        public List<EAnalogOutput> GetListOfParams(bool onlyVisible)
        {
            if (onlyVisible) return BindedOutputs.Where(item => item.Visibility == true).Select(item => item.AnalogOutput).ToList();
            return BindedOutputs.Select(item => item.AnalogOutput).ToList();
        }

        public int GetParameter(EAnalogOutput analogOutput)
        {
            var bindedParameter = BindedOutputs.FirstOrDefault(item => item.AnalogOutput == analogOutput);
            if (bindedParameter == null)
            {
                OnSimulationErrorOccured(string.Format("Próba odczytu nieprawidłowego parametru ze środowiska zewnętrznego: {0}", analogOutput.ToString()));
                return 0;
            }
            int output = 0;
            switch (analogOutput)
            {
                case EAnalogOutput.environmentalAirTemperature:
                    output = bindedParameter.ConvertTo12BitRange(Air.Temperature);
                    break;
            }
            return output;
        }

        public void OnSimulationErrorOccured(string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
        }

        public void GetGlobalErrorHandlerSubscription()
        {
            SimulationErrorOccured += GlobalParameters.Instance.OnErrorSimulationOccured;
        }
    }
}
