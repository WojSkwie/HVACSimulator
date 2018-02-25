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

            ACoeff = 1;
            BCoeff = 1;
            CCoeff = 0;

            ImageSource = @"images\cooler.png";
        }

        public void UpdateParams()
        {
            Console.Write("TODO UPDATE PARAMS COOLER\n"); //TODO
        }
    }
}
