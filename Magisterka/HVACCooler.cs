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
            IsMovable = true;
        }

        public void UpdateParams()
        {
            throw new NotImplementedException();
        }
    }
}
