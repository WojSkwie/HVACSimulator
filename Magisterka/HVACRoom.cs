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
            GetSubscription();
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
                new BindableAnalogOutputPort(50,-10, EAnalogOutput.temperature),
                new BindableAnalogOutputPort(100, 0, EAnalogOutput.relativeHumidity)
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

        public List<EAnalogOutput> GetListOfParams()
        {
            return BindedOutputs.Select(item => item.AnalogOutput).ToList();
        }

        public int GetParamter(EAnalogOutput analogOutput)
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
                case EAnalogOutput.temperature:
                    output = bindedParameter.ConvertTo12BitRange(AirInRoom.Temperature);
                    break;
                case EAnalogOutput.relativeHumidity:
                    output = bindedParameter.ConvertTo12BitRange(AirInRoom.RelativeHumidity);
                    break;
            }
            return output;
        }

        public void OnSimulationErrorOccured(string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
        }

        public void GetSubscription()
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
    }
}
