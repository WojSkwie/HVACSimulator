using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACMixingBox : HVACObject 
    {
        private HVACMixingBox CoupledMixingBox;
        private bool InSupply;
        public double MixingPercent { get; set; }
        public HVACMixingBox(bool inSupply)
        {
            IsGenerativeFlow = false;
            Name = "Komora mieszania";
            IsMovable = false;
            IsMutable = false;
            InSupply = inSupply;
            ImageSource = @"images\fan.png";
            SetPlotDataNames();

            MixingPercent = 100;
        }

        public void CoupleMixingBox(HVACMixingBox mixingBox)
        {
            CoupledMixingBox = mixingBox;
        }

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow)
        {
            if(InSupply)
            {
                double specHum = inputAir.SpecificHumidity * (100 - MixingPercent) / 100 +
                    CoupledMixingBox.OutputAir.SpecificHumidity * MixingPercent / 100;
                double temp = inputAir.Temperature * (100 - MixingPercent) / 100 +
                    CoupledMixingBox.OutputAir.Temperature * MixingPercent / 100;
                OutputAir = new Air(temp, specHum, EAirHum.specific);
                AddDataPointFromAir(OutputAir, EDataType.temperature);
                AddDataPointFromAir(OutputAir, EDataType.humidity);
                return OutputAir;
            }
            else
            {
                return base.CalculateOutputAirParameters(inputAir, airFlow);
            }
        }
    }
}
