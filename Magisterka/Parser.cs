using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public class Parser : INotifyErrorSimulation
    {
        public Parser()
        {
            
        }
        private const int croppedFrameBytes = 10;
        private const int booleanValuesIndex = 9;
        public AdapterManager AdapterManager;
        public enum ECommand : byte
        {
            WriteAll = 0x01,
            WriteOneAn = 0x02,
            WriteOneDi = 0x03,
            ReadAll = 0x11,
            ReadOneAn = 0x12,
            ReadOneDi = 0x13,
            AnswerAll = 0x21,
            AnswerOneAn = 0x22,
            AnswerOneDi = 0x23,
            Echo = 0xAA,
            AnswerEcho = 0x55
        }

        public event EventHandler<string> SimulationErrorOccured;
        public event EventHandler<KeyValuePair<EAnalogInput, int>> AnalogParameterArrived;
        public event EventHandler<KeyValuePair<EDigitalInput, bool>> DigitalParamterArrived;
        public event EventHandler<string> AdapterFound;

        public void ParseCorrectCroppedFrame(object sender, byte[] frame)
        {
            Console.WriteLine("Parsing correctly cropped frame {0}", Encoding.Default.GetString(frame));
            byte byteAfterComand = frame[1];
            switch(frame[0])
            {
                case (byte)ECommand.AnswerAll:
                    foreach(EAnalogInput analogInput in Enum.GetValues(typeof(EAnalogInput)))
                    {
                        int value = Get2BytesValueFromFrame(frame, (byte)(1 + (byte)analogInput * 2));
                        OnAnalogInputValueChange(new KeyValuePair<EAnalogInput, int>(analogInput, value));
                    }
                    foreach(EDigitalInput digitalInput in Enum.GetValues(typeof(EDigitalInput)))
                    {
                        bool value = GetBoolValueFromByte(frame[booleanValuesIndex], (byte)digitalInput); 
                    }
                    break;
                case (byte)ECommand.AnswerOneAn:
                    if(Enum.IsDefined(typeof(EAnalogInput), byteAfterComand))
                    {
                        EAnalogInput parameterNumber = (EAnalogInput)byteAfterComand;
                        int value = Get2BytesValueFromFrame(frame, 2);
                        OnAnalogInputValueChange(new KeyValuePair<EAnalogInput, int>(parameterNumber, value));
                    }
                    break;
                case (byte)ECommand.AnswerOneDi:
                    if (Enum.IsDefined(typeof(EDigitalInput), byteAfterComand))
                    {
                        EDigitalInput parameterNumber = (EDigitalInput)byteAfterComand;
                        bool value = Convert.ToBoolean(frame[2]);
                        OnDigitalInputValueChange(new KeyValuePair<EDigitalInput, bool>(parameterNumber, value));
                    }
                    break;
                case (byte)ECommand.AnswerEcho:
                    AdapterFound?.Invoke(this, AdapterManager.LastOpenedPort);
                    break;
                default:
                    OnSimulationErrorOccured("Nieobsługiwana komenda ramki");
                    break;
            }
        }

        public void OnSimulationErrorOccured(string error)
        {
            //throw new NotImplementedException();
        }

        public byte[] CreateCroppedFrame(ECommand command, params byte[] dataToSend)
        {
            byte[] frame = new byte[croppedFrameBytes];
            frame[0] = (byte)command;
            switch(command)
            {
                case ECommand.ReadAll:
                    break;
                case ECommand.ReadOneAn:
                    if (dataToSend.Length != 1)
                    {
                        throw new ArgumentException("Niewłaściwa liczba danych wejściowych");
                    }
                    frame[1] = dataToSend[0];
                    break;
                case ECommand.WriteAll:
                    if (dataToSend.Length != 8)
                    {
                        throw new ArgumentException("Niewłaściwa liczba danych wejściowych");
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        frame[i + 1] = dataToSend[i];
                    }
                    break;
                case ECommand.WriteOneAn:
                    if (dataToSend.Length != 3)
                    {
                        throw new ArgumentException("Niewłaściwa liczba danych wejściowych");
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        frame[i + 1] = dataToSend[i];
                    }
                    break;
                case ECommand.Echo:
                    break;
                default:
                    OnSimulationErrorOccured("Nieobsługiwana komenda wiadomości do wysłania");
                    break;
            }
            return frame;
        }

        protected virtual void OnAnalogInputValueChange(KeyValuePair<EAnalogInput, int> inputValuePair)
        {
            AnalogParameterArrived?.Invoke(this, inputValuePair);
        }

        protected virtual void OnDigitalInputValueChange(KeyValuePair<EDigitalInput, bool> inputValuePair)
        {
            DigitalParamterArrived?.Invoke(this, inputValuePair);
        }

        private int Get2BytesValueFromFrame(byte[] frame, byte valueIndex)
        {
            const int BytesInInt = 4;
            const int BytesInUint16 = 2;
            byte[] croppedArray = new byte[BytesInInt];
            Array.Copy(frame, valueIndex, croppedArray, BytesInInt - BytesInUint16, BytesInUint16);
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(croppedArray);
            int output = BitConverter.ToInt32(croppedArray, 0);
            return output;
        }

        private bool GetBoolValueFromByte(byte allValues, byte valueIndex)
        {
            byte mask = (byte)(1 << valueIndex);
            bool output = (allValues & mask) != 0;
            return output;
        }
    }
}
