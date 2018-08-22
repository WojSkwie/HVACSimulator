using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public sealed class HVACInletExchange : HVACObject
    {
        

        public HVACInletExchange() : base()
        {
            IsGenerativeFlow = false;
            Name = "Kanał wymiennika";
            IsMovable = false;
            IsMutable = false;

            //TODO dopytać

            ImageSource = @"refactor";
            SetPlotDataNames();
            SetInitialValuesParameters();
        }

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow)
        {
            AddDataPointFromAir(OutputAir, EDataType.humidity);
            AddDataPointFromAir(OutputAir, EDataType.temperature);
            return OutputAir;
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();

            ACoeff = 0;
            BCoeff = 0;
            CCoeff = 0;
        }


    }
}
