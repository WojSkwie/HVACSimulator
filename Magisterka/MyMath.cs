using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public static class MyMath
    {
        public static double Quad(double A, double B, double C, double X)
        {
            double Y = A * X * X + B * X + C;
            return Y;
        }
    }
}
