using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public class HVACExchanger : INotifyErrorSimulation, IDynamicObject, IBindableDigitalInput, IBindableDigitalOutput, IResetableObject
    {
        private HVACOutletExchange OutletExchange;
        private HVACInletExchange InletExchange;

        public double AproxA { get; set; }
        public double AproxB { get; set; }
        public double AproxC { get; set; }
        public double AproxD { get; set; }

        public double SetEfficiency { get; private set; }
        private double ActualEfficiency { get; set; }
        private const double ReferenceTemperatureDifference = 32 - 6; 

        public double TimeConstant;

        public event EventHandler<string> SimulationErrorOccured;

        public bool BypassActivated { get; private set; }
        public bool IsFrozen { get; private set; }

        public double SecondsToFreeze { get; set; }
        public double SecondsToMelt { get; set; }

        
        List<EDigitalInput> IBindableDigitalInput.ParamsList { get; set; } = new List<EDigitalInput>
        {
            EDigitalInput.bypass
        };
        List<EDigitalOutput> IBindableDigitalOutput.ParamsList { get; set; } = new List<EDigitalOutput>
        {
            EDigitalOutput.frozenExchanger
        };

        public HVACExchanger(HVACInletExchange inletExchange, HVACOutletExchange outletExchange)
        {
            InletExchange = inletExchange;
            OutletExchange = outletExchange;
            GetSubscription();
            SetInitialValuesParameters();
        }
        /// <summary>
        /// Oblicza parametry powietrza na wylotach kanałów. Zakłada równy przepływ przez nawiew i wywiew
        /// </summary>
        public void CalculateExchangeAndSetOutputAir(Air supplyAir, Air exhaustAir, out Air supplyAirAfter, out Air exhaustAirAfter)
        {
            //TODO przepisać pod odzysk chłodu
            if(BypassActivated) 
            {
                OutletExchange.OutputAir = (Air)exhaustAir.Clone(); 
                InletExchange.OutputAir = (Air)supplyAir.Clone();
                supplyAirAfter = InletExchange.OutputAir;
                exhaustAirAfter = OutletExchange.OutputAir;
            }
            else
            {
                Air cooledAir;
                Air heatedAir;
                Air maximallyCooledAir;
                double dewPoint = MolierCalculations.CalculateDewPoint(exhaustAir);
                double tempDiff = exhaustAir.Temperature - supplyAir.Temperature;
                double heatedTemp = supplyAir.Temperature + tempDiff * ActualEfficiency / 100;
                heatedAir = new Air(heatedTemp, supplyAir.SpecificHumidity, EAirHum.specific);
                double energyAdded = heatedAir.Enthalpy - supplyAir.Enthalpy;
                
                if(dewPoint > supplyAir.Temperature)///wykroplenie
                {
                    maximallyCooledAir = new Air(supplyAir.Temperature, 100, EAirHum.relative);
                }
                else
                {
                    maximallyCooledAir = new Air(supplyAir.Temperature, exhaustAir.SpecificHumidity, EAirHum.specific);
                }
                double enthalpyDiff = exhaustAir.Enthalpy - maximallyCooledAir.Enthalpy;
                double proportion = energyAdded / enthalpyDiff;
                if (proportion > 1 || proportion < 0) OnSimulationErrorOccured("Niewłaściwy stosunek energi w wymienniku");
                double tempCoolDiff = exhaustAir.Temperature - maximallyCooledAir.Temperature;
                double humidCoolDiff = exhaustAir.SpecificHumidity - maximallyCooledAir.SpecificHumidity;
                double cooledTemp = exhaustAir.Temperature - tempCoolDiff * proportion;
                double cooledHumid = exhaustAir.SpecificHumidity - humidCoolDiff * proportion;
                cooledAir = new Air(cooledTemp, cooledHumid, EAirHum.specific);
                OutletExchange.OutputAir = cooledAir;
                InletExchange.OutputAir = heatedAir;
                supplyAirAfter = heatedAir;
                exhaustAirAfter = cooledAir;
            }
        }

        public void OnSimulationErrorOccured(string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
            MessageBox.Show(error);
        }

        public double UpdateSetEfficiency(double airFlow)
        {
            double efficiency = MathUtil.QubicEquaVal(AproxA, AproxB, AproxC, AproxD, airFlow);
            if (efficiency > 95) efficiency = 95;
            if (efficiency < 0) efficiency = 0;
            SetEfficiency = efficiency;
            return efficiency;
        }

        void IBindableDigitalInput.SetDigitalParameter(bool state, EDigitalInput digitalInput)
        {
            switch (digitalInput)
            {
                case EDigitalInput.bypass:
                    BypassActivated = state;
                    break;
                default:
                    OnSimulationErrorOccured(string.Format("Próba przypisania nieprawidłowego parametru do wymiennika ciepła: {0}", digitalInput));
                    break;
            }

        }

        bool IBindableDigitalOutput.GetDigitalParameter(EDigitalOutput digitalOutput)
        {
            switch(digitalOutput)
            {
                case EDigitalOutput.frozenExchanger:
                    return IsFrozen;
                default:
                    OnSimulationErrorOccured(string.Format("Próba odczytu nieprawidłowego parametru z wymiennika ciepła: {0}", digitalOutput));
                    return false;
            }
        }

        public void GetSubscription()
        {
            SimulationErrorOccured += GlobalParameters.Instance.OnErrorSimulationOccured;
        }

        public void SetInitialValuesParameters()
        {
            TimeConstant = 2;
            AproxA = -11.37;
            AproxB = 53.21;
            AproxC = -68.65;
            AproxD = 97.93;

            SecondsToFreeze = 30;
            SecondsToMelt = 30;
        }

        public double CalculateDerivative(EVariableName variableName, double variableToDerivate)
        {
            switch (variableName)
            {
                case EVariableName.exchangerEfficiency:
                    return (SetEfficiency - variableToDerivate) / TimeConstant;
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

            double startDerivative = CalculateDerivative(EVariableName.exchangerEfficiency, ActualEfficiency);
            double midValue = ActualEfficiency + (startDerivative * Constants.step / 2.0);
            double midDerivative = CalculateDerivative(EVariableName.exchangerEfficiency, midValue);
            ActualEfficiency += midDerivative * Constants.step;
        }
    }
}
