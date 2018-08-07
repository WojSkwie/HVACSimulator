using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class BindableAnalogInputPort : BindableAnalogPort
    {
        public EAnalogInput AnalogInput { get; private set; }

        public BindableAnalogInputPort(double min, double max, EAnalogInput analogInput) : base(max: max, min: min)
        {
            AnalogInput = analogInput;
        }

        public double ConvertToParameterRange(int inputParamter)
        {
            double percent = inputParamter / max12BitsNumber;
            double convertedValue = MinValue + (MaxValue - MinValue) * percent;
            return convertedValue;
        }
    }
}
