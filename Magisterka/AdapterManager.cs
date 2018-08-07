using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class AdapterManager
    {
        private List<IBindableAnalogInput> BindedInputs = new List<IBindableAnalogInput>();
        private List<IBindableAnalogOutput> BindedOutputs = new List<IBindableAnalogOutput>();
    }
}
