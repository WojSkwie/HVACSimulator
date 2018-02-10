using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public sealed class HVACFan : HVACObject, IDynamicObject
    {
        public HVACFan()
        {
            IsGenerativeFlow = true;
            Name = "Wentylator";
            IsMovable = true;
        }

        public void UpdateParams()
        {
            throw new NotImplementedException();
        }
    }
}
