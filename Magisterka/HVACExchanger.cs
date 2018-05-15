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

        public HVACExchanger(HVACInletExchange inletExchange, HVACOutletExchange outletExchange)
        {
            InletExchange = inletExchange;
            OutletExchange = outletExchange;
        }

        public void CalculateExchange(double AirFlow)
        {

        }


    }
}
