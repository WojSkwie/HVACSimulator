using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class AdapterConfigViewModel
    {
        public List<EAnalogInput> SelectedAnalogInputs = new List<EAnalogInput>(4);
        public List<EAnalogInput> SelectedAnalogOutputs = new List<EAnalogInput>(4);
        public List<EAnalogInput> SelectedDigitalInputs = new List<EAnalogInput>(6);
        public List<EAnalogInput> SelectedDigitalOutputs = new List<EAnalogInput>(6);
        AdapterConfigViewModel(AdapterManager adapterManager)
        {
            adapterManager.
        }
    }
}
