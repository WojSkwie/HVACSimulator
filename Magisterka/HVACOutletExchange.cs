using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACOutletExchange : HVACObject, IDynamicObject
    {

        public HVACOutletExchange()
        {
            IsGenerativeFlow = false;
            Name = "Kanał wymiennika";
            IsMovable = false;

            //TODO dopytać
            ACoeff = 0;
            BCoeff = 0;
            CCoeff = 0;

            ImageSource = @"refactor";
        }

        public void UpdateParams()
        {
            //throw new NotImplementedException(); //TODO
        }
    }
}
