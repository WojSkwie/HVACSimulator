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

            ACoeff = 1;
            BCoeff = 1;
            CCoeff = 0;
        }


        public void UpdateParams()
        {
            Console.Write("TODO UPDATE PARAMS HEATER\n"); //TODO
        }
    }
}
