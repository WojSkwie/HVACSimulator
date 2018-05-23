using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACExchanger
    {
        private HVACOutletExchange OutletExchange;
        private HVACInletExchange InletExchange;

        public bool BypassActivated { get; set; }

        public double Efficiencypercent { get; set; }

        public HVACExchanger(HVACInletExchange inletExchange, HVACOutletExchange outletExchange)
        {
            InletExchange = inletExchange;
            OutletExchange = outletExchange;
        }

        public void CalculateExchange(double AirFlow, Air supplyAir, Air exhaustAir)
        {
            if(BypassActivated)
            {
                OutletExchange.OutputAir = (Air)exhaustAir.Clone(); // TODO zmienić
                InletExchange.OutputAir = (Air)supplyAir.Clone();
            }
            else
            {
                double enthalpyDiff = exhaustAir.Enthalpy - supplyAir.Enthalpy;
                double heatedTemp = supplyAir.Temperature + (enthalpyDiff * Efficiencypercent/100 / Constants.airHeatCapacity); //TODO nie wiem czy prawidłowe bo zakłada równość mas powietrza wyw i naw
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
                InletExchange.OutputAir = heatedAir;
            }
            
        }


    }
}
