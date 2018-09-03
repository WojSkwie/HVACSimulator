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
                ImageSource = @"images\fan2.png";
            }
            else
            {
                ImageSource = @"images\fan1.png";
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
                if(_SetSpeedPercent != value)
                {
                    _SetSpeedPercent = value;
                    if (CoupledFan != null) CoupledFan.SetSpeedPercent = value;
                    OnPropertyChanged("SetSpeedPercent");
                }
            }
        }
        private HVACFan CoupledFan;
        public double ActualSpeedPercent { get; set; }
        public List<BindableAnalogInputPort> BindedInputs { get; set; }
        public double GoalAirFlowWithMixingBox { get; set; }
        private bool _ActivateFan;
        private bool ActivateFan
        {
            get
            {
                return _ActivateFan;
            }
            set
            {
                if(value != _ActivateFan)
                {
                    _ActivateFan = value;
                    if (CoupledFan != null) CoupledFan._ActivateFan = value;
                }
            }
        }
        List<EDigitalInput> IBindableDigitalInput.ParamsList { get; set; } = new List<EDigitalInput>
        {
            EDigitalInput.fanStart
        };

        public List<EAnalogInput> GetListOfParams(bool onlyVisible)
        {
            if (onlyVisible) return BindedInputs.Where(item => item.Visibility == true).Select(item => item.AnalogInput).ToList();
            return BindedInputs.Select(item => item.AnalogInput).ToList();
        }

        public void InitializeParametersList()
        {
            BindedInputs = new List<BindableAnalogInputPort>
            {
                new BindableAnalogInputPort(0.01, 100, true, EAnalogInput.fanSpeed)
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

            ACoeff = -200;
            BCoeff = 40;
            CCoeff = 700;
            ActualSpeedPercent = 0.01;
            SetSpeedPercent = 0.01;
            TimeConstant = 5;
            GoalAirFlowWithMixingBox = 0.8;
        }

        public double CalculateDerivative(EVariableName variableName, double variableToDerivate)
        {
            switch (variableName)
            {
                case EVariableName.fanSpeed:
                    if(ActivateFan)
                    {
                        return (SetSpeedPercent - variableToDerivate) / TimeConstant;
                    }
                    else
                    {
                        return (0.01 - variableToDerivate) / TimeConstant;
                    }
                    
                default:
                    OnSimulationErrorOccured(string.Format("Próba całkowania niewłaściwego obiektu w wentylatorze: {0}", variableName));
                    return 0;
            }
        }

        public void UpdateParams()
        {
            if (TimeConstant <= 0)
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }

            double startDerivative = CalculateDerivative(EVariableName.fanSpeed, ActualSpeedPercent);
            double midValue = ActualSpeedPercent + (startDerivative * Constants.step / 2.0);
            double midDerivative = CalculateDerivative(EVariableName.fanSpeed, midValue);
            ActualSpeedPercent += midDerivative * Constants.step;

        }

        public void CoupleFan(HVACFan fan)
        {
            CoupledFan = fan;
        }
    }
}
