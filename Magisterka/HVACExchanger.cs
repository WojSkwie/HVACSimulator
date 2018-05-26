using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public class HVACExchanger : INotifyErrorSimulation, IDynamicObject
    {
        private HVACOutletExchange OutletExchange;
        private HVACInletExchange InletExchange;

        public double MaximalEfficiencyPercent = 80;
        public double EfficiencyDropoutCoefficient = 0.05;
        private const double ReferenceTemperatureDifference = 32 - 6; //TODO uwzględnić

        public double TimeConstant = 2;

        public event EventHandler<string> SimulationErrorOccured;

        public bool BypassActivated { get; private set; }
        public bool IsFrozen { get; private set; }

        public double SetEfficiencyPercent { get; set; }
        public double ActualEfficiencyPercent { get; set; }

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
                double heatedTemp = supplyAir.Temperature + tempDiff * ActualEfficiencyPercent;
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

        public void UpdateSetEfficiency(double airFlow)
        {
            SetEfficiencyPercent = MaximalEfficiencyPercent * (1 - 2 / Math.PI 
                * Math.Atan(airFlow * EfficiencyDropoutCoefficient));
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
    }
}
