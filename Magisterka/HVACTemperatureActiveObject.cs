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
        private double _WaterFlowPercent;

        public double WaterFlowPercent
        {
            get { return _WaterFlowPercent; }
            set
            {
                _WaterFlowPercent = value;
                OnPropertyChanged("WaterFlowPercent");
            }
        }

        public double MaximalWaterFlow { get; set; }

        public double ActualWaterTemperature { get; set; }
        public double SetWaterTemperature { get; set; }

        public double HeatTransferCoeff { get; set; }
        public double HeatExchangeSurface { get; set; }

        protected bool ActivatePump;
    }
}
