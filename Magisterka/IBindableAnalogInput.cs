using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EAnalogInput
    {
        fanSpeed = 0,
        heaterFlow = 1,
        coolerFlow = 2,
        mixingBox = 3

    }
    public interface IBindableAnalogInput
    {
        void InitializeParametersList();
        List<BindableAnalogInputPort> BindedInputs { get; set; }
        void SetParameter(int parameter, EAnalogInput analogInput);
        List<EAnalogInput> GetListOfParams();
    }
}
