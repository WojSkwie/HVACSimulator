using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACInletExchange : HVACObject, IDynamicObject
    {
        public HVACInletExchange() : base()
        {
            IsGenerativeFlow = false;
            Name = "Kanał wymiennika";
            IsMovable = false;
            IsMutable = false;

            //TODO dopytać
            ACoeff = 0;
            BCoeff = 0;
            CCoeff = 0;

            ImageSource = @"refactor";
        }

        public void UpdateParams()
        {
            Console.Write("TODO UPDATE INLET"); //TODO
            //throw new NotImplementedException();
        }
    }
}
