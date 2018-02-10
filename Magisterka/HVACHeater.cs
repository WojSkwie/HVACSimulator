using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public sealed class HVACHeater : HVACObject, IDynamicObject
    {
        public HVACHeater()
        {
            IsGenerativeFlow = false;
            Name = "Nagrzewnica";
            IsMovable = true;
        }


        public void UpdateParams()
        {
            throw new NotImplementedException();
        }
    }
}
