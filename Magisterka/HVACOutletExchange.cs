using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public sealed class HVACOutletExchange : HVACObject, IBindableAnalogOutput
    {
        public HVACOutletExchange() : base()
        {
            
            IsGenerativeFlow = false;
            Name = "Kanał wymiennika";
            IsMovable = false;
            IsMutable = false;

            SetInitialValuesParameters();
            ImageSource = @"refactor";
            InitializeParametersList();
        }

        public List<BindableAnalogOutputPort> BindedOutputs { get; set; }

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow, double massFlow)
        {
            AddDataPointFromAir(OutputAir, EDataType.humidity);
            AddDataPointFromAir(OutputAir, EDataType.temperature);
            return OutputAir;
        }

        public List<EAnalogOutput> GetListOfParams(bool onlyVisible)
        {
            if (onlyVisible) return BindedOutputs.Where(item => item.Visibility == true).Select(item => item.AnalogOutput).ToList();
            return BindedOutputs.Select(item => item.AnalogOutput).ToList();
        }

        public int GetParameter(EAnalogOutput analogOutput)
        {
            var bindedParameter = BindedOutputs.FirstOrDefault(item => item.AnalogOutput == analogOutput);
            if (bindedParameter == null)
            {
                OnSimulationErrorOccured(string.Format("Próba odczytu nieprawidłowego parametru z wywiewu wymiennika: {0}", analogOutput.ToString()));
                return 0;
            }
            int output = 0;
            switch (analogOutput)
            {
                case EAnalogOutput.exchangerExhaustAirTemperature:
                    output = bindedParameter.ConvertTo12BitRange(OutputAir.Temperature);
                    break;
            }
            return output;
        }

        public void InitializeParametersList()
        {
            BindedOutputs = new List<BindableAnalogOutputPort>
            {
                new BindableAnalogOutputPort(40,-20, false, EAnalogOutput.exchangerExhaustAirTemperature)
            };
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();

            ACoeff = 0;
            BCoeff = 0;
            CCoeff = 0;
        }

        public void DeactivateOutput(EAnalogOutput analogOutput)
        {
            BindedOutputs.Where(item => item.AnalogOutput.ToString() == analogOutput.ToString()).Single().Visibility = false;
        }
    }
}
