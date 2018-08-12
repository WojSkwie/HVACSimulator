using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACRoom : IBindableAnalogOutput, INotifyErrorSimulation
    {
        public double Volume { get; set; }
        public Air AirInRoom { get; set; }
        public List<BindableAnalogOutputPort> BindedOutputs { get ; set; }

        private double WallTemperature;
        private HVACEnvironment HVACEnvironment;

        public event EventHandler<string> SimulationErrorOccured;

        public HVACRoom(HVACEnvironment environment)
        {
            HVACEnvironment = environment;
        }

        public void CalculateAirParametersInRoom(Air inputAir, double airFlow)
        {

        }

        public void InitializeParametersList()
        {
            BindedOutputs = new List<BindableAnalogOutputPort>
            {
                new BindableAnalogOutputPort(50,-10, EAnalogOutput.temperature),
                new BindableAnalogOutputPort(100, 0, EAnalogOutput.relativeHumidity)
            };

        }

        public List<EAnalogOutput> GetListOfParams()
        {
            return BindedOutputs.Select(item => item.AnalogOutput).ToList();
        }

        public int GetParamter(EAnalogOutput analogOutput)
        {
            var bindedParameter = BindedOutputs.FirstOrDefault(item => item.AnalogOutput == analogOutput);
            if (bindedParameter == null)
            {
                OnSimulationErrorOccured(string.Format("Próba przypisania nieprawidłowego parametru jako wysterowanie pompy nagrzewnicy: {0}", analogInput.ToString()));
                return 0;
            }
            int output = 0;
            switch(analogOutput)
            {
                case EAnalogOutput.temperature:
                    output = bindedParameter.ConvertTo12BitRange(AirInRoom.Temperature);
                    break;
                case EAnalogOutput.relativeHumidity:
                    output = bindedParameter.ConvertTo12BitRange(AirInRoom.RelativeHumidity);
                    break;
            }
            return output;
        }

        public void OnSimulationErrorOccured(string error)
        {
            throw new NotImplementedException();
        }
    }
}
