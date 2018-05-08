using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public class USB : SerialPort, INotifyErrorSimulation
    {
        private int frameBytes = 12;
        private readonly string DeviceName = "HVACBoard";

        private const byte StartByte = 0xFE;
        private const byte EndByte = 0xF0;

        public event EventHandler<string> SimulationErrorOccured;
        public event EventHandler<byte[]> CorrectFrameRead;

        public USB()
        {
            BaudRate = 9600;
            Parity = Parity.None;
            StopBits = StopBits.One;
            DataBits = 8;
            DataReceived += USBDataReceived;
            ReadTimeout = 100;
            WriteTimeout = 100;
        }

        private void USBDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[frameBytes];
            try
            {
                while(true)
                {
                    Read(buffer, 0, 1);
                    if (buffer[0] == StartByte) break;
                }
                for(int i = 1; i < frameBytes; i++)
                {
                    Read(buffer, i, 1);
                }
                if(buffer.Last() == EndByte)
                {
                    byte[] croppedFrame = new byte[frameBytes - 3];
                    Array.Copy(buffer, 1, croppedFrame, 0, frameBytes - 3);
                    byte crcPC = CRC8(croppedFrame);
                    byte crcDevice = croppedFrame.Last();
                    if(crcDevice == crcPC)
                    {
                        OnCorrectFrameRead(buffer);
                    }
                }
            }
            catch(TimeoutException)
            {
                OnSimulationErrorOccured("Timeout odbioru USB");
            }
            catch(InvalidOperationException)
            {
                OnSimulationErrorOccured("Port nie jest otwarty");
            }
        }

        public bool FindDevice()
        {
            string[] availablePorts = GetPortNames();
            foreach(string name in availablePorts)
            {
                if(name == DeviceName)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnSimulationErrorOccured(string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
            MessageBox.Show(error);
        }

        protected void OnCorrectFrameRead(byte[] frame)
        {
            CorrectFrameRead?.Invoke(this, frame);
        }

        public bool TryWriteFrame(byte[] frame)
        {
            if (frame.Length == 0) return false;
            try
            {
                Write(frame, 0, frame.Length);
                return true;
            }
            catch(TimeoutException)
            {
                OnSimulationErrorOccured("Timeout wysyłania USB");
                return false;
            }
            catch(InvalidOperationException)
            {
                OnSimulationErrorOccured("Port nie jest otwarty");
                return false;
            }
        }

        private byte CRC8(byte[] frame)
        {
            byte crc = 0;
            for (int i = 0; i < frame.Length; ++i)
            {
                byte inbyte = frame[i];
                for (byte j = 0; j < 8; ++j)
                {
                    byte mix = (byte)((crc ^ inbyte) & 0x01);
                    crc >>= 1;
                    if (mix != 0) crc ^= 0x8C;
                    inbyte >>= 1;
                }
            }
            return crc;
        }
    }
}
