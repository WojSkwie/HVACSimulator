using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACTemperatureActiveObject : HVACObject
    {
        public HVACTemperatureActiveObject() : base()
        {

        }
        public double WaterFlowPercent { get; set; }
        public double MaximalWaterFlow { get; set; }

        public double ActualWaterTemperature { get; set; }
        public double SetWaterTemperature { get; set; }

        public double HeatTransferCoeff { get; set; }
        public double HeatExchangeSurface { get; set; }
    }
}
