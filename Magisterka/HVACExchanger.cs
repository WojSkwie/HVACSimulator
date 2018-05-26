using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public class HVACExchanger : INotifyErrorSimulation
    {
        private HVACOutletExchange OutletExchange;
        private HVACInletExchange InletExchange;

        public event EventHandler<string> SimulationErrorOccured;

        public bool BypassActivated { get; set; }

        public double EfficiencyPercent { get; set; }

        public HVACExchanger(HVACInletExchange inletExchange, HVACOutletExchange outletExchange)
        {
            InletExchange = inletExchange;
            OutletExchange = outletExchange;
        }
        /// <summary>
        /// Oblicza parametry powietrza na wylotach kanałów. Zakłada równy przepływ przez nawiew i wywiew
        /// </summary>
        public void CalculateExchange(double AirFlow, Air supplyAir, Air exhaustAir)
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
                double heatedTemp = supplyAir.Temperature + tempDiff * EfficiencyPercent;
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

                /*double enthalpyDiff = exhaustAir.Enthalpy - supplyAir.Enthalpy;
                double heatedTemp = supplyAir.Temperature + (enthalpyDiff * Efficiencypercent / 100 / Constants.airHeatCapacity); //TODO nie wiem czy prawidłowe bo zakłada równość mas powietrza wyw i naw
                Air heatedAir = new Air(heatedTemp, supplyAir.SpecificHumidity, EAirHum.specific);
                Air cooledAir;
                double dewPoint = MolierCalculations.CalculateDewPoint(exhaustAir);
                if(dewPoint > supplyAir.Temperature)///wykroplenie
                {
                    Air MaximallyCooledAir = new Air(supplyAir.Temperature, 100, EAirHum.relative);
                    double maximalEnthalpyDiff = exhaustAir.Enthalpy - MaximallyCooledAir.Enthalpy;
                    if (enthalpyDiff * Efficiencypercent/100 > maximalEnthalpyDiff)
                    {
                        cooledAir = MaximallyCooledAir;
                    }
                    else
                    {
                        double temp = MaximallyCooledAir.Temperature+()
                    }
                }
                else///bez wykroplenia
                {
                    double cooledTemp = exhaustAir.Temperature - enthalpyDiff * Efficiencypercent/100 / Constants.airHeatCapacity;
                    cooledAir = new Air(cooledTemp, exhaustAir.SpecificHumidity, EAirHum.specific);
                }
                OutletExchange.OutputAir = cooledAir;
                InletExchange.OutputAir = heatedAir;*/
            }

        }

        public void OnSimulationErrorOccured(string error)
        {
            MessageBox.Show(error);
        }
    }
}
