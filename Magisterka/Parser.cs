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
        private int croppedFrameBytes = 9;

        public enum ECommand : byte
        {
            WriteAll = 0x01,
            WriteOne = 0x02,
            ReadAll = 0x11,
            ReadOne = 0x12,
            AnswerAll = 0x21,
            AnswerOne = 0x22,
        }

        public Parser()
        {

        }

        public event EventHandler<string> SimulationErrorOccured;

        public void ParseCorrectCroppedFrame(object sender, byte[] frame)
        {
            switch(frame[0])
            {
                case (byte)ECommand.AnswerAll:

                    break;
                case (byte)ECommand.AnswerOne:

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
                case ECommand.ReadOne:
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
                case ECommand.WriteOne:
                    if (dataToSend.Length != 3)
                    {
                        throw new ArgumentException("Niewłaściwa liczba danych wejściowych");
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        frame[i + 1] = dataToSend[i];
                    }
                    break;
                default:
                    OnSimulationErrorOccured("Nieobsługiwana komenda wiadomości do wysłania");
                    break;
            }
            return frame;
        }
    }
}
