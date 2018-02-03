using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public class HVACFan : HVACObject, IDynamicObject
    {
        public HVACFan()
        {
            IsGenerativeFlow = true;
        }

        public void UpdateParams()
        {
            throw new NotImplementedException();
        }
    }
}
