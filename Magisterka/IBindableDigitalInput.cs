using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EDigitalInput
    {
        fanStart = 0,
        heaterStart = 1,
        coolerStart = 2,
        bypass = 3,
    }

    public interface IBindableDigitalInput
    {

        List<EDigitalInput> ParamsList { get; set; }
        void SetDigitalParameter(bool state, EDigitalInput digitalInput);
    }
}
