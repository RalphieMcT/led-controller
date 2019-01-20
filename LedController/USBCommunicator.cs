using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace Nightlight
{
    public static class USBCommunicator
    {
        public static void SendCommand(string command)
        {
            command += '\r';
            char[] data = command.ToCharArray();
            using (var com = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One))
            {
                com.Open();
                com.Write(data, 0, data.Length);
            }
        }
    }
}
