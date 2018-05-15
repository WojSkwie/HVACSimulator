using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACOutletExchange : HVACObject, IDynamicObject
    {
        public Air ExchangedAir { get; set; }
        public HVACOutletExchange() :base()
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

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow)
        {
            return ExchangedAir;
        }

        public void UpdateParams()
        {
            //throw new NotImplementedException(); //TODO
        }
    }
}
