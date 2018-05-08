using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class Parser : INotifyErrorSimulation
    {
        private int croppedFrameBytes = 9;

        private enum ECommand : byte
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

        public void ParseCorrectFrame(object sender, byte[] frame)
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
                    //throw new NotSupportedException("Nieobsługiwana komenda ramki");

            }
        }

        public void OnSimulationErrorOccured(string error)
        {
            //throw new NotImplementedException();
        }

        /*public byte[] createFrame(ECommand command, params byte[]  dataToSend)
        {
            byte[] frame = new byte[croppedFrameBytes];
            frame[0] = (byte)command;
            switch(command)
            {
                case ECommand.ReadAll:

            }
        }*/
    }
}
