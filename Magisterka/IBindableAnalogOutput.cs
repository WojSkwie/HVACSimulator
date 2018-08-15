using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EAnalogOutput: byte
    {
        temperature = 0,
        relativeHumidity = 1,


    }

    public interface IBindableAnalogOutput 
    {
        void InitializeParametersList();
        List<BindableAnalogOutputPort> BindedOutputs { get; set; }
        List<EAnalogOutput> GetListOfParams();
        int GetParamter(EAnalogOutput analogOutput);
    }
}
