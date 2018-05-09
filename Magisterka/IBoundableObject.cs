using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EBoundableParameter
    {
        flowPercent,
        temperature,

    }
    interface IBoundableObject
    {
        double Min { get; set; }
        double Max { get; set; }
        List<EBoundableParameter> AvailableParameters { get; }

        void BoundedParamChanged(object sender, int parameter);
        void BoundParameter(EBoundableParameter boundableParameter, double minVal, double maxVal);
        void BoundParameter(EBoundableParameter boundableParameter);
        void InitializeParameterList();
    }

    
}
