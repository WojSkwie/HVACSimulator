using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public sealed class HVACFan : HVACObject, IDynamicObject, IBindableAnalogInput
    {
        public HVACFan() : base()
        {
            IsGenerativeFlow = true;
            Name = "Wentylator";
            IsMovable = true;
            IsMutable = false;

            ActualSpeedPercent = 0.01;
            SetSpeedPercent = 0.01;
            TimeConstant = 5;
            HasSingleTimeConstant = true;

            ACoeff = -1;
            BCoeff = 1;
            CCoeff = 120;

            ImageSource = @"images\fan.png";
            SetPlotDataNames();
        } 
        public double SetSpeedPercent { get; set; } 
        public double ActualSpeedPercent { get; set; }
        public List<BindableAnalogInputPort> BindedInputs { get; set; }

        public void InitializeParameters()
        {
            BindedInputs = new List<BindableAnalogInputPort>
            {
                new BindableAnalogInputPort(0, 100, EAnalogInput.fanSpeed)
            };

        }

        public void SetParameter(int parameter, EAnalogInput analogInput)
        {
            if(analogInput == EAnalogInput.fanSpeed)
            {
                //SetSpeedPercent
            }
            else
            {
                OnSimulationErrorOccured(string.Format("Próba przypisania nieprawidłowego parametru jako wysterowanie wnetylatora: {0}", analogInput.ToString()));
            }
        }

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
