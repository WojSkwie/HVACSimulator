using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

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

        public void Connect(string portName)
        {
            if (!VerifyPortName(portName)) return;
            USB.PortName = portName;
            USB.OpenWithEvent();
        }

        public void Disconnect()
        {
            USB.CloseWithEvent();
        }


        public AdapterManager(EventHandler<byte[]> correctFrameHandler, EventHandler<bool> stateChangedHandler)
        {
            USB.CorrectFrameRead += correctFrameHandler;
            USB.StateChanged += stateChangedHandler;
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
            //throw new NotImplementedException(); TODO
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
                try
                {
                    USB.PortName = name;
                    USB.Open();
                    LastOpenedPort = name;
                    byte[] echoFrame = Parser.CreateCroppedFrame(Parser.ECommand.Echo, new byte[0]);
                    USB.DecoreAndTryWriteFrame(echoFrame);
                    await Task.Delay(100);
                    USB.Close();
                }
                catch (IOException)
                {
                    OnSimulationErrorOccured("Port znajduje się w nieprawidłowym stanie");
                }
                catch(ArgumentException)
                {
                    OnSimulationErrorOccured("Obiekt jest nullem");
                }
                catch(InvalidOperationException)
                {
                    OnSimulationErrorOccured("Port jest w nieprawidłowym stanie");
                }
                catch(Exception e)
                {
                    OnSimulationErrorOccured(string.Format("Nieprzewidziany błąd portu szeregowego: {0}", e));
                }
                
            }
        }

        public bool VerifyPortName(string portName)
        {
            List<string> availablePorts = USB.GetPortNames().ToList();
            if (availablePorts.Contains(portName)) return true;
            else
            {
                MessageBox.Show("Niepoprawna nazwa portu");
                return false;
            }
        }

        public void SendRequestForData()
        {
            byte[] innerFrame = Parser.CreateCroppedFrame(Parser.ECommand.ReadAll, new byte[0]);
            USB.DecoreAndTryWriteFrame(innerFrame);
        }

        public void CreateAndSendDataToAdapter()
        {
            byte[] WholeData = new byte[9];
            foreach(EAnalogOutput analogOutputType in Enum.GetValues(typeof(EAnalogOutput)))
            {
                int value = GetAnalogParameterSimulation(analogOutputType);
                Array.Copy(Parser.GetBytesFromInt(value), 0, WholeData, 1 + (int)analogOutputType * 2, 0);
            }
            
            foreach(EDigitalOutput digitalOutput in Enum.GetValues(typeof(EDigitalOutput)))
            {
                bool value = GetDigitalParameterSimulation(digitalOutput);
                WholeData[8] = Parser.SetBitValueInByte(value, (int)digitalOutput, WholeData[8]);
            }
            byte[] innerFrame = Parser.CreateCroppedFrame(Parser.ECommand.WriteAll, WholeData);
            USB.DecoreAndTryWriteFrame(innerFrame);
        }
    }
}
