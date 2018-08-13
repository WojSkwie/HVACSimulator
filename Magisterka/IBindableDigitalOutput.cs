using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EDigitalOutput
    {
        frozenExchanger = 0,

    }
    public interface IBindableDigitalOutput
    {
        List<EDigitalOutput> ParamsList { get; set; }
        bool SetDigitalParameter(EDigitalOutput digitalOutput);
    }
}
