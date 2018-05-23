using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACMixingBox : HVACObject 
    {
        private HVACMixingBox CoupledMixingBox;
        private bool InSupply;
        /// <summary>
        /// Procent powietrza użytego ponownie
        /// </summary>
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
                double specHum = (inputAir.SpecificHumidity * (100 - MixingPercent) +
                    CoupledMixingBox.OutputAir.SpecificHumidity * MixingPercent) / 100; //średnia ważona
                double temp = (inputAir.Temperature * (100 - MixingPercent) +
                    CoupledMixingBox.OutputAir.Temperature * MixingPercent) / 100; //średnia ważona
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
