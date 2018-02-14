using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public static class MyMath
    {
        /// <summary>
        /// Finds value of quadratic equation for given X
        /// </summary>
        /// <returns></returns>
        public static double QuadEquaVal(double A, double B, double C, double X)
        {
            double Y = A * X * X + B * X + C;
            return Y;
        }

        public static double CalculateDelta(double A, double B, double C)
        {
            double delta = B * B - 4 * A * C;
            return delta;
        }

        public static double[] FindRoots(double A, double B, double C, double delta)
        {
            double[] roots = new double[2];
            if(delta == 0)
            {
                roots[0] = roots[1] = -B / (2 * A);
            }
            else
            {
                roots[0] = (-B - Math.Sqrt(delta)) / (2 * A);
                roots[1] = (-B + Math.Sqrt(delta)) / (2 * A);
            }
            return roots;

        }
    }
}
