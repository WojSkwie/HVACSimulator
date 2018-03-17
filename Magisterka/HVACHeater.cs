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

            HasSingleTimeConstant = true;

            ACoeff = 1;
            BCoeff = 1;
            CCoeff = 0;

            ImageSource = @"images\heater.png";
        }

        public double ActualHotWaterFlow { get; set; }
        public double SetHotWaterFlow { get; set; }

        public void UpdateParams()
        {
            if (TimeConstant <= 0)
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }
            double derivative = (SetHotWaterFlow - ActualHotWaterFlow) / TimeConstant;
            ActualHotWaterFlow += derivative * Constants.step;
        }
    }
}
