using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EAnalogOutput
    {
        temperature = 0,
        humidity = 1,


    }

    public interface IBindableOutput : IBindableObject
    {
        int GetParamter(int index);
    }
}
