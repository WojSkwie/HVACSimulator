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

        public BindableAnalogInputPort(double min, double max, bool visibility, EAnalogInput analogInput) : base(max: max, min: min, visibility: visibility)
        {
            AnalogInput = analogInput;
        }

        public double ConvertToParameterRange(int inputParamter)
        {
            double percent = (double)inputParamter / (double)max12BitsNumber;
            double convertedValue = MinValue + (MaxValue - MinValue) * percent;
            return convertedValue;
        }
    }
}
