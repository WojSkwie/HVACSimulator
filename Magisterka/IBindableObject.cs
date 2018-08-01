using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EBindableParameter
    {
        flowPercent,
        temperature,

    }
    interface IBindableObject
    {
        double Min { get; set; }
        double Max { get; set; }
        List<EBindableParameter> AvailableParameters { get; }

        void BoundedParamChanged(object sender, int parameter);
        void BoundParameter(EBindableParameter boundableParameter, double minVal, double maxVal);
        void BoundParameter(EBindableParameter boundableParameter);
        double GetParameter(EBindableParameter boundableParameter);
        void InitializeParameterList();
    }

    
}
