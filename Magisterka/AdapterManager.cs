using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class AdapterManager : INotifyErrorSimulation
    {
        private List<IBindableAnalogInput> ABindedInputs;
        private List<IBindableAnalogOutput> ABindedOutputs;
        private List<IBindableDigitalInput> DBindedInputs;
        private List<IBindableDigitalOutput> DBindedOutputs;


        public AdapterManager()
        {

        }

        public event EventHandler<string> SimulationErrorOccured;

        #region Initializations
        public void InitializeAnalogInputs(params IBindableAnalogInput[] bindableAnalogInputs)
        {
            ABindedInputs = bindableAnalogInputs.ToList();
        }

        public void InitializeAnalogOutputs(params IBindableAnalogOutput[] bindableAnalogOutputs)
        {
            ABindedOutputs = bindableAnalogOutputs.ToList();
        }

        public void InitializeDigitalInputs(params IBindableDigitalInput[] bindableDigitalInputs)
        {
            DBindedInputs = bindableDigitalInputs.ToList();
        }

        public void InitializeDigitalOutputs(params IBindableDigitalOutput[] bindableDigitalOutputs)
        {
            DBindedOutputs = bindableDigitalOutputs.ToList();
        }

        public void OnSimulationErrorOccured(string error)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void SetParameterSimulation(byte inputNumber, int inputValue)
        {
            if (!Enum.IsDefined(typeof(EAnalogInput), inputNumber))
            {
                OnSimulationErrorOccured("W symulacji nie ma takiego wejścia analogowego");
                return;
            }
            EAnalogInput analogInput = (EAnalogInput)inputNumber;
            IBindableAnalogInput bindableAnalogInput = ABindedInputs.Where(item => item.GetListOfParams().Contains(analogInput)).First();
            bindableAnalogInput.SetParameter(inputValue, analogInput);
        }

        public int GetParameterSimulation(byte outputNumber)
        {
            if(!Enum.IsDefined(typeof(EAnalogOutput), outputNumber))
            {
                OnSimulationErrorOccured("W symulacji nie ma takiego wyjścia analogowego");
                return 0;
            }
            EAnalogOutput analogOutput = (EAnalogOutput)outputNumber;
            IBindableAnalogOutput bindableAnalogOutput = ABindedOutputs.Where(item => item.GetListOfParams().Contains(analogOutput)).First();
            return bindableAnalogOutput.GetParamter(analogOutput);
        }
    }
}
