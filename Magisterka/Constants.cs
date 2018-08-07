using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public static class Constants
    {
        public static double[] stepValues = { 0.1, 0.2, 0.5, 1, 2, 5 };

        public static int pointsOnCharac = 20;

        public static double step = stepValues[0];

        public static double airHeatCapacity = 1016; //J/kg/C
        public static double heaterFluidHeatCapacity = 4200; //J/kg/C
    }
}
