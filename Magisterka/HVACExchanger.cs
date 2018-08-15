using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public class HVACExchanger : INotifyErrorSimulation, IDynamicObject, IBindableDigitalInput, IBindableDigitalOutput
    {
        private HVACOutletExchange OutletExchange;
        private HVACInletExchange InletExchange;

        public double MaximalEfficiencyPercent { get; set; } = 80;
        public double EfficiencyDropoutCoefficient { get; set; } = 0.1;
        private const double ReferenceTemperatureDifference = 32 - 6; //TODO uwzględnić

        public double TimeConstant = 2;

        public event EventHandler<string> SimulationErrorOccured;

        public bool BypassActivated { get; private set; }
        public bool IsFrozen { get; private set; }

        public double SetEfficiencyPercent { get; set; }
        public double ActualEfficiencyPercent { get; set; }
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
        }
        /// <summary>
        /// Oblicza parametry powietrza na wylotach kanałów. Zakłada równy przepływ przez nawiew i wywiew
        /// </summary>
        public void CalculateExchangeAndSetOutputAir(Air supplyAir, Air exhaustAir)
        {
            if(BypassActivated)
            {
                OutletExchange.OutputAir = (Air)exhaustAir.Clone(); // TODO zmienić
                InletExchange.OutputAir = (Air)supplyAir.Clone();
            }
            else
            {
                Air cooledAir;
                Air heatedAir;
                Air maximallyCooledAir;
                double dewPoint = MolierCalculations.CalculateDewPoint(exhaustAir);
                double tempDiff = exhaustAir.Temperature - supplyAir.Temperature;
                double heatedTemp = supplyAir.Temperature + tempDiff * ActualEfficiencyPercent / 100;
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
            }
        }

        public void OnSimulationErrorOccured(string error)
        {
            MessageBox.Show(error);
        }

        public double UpdateSetEfficiency(double airFlow)
        {
            SetEfficiencyPercent = MaximalEfficiencyPercent * (1 - 2 / Math.PI 
                * Math.Atan(airFlow * EfficiencyDropoutCoefficient));
            return SetEfficiencyPercent;
        }

        public double UpdateSetEfficiency(double airFlow, double MaximalEff, double dropoutCoeff)
        {
            SetEfficiencyPercent = MaximalEff * (1 - 2 / Math.PI
                * Math.Atan(airFlow * dropoutCoeff));
            return SetEfficiencyPercent;
        }

        public void UpdateParams()
        {
            if (TimeConstant <= 0)
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }
            double derivative = (SetEfficiencyPercent - ActualEfficiencyPercent) / TimeConstant;
            ActualEfficiencyPercent += derivative * Constants.step;
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
    }
}
