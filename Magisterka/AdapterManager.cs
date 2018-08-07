using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class AdapterManager
    {
        private List<IBindableInput> BindedInputs = new List<IBindableInput>();
        private List<IBindableOutput> BindedOutputs = new List<IBindableOutput>();
    }
}
