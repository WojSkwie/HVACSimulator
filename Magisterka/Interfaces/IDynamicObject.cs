using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EVariableName
    {
        waterTemp,
        fanSpeed,
        exchangerEfficiency,
        coolingPower
    }
    interface IDynamicObject
    {
        void UpdateParams();
        double CalculateDerivative(EVariableName variableName, double variableToDerivate);
        
    }
}
