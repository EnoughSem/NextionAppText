using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextionApp
{
    class Program
    {
         
        static void Main(string[] args)
        {
            SerialPort serialPort = new SerialPort();

            var hour = DateTime.Now.ToString("HH");
            var min = DateTime.Now.ToString("mm");
            var sec = DateTime.Now.ToString("ss");

            serialPort.PortName = "COM3";
            serialPort.BaudRate = 9600;
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();

            byte[] bytesEnd = new byte[] { 0xff, 0xff, 0xff};

            serialPort.Write("rtc3="+hour);
            serialPort.Write(bytesEnd,0, bytesEnd.Length);

            serialPort.Write("rtc4=" + min);
            serialPort.Write(bytesEnd, 0, bytesEnd.Length);

            serialPort.Write("rtc5=" + sec);
            serialPort.Write(bytesEnd, 0, bytesEnd.Length);

            Console.ReadLine();
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var _serialPort = sender as SerialPort;
            if (_serialPort != null)
            {
                string command = _serialPort.ReadLine();
                if (command.Contains("pass"))
                {
                    byte[] bytesEnd = new byte[] { 0xff, 0xff, 0xff };
                    string subCommand = command.Substring(command.IndexOf("pass")+4, 4);
                    if (subCommand == "1234")
                    {
                        _serialPort.Write("page MainPage");
                        _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                    }
                    else
                    {
                        _serialPort.Write("t0.txt= "+ "");
                        _serialPort.Write(bytesEnd, 0, bytesEnd.Length);
                       
                    }
                }
                
            }
        }

    }
}
