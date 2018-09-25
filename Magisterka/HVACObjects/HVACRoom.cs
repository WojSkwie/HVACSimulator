using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACRoom : PlottableObject, IBindableAnalogOutput, INotifyErrorSimulation, IResetableObject
    {
        private GlobalParameters GlobalParameters = GlobalParameters.Instance;
        public double Volume { get; set; }
        public string Name { get; set; } = "Pomieszczenie";
        public Air AirInRoom { get; set; }
        public double WallHeatCapacity { get; set; }
        public double WallAlpha { get; set; }
        public double ConstantWaterVaporSupply { get; set; }
        public List<BindableAnalogOutputPort> BindedOutputs { get ; set; }

        private double WallTemperature;
        private HVACEnvironment HVACEnvironment;

        public event EventHandler<string> SimulationErrorOccured;

        public HVACRoom(HVACEnvironment environment)
        {
            HVACEnvironment = environment;
            InitializeParametersList();
            GetGlobalErrorHandlerSubscription();
            SetInitialValuesParameters();
            InitializePlotDataList();
        }

        public void CalculateAirParametersInRoom(Air inputAir, double airFlow, double massFlow)
        {
            double densityOfRoomAir = MolierCalculations.FindAirDensity(AirInRoom);
            double firstDenominator = (Volume * densityOfRoomAir * Constants.airHeatCapacity);
            double RoomTempDelta = (massFlow * Constants.airHeatCapacity * (inputAir.Temperature - AirInRoom.Temperature) * Constants.step) / firstDenominator
                - (WallAlpha * (AirInRoom.Temperature - WallTemperature) * Constants.step) / firstDenominator;

            double WallTempDelta = WallAlpha * (AirInRoom.Temperature - WallTemperature) * Constants.step / WallHeatCapacity 
                - WallAlpha * (WallTemperature - HVACEnvironment.Air.Temperature) * Constants.step / WallHeatCapacity;

            double thirdDenominator = Volume * densityOfRoomAir;
            double humidityDelta = massFlow * (inputAir.SpecificHumidity - AirInRoom.SpecificHumidity) * Constants.step / thirdDenominator
                + ConstantWaterVaporSupply * ConstantWaterVaporSupply / thirdDenominator;

            AirInRoom.Temperature += RoomTempDelta;
            WallTemperature += RoomTempDelta;
            AirInRoom.SpecificHumidity += humidityDelta;

            AddDataPointsFromAir();
        }

        public void InitializeParametersList()
        {
            BindedOutputs = new List<BindableAnalogOutputPort>
            {
                new BindableAnalogOutputPort(40,-20, true, EAnalogOutput.roomTemperature),
                new BindableAnalogOutputPort(100, 0, false, EAnalogOutput.roomRelativeHumidity)
            };

        }

        public void AddDataPointsFromAir()
        {
            PlotData plotData = PlotDataList.Single(item => item.DataType == EDataType.temperature);
            DataPoint newPoint = new DataPoint(GlobalParameters.SimulationTime, AirInRoom.Temperature);
            plotData.AddPointWithEvent(newPoint);

            plotData = PlotDataList.Single(item => item.DataType == EDataType.humidity);
            newPoint = new DataPoint(GlobalParameters.SimulationTime, AirInRoom.RelativeHumidity);
            plotData.AddPointWithEvent(newPoint);
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
                OnSimulationErrorOccured(string.Format("Próba odczytu nieprawidłowego parametru z pomieszczenia: {0}", analogOutput.ToString()));
                return 0;
            }
            int output = 0;
            switch(analogOutput)
            {
                case EAnalogOutput.roomTemperature:
                    output = bindedParameter.ConvertTo12BitRange(AirInRoom.Temperature);
                    break;
                case EAnalogOutput.roomRelativeHumidity:
                    output = bindedParameter.ConvertTo12BitRange(AirInRoom.RelativeHumidity);
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

        public override void SetInitialValuesParameters()
        {
            AirInRoom = new Air(10, 40, EAirHum.relative);
            Volume = 50;
            WallTemperature = AirInRoom.Temperature;
            WallAlpha = 50 * 2.65;
            WallHeatCapacity = 6336000; //J/kg/C
        }

        protected override void InitializePlotDataList()
        {
            PlotData tempPlotData = new PlotData(EDataType.temperature, "Czas [s]", "Temperatura *C", "Pomieszczenie"); //TODO stopnie
            PlotDataList.Add(tempPlotData);
            PlotData humidPlotData = new PlotData(EDataType.humidity, "Czas [s]", "Wilgotność [%RH]", "Pomieszczenie");
            PlotDataList.Add(humidPlotData);
        }

        public void DeactivateOutput(EAnalogOutput analogOutput)
        {
            BindedOutputs.Where(item => item.AnalogOutput.ToString() == analogOutput.ToString()).Single().Visibility = false;
        }
    }
}
