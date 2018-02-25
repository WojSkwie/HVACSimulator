using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Magisterka
{
    public sealed class HVACFan : HVACObject, IDynamicObject
    {
        public HVACFan()
        {
            IsGenerativeFlow = true;
            Name = "Wentylator";
            IsMovable = true;

            ActualSpeedPercent = 0;
            SetSpeedPercent = 0;
            TimeConstant = 5;

            ACoeff = -1;
            BCoeff = 1;
            CCoeff = 120;

            ImageSource = "images\fan.png";
        } 
        public double TimeConstant { get; set; }
        public double SetSpeedPercent { get; set; } 
        public double ActualSpeedPercent { get; set; } 

        public void UpdateParams()
        {
            if(TimeConstant <= 0 )
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }
            double derivative = (SetSpeedPercent - ActualSpeedPercent) / TimeConstant;
            ActualSpeedPercent += derivative * Constants.step;

        }
    }
}
