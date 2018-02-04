using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public class HVACCooler : HVACObject, IDynamicObject
    {
        public HVACCooler()
        {
            IsGenerativeFlow = false;
            Name = "Chłodnica";
        }

        public void UpdateParams()
        {
            throw new NotImplementedException();
        }
    }
}
