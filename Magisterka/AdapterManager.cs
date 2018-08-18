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

        private Parser Parser = new Parser();

        private USB USB = new USB();

        public static string LastOpenedPort = string.Empty;

        public void OnAnalogParameterArrived(object sender, KeyValuePair<EAnalogInput, int> parameter)
        {
            SetAnalogParameterSimulation(parameter.Key, parameter.Value);
        }

        public void OnDigitalParameterArrived(object sender, KeyValuePair<EDigitalInput, bool> parameter)
        {
            SetDigitalParameterSimulation(parameter.Key, parameter.Value);
        }

        public bool IsConnected
        {
            get
            {
                return USB.IsOpen;
            }
        }



        public AdapterManager(EventHandler<byte[]> correctFrameHandler)
        {
            USB.CorrectFrameRead += correctFrameHandler;
            //USB.CorrectFrameRead += Parser.ParseCorrectCroppedFrame;
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

        public void SetAnalogParameterSimulation(EAnalogInput analogInput, int inputValue)
        {
            IBindableAnalogInput bindableAnalogInput = ABindedInputs.Where(item => item.GetListOfParams().Contains(analogInput)).First();
            bindableAnalogInput.SetParameter(inputValue, analogInput);
        }

        public int GetAnalogParameterSimulation(EAnalogOutput analogOutput)
        {
            IBindableAnalogOutput bindableAnalogOutput = ABindedOutputs.Where(item => item.GetListOfParams().Contains(analogOutput)).First();
            return bindableAnalogOutput.GetParamter(analogOutput);
        }

        public void SetDigitalParameterSimulation(EDigitalInput digitalInput, bool inputValue)
        {
            IBindableDigitalInput bindableDigitalInput = DBindedInputs.Where(item => item.ParamsList.Contains(digitalInput)).First();
            bindableDigitalInput.SetDigitalParameter(inputValue, digitalInput);
        }

        public bool GetDigitalParameterSimulation(EDigitalOutput digitalOutput)
        {
            IBindableDigitalOutput bindableDigitalOutput = DBindedOutputs.Where(item => item.ParamsList.Contains(digitalOutput)).First();
            return bindableDigitalOutput.GetDigitalParameter(digitalOutput);
        }

        public async void SendEchoToFindAdapter()
        {
            foreach(var name in USB.GetPortNames())
            {
                USB.PortName = name;
                USB.Open();
                LastOpenedPort = name;
                byte[] echoFrame = Parser.CreateCroppedFrame(Parser.ECommand.Echo, new byte[0]);
                USB.DecoreAndTryWriteFrame(echoFrame);
                await Task.Delay(100);
                USB.Close();
            }
        }
    }
}
