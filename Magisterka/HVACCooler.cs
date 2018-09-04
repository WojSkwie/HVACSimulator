using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public sealed class HVACCooler : HVACTemperatureActiveObject, IDynamicObject, IBindableAnalogInput, IBindableDigitalInput
    {
        
        private const double ReferenceTemperatureDifference = 32 - 6;
        public double ActualMaximalCoolingPower { get; set; } 
        public double SetMaximalCoolingPower { get; set; }
        public double CoolingTimeConstant { get; set; }
        public List<BindableAnalogInputPort> BindedInputs { get; set; }
        List<EDigitalInput> IBindableDigitalInput.ParamsList { get; set; } = new List<EDigitalInput>
        {
            EDigitalInput.coolerStart
        };

        public HVACCooler() : base()
        {
            IsGenerativeFlow = false;
            Name = "Chłodnica";
            IsMovable = true;

            ImageSource = @"images\cooler.png";

            SetPlotDataNames();
            InitializeParametersList();
            SetInitialValuesParameters();
        }

        public override Air CalculateOutputAirParameters(Air inputAir, ref double airFlow, ref double massFlow)
        {
            double inputAirDewPoint = MolierCalculations.CalculateDewPoint(inputAir);
            double enthalpyDiff = 0;
            double energyDiff = 0;
            Air MaximallyCooledAir;
            if (ActualWaterTemperature < inputAirDewPoint) ///wykroplenie
            {
                MaximallyCooledAir = new Air(ActualWaterTemperature, 100, EAirHum.relative);
            }
            else
            {
                MaximallyCooledAir = new Air(ActualWaterTemperature, inputAir.SpecificHumidity, EAirHum.specific);
            }
            enthalpyDiff = inputAir.Enthalpy - MaximallyCooledAir.Enthalpy;
            energyDiff = enthalpyDiff * massFlow;
            double coolingPower = ActualMaximalCoolingPower * (inputAir.Temperature - ActualWaterTemperature) / ReferenceTemperatureDifference;
            double RealWaterFlowValve = ActivatePump ? WaterFlowPercent : 0.01;
            coolingPower *= RealWaterFlowValve / 100;
            if (coolingPower > energyDiff)
            {
                OutputAir = MaximallyCooledAir;
            }
            else
            {
                double temp = ActualWaterTemperature + (inputAir.Temperature - ActualWaterTemperature)
                        * ((energyDiff - coolingPower) / energyDiff);
                double specHum = MaximallyCooledAir.SpecificHumidity + (inputAir.SpecificHumidity - MaximallyCooledAir.SpecificHumidity)
                    * ((energyDiff - coolingPower) / energyDiff);
                OutputAir = new Air(temp, specHum, EAirHum.specific);
                if (OutputAir.RelativeHumidity > 100) OutputAir.RelativeHumidity = 100; //TODO sprawdzić jak częśto występuje i jaka wartość
                //OutputAir = new Air(temp, specHum, EAirHum.specific);
            }
            AddDataPointFromAir(OutputAir, EDataType.humidity);
            AddDataPointFromAir(OutputAir, EDataType.temperature);
            return OutputAir;
        }

        public void InitializeParametersList()
        {
            BindedInputs = new List<BindableAnalogInputPort>
            {
                new BindableAnalogInputPort(0, 100, true, EAnalogInput.coolerFlow)
            };
        }

        public void SetParameter(int parameter, EAnalogInput analogInput)
        {
            var bindedParameter = BindedInputs.FirstOrDefault(item => item.AnalogInput == analogInput);
            if (bindedParameter == null)
            {
                OnSimulationErrorOccured(string.Format("Próba przypisania nieprawidłowego parametru jako wysterowanie pompy chłodnicy: {0}", analogInput.ToString()));
                return;
            }
            if (!bindedParameter.ValidateValue(parameter))
            {
                OnSimulationErrorOccured(string.Format("Niewłaściwa wartość parametru: {0}", parameter));
            }
            WaterFlowPercent = bindedParameter.ConvertToParameterRange(parameter);
        }

        public List<EAnalogInput> GetListOfParams(bool onlyVisible)
        {
            if (onlyVisible) return BindedInputs.Where(item => item.Visibility == true).Select(item => item.AnalogInput).ToList();
            return BindedInputs.Select(item => item.AnalogInput).ToList();
        }

        void IBindableDigitalInput.SetDigitalParameter(bool state, EDigitalInput digitalInput)
        {
            switch(digitalInput)
            {
                case EDigitalInput.heaterStart:
                    ActivatePump = state;
                    break;
                default:
                    OnSimulationErrorOccured(string.Format("Próba ustawienia stanu nieistniejącego parametru w chłodnicy: {0}", digitalInput));
                    break;
            }
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();
            ACoeff = 100;
            BCoeff = 0;
            CCoeff = 0;

            TimeConstant = 1;

            SetWaterTemperature = 6;
            ActualWaterTemperature = 6;
            WaterFlowPercent = 100;
            ActualMaximalCoolingPower = 100;
            SetMaximalCoolingPower = 100;
            CoolingTimeConstant = 10;
            MaximalWaterFlow = 1;
        }

        public double CalculateDerivative(EVariableName variableName, double variableToDerivate)
        {
            switch (variableName)
            {
                case EVariableName.waterTemp:
                    return (SetWaterTemperature - variableToDerivate) / TimeConstant;
                case EVariableName.coolingPower:
                    return (SetMaximalCoolingPower - variableToDerivate) / TimeConstant;
            default:
                    OnSimulationErrorOccured(string.Format("Próba całkowania niewłaściwego obiektu w wymienniku ciepła: {0}", variableName));
                    return 0;
            }
        }

        public void UpdateParams()
        {
            if (TimeConstant <= 0)
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }

            double startDerivative = CalculateDerivative(EVariableName.waterTemp, ActualWaterTemperature);
            double midValue = ActualWaterTemperature + (startDerivative * Constants.step / 2.0);
            double midDerivative = CalculateDerivative(EVariableName.waterTemp, midValue);
            ActualWaterTemperature += midDerivative * Constants.step;

            startDerivative = CalculateDerivative(EVariableName.coolingPower, ActualMaximalCoolingPower);
            midValue = ActualMaximalCoolingPower + (startDerivative * Constants.step / 2.0);
            midDerivative = CalculateDerivative(EVariableName.coolingPower, midValue);
            ActualMaximalCoolingPower += midDerivative * Constants.step;
        }
    }
}
