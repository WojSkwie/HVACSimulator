using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public static class MolierCalculations
    {
        public static double FindAirDensity(double temperature)//TODO Add humidity to this method
        {
            //y = 1.289429 - 0.004834286*x + 0.00001571429*x^2
            double C = 1.289429;
            double B = 0.004834286;
            double A = 0.00001571429;
            return (A * temperature * temperature + B * temperature + C);
        }
    }
}
