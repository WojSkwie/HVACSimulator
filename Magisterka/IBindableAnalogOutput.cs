using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EAnalogOutput: byte
    {
        roomTemperature = 0,
        roomRelativeHumidity = 1,
        environmentalAirTemperature = 2,
        supplyAirTemperature = 3,
        exchangerExhaustAirTemperature = 1
    }

    public interface IBindableAnalogOutput 
    {
        void InitializeParametersList();
        List<BindableAnalogOutputPort> BindedOutputs { get; set; }
        List<EAnalogOutput> GetListOfParams();
        int GetParameter(EAnalogOutput analogOutput);
    }
}
