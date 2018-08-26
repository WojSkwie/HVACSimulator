using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public sealed class HVACFan : HVACObject, IDynamicObject, IBindableAnalogInput, IBindableDigitalInput
    {
        public HVACFan(bool inverted) : base()
        {
            IsGenerativeFlow = true;
            Name = "Wentylator";
            IsMovable = true;
            IsMutable = false;

            HasSingleTimeConstant = true;
            if(inverted)
            {
                ImageSource = @"images\fan1.png";
            }
            else
            {
                ImageSource = @"images\fan2.png";
            }
            SetPlotDataNames();
            InitializeParametersList();
            SetInitialValuesParameters();
        }

        private double _SetSpeedPercent;

        public double SetSpeedPercent
        {
            get { return _SetSpeedPercent; }
            set
            {
                _SetSpeedPercent = value;
                OnPropertyChanged("SetSpeedPercent");
            }
        }
        

        //public double SetSpeedPercent { get; set; } 
        public double ActualSpeedPercent { get; set; }
        public List<BindableAnalogInputPort> BindedInputs { get; set; }
        private bool ActivateFan;
        List<EDigitalInput> IBindableDigitalInput.ParamsList { get; set; } = new List<EDigitalInput>
        {
            EDigitalInput.fanStart
        };

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

        void IBindableDigitalInput.SetDigitalParameter(bool state, EDigitalInput digitalInput)
        {
            switch (digitalInput)
            {
                case EDigitalInput.fanStart:
                    ActivateFan = state;
                    break;
                default:
                    OnSimulationErrorOccured(string.Format("Próba ustawienia stanu nieistniejącego parametru w wentylatorze: {0}", digitalInput));
                    break;
            }
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();

            ACoeff = -1;
            BCoeff = 1;
            CCoeff = 120;
            ActualSpeedPercent = 0.01;
            SetSpeedPercent = 0.01;
            TimeConstant = 5;
        }
    }
}
