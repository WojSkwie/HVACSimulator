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

        public static double CalculateSaturationVaporPressure(double temperature)
        {
            double Es = 611.2 * Math.Exp(17.67 * (temperature) / (temperature + 273.16 - 29.65));
            return Es;
            //double pressure = 0.6108 * Math.Exp(17.27 * temperature / (temperature + 237.3));
            //return pressure;
        }

        public static double HumiditySpecificToRelative(Air air)
        {
            double Es = CalculateSaturationVaporPressure(air.Temperature);  //611.2 * Math.Exp(17.67 * (air.Temperature) / (air.Temperature + 273.16 - 29.65));
            double E = air.SpecificHumidity * 101325.0 / (0.378 * air.SpecificHumidity + 0.622);
            double Relative = E / Es;
            return Relative * 100.0;
        }

        public static double HumidityRelativeToSpecific(Air air)
        {
            double Es = CalculateSaturationVaporPressure(air.Temperature);//6.122 * Math.Exp
            double E = air.RelativeHumidity / 100.0 * Es;
            double p_mb = 101325.0;// 100.0;
            double Specific = (0.622 * E) / (p_mb - (0.378 * E));
            return Specific;

        }

        /*public static double HumidityRelativeToAbsulute(Air air)
        {

        }*/
        /// <returns>Dew point as temperature</returns>
        public static double CalculateDewPoint(Air air)
        {
            double A = (Math.Log(air.RelativeHumidity / 100) / 17.27 + air.Temperature / (237.3 + air.Temperature));
            double Nominator = 237.3 * A;
            double Denominator = 1 - A;
            double DewPoint = Nominator / Denominator;
            return DewPoint;
        }
    }
}
