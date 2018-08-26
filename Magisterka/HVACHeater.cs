using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public sealed class HVACHeater : HVACTemperatureActiveObject, IDynamicObject, IBindableAnalogInput, IBindableDigitalInput
    {
        public List<BindableAnalogInputPort> BindedInputs { get; set; }
        List<EDigitalInput> IBindableDigitalInput.ParamsList { get; set; }


        public HVACHeater() : base()
        {
            IsGenerativeFlow = false;
            Name = "Nagrzewnica";
            IsMovable = true;
            HasSingleTimeConstant = true;
            ImageSource = @"images\heater.png";

            SetInitialValuesParameters();
            SetPlotDataNames();
            InitializeParametersList();
        }
 

        public void UpdateParams()
        {
            if (TimeConstant <= 0)
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }
            double derivative = (SetWaterTemperature - ActualWaterTemperature) / TimeConstant;
            ActualWaterTemperature += derivative * Constants.step;
        }

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow, double massFlow)
        {
            double W1 = WaterFlowPercent * MaximalWaterFlow / 100 * Constants.heaterFluidHeatCapacity;
            double W2 = massFlow * Constants.airHeatCapacity;
            double Nominator = 1 - Math.Exp(-(1 - W1 / W2) * HeatTransferCoeff * HeatExchangeSurface / W1);
            double Denominator = 1 - W1 / W2 * Math.Exp(-(1 - W1 / W2) * HeatTransferCoeff * HeatExchangeSurface / W1);
            double Phi = Nominator / Denominator;
            double outputTemperatureKelvin = (inputAir.Temperature - 273) + (ActualWaterTemperature - inputAir.Temperature) * W1 / W2 * Phi;
            OutputAir = new Air(outputTemperatureKelvin + 273, inputAir.SpecificHumidity, EAirHum.specific);
            AddDataPointFromAir(OutputAir, EDataType.humidity);
            AddDataPointFromAir(OutputAir, EDataType.temperature);
            return OutputAir;
        }

        public void InitializeParametersList()
        {
            BindedInputs = new List<BindableAnalogInputPort>
            {
                new BindableAnalogInputPort(0, 100, EAnalogInput.heaterFlow)
            };
        }

        public void SetParameter(int parameter, EAnalogInput analogInput)
        {
            var bindedParameter = BindedInputs.FirstOrDefault(item => item.AnalogInput == analogInput);
            if (bindedParameter == null)
            {
                OnSimulationErrorOccured(string.Format("Próba przypisania nieprawidłowego parametru jako wysterowanie pompy nagrzewnicy: {0}", analogInput.ToString()));
                return;
            }
            if (!bindedParameter.ValidateValue(parameter))
            {
                OnSimulationErrorOccured(string.Format("Niewłaściwa wartość parametru: {0}", parameter));
            }
            WaterFlowPercent = bindedParameter.ConvertToParameterRange(parameter);
        }

        public List<EAnalogInput> GetListOfParams()
        {
            return BindedInputs.Select(item => item.AnalogInput).ToList();
        }

        void IBindableDigitalInput.SetDigitalParameter(bool state, EDigitalInput digitalInput)
        {
            switch (digitalInput)
            {
                case EDigitalInput.coolerStart:
                    ActivatePump = state;
                    break;
                default:
                    OnSimulationErrorOccured(string.Format("Próba ustawienia stanu nieistniejącego parametru w nagrzewnicy: {0}", digitalInput));
                    break;
            }
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();

            ACoeff = 1;
            BCoeff = 1;
            CCoeff = 0;
            TimeConstant = 5;
            SetWaterTemperature = 80;
            ActualWaterTemperature = 80;
            WaterFlowPercent = 100;
            HeatExchangeSurface = 1;
            HeatTransferCoeff = 200;
            MaximalWaterFlow = 1;
        }
    }
}

