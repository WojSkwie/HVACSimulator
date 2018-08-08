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

        public List<EAnalogInput> GetListOfParams()
        {
            return BindedInputs.Select(item => item.AnalogInput).ToList();
        }

        public void InitializeParametersList()
        {
            BindedInputs = new List<BindableAnalogInputPort>
            {
                new BindableAnalogInputPort(0.01, 100, EAnalogInput.fanSpeed)
            };

        }

        public void SetParameter(int parameter, EAnalogInput analogInput)
        {
            var bindedParameter = BindedInputs.FirstOrDefault(item => item.AnalogInput == analogInput);
            if(bindedParameter == null)
            {
                OnSimulationErrorOccured(string.Format("Próba przypisania nieprawidłowego parametru jako wysterowanie wentylatora: {0}", analogInput.ToString()));
                return;
            }
            if (!bindedParameter.ValidateValue(parameter))
            {
                OnSimulationErrorOccured(string.Format("Niewłaściwa wartość parametru: {0}", parameter));
            }
            SetSpeedPercent = bindedParameter.ConvertToParameterRange(parameter);
            
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
