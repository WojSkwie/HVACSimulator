using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class BindableAnalogOutputPort : BindableAnalogPort
    {
        public EAnalogOutput AnalogOutput { get; private set; }

        public BindableAnalogOutputPort(double max, double min, EAnalogOutput analogOutput) : base(max, min)
        {
            AnalogOutput = analogOutput;
        }

        public int ConvertTo12BitRange(double parameter)
        {
            if (parameter > MaxValue) parameter = MaxValue;
            if (parameter < MinValue) parameter = MinValue;
            double ratio = (parameter - MinValue) / (MaxValue - MinValue);
            return (int)(ratio * max12BitsNumber);
        }
    }
}
