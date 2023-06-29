using System;
using System.IO.Ports;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace NextionApp
{
    class Program
    {
        static byte[] bytesEnd = new byte[] { 0xff, 0xff, 0xff };
        static void Main(string[] args)
        {
            serialPort();
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
                    checkPassword(_serialPort, command);
                }
                if (command.Contains("time"))
                {
                    getTime(_serialPort);
                }
                if (command.Contains("date"))
                {
                    dateReserv(_serialPort);
                }
                if (command.Contains("addDate"))
                {
                    addData(_serialPort, command);
                }
                if (command.Contains("addTime"))
                {
                    addTime(_serialPort, command);
                }
            }
        }

        private static void serialPort()
        {
                SerialPort serialPort = new SerialPort();        
                serialPort.PortName = "COM3";
                serialPort.BaudRate = 9600;
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                getTime(serialPort);   
        }
        
        private static void getTime(SerialPort _serialPort)
        {
            var hour = DateTime.Now.ToString("HH");
            var min = DateTime.Now.ToString("mm");
            var sec = DateTime.Now.ToString("ss");

            byte[] bytesEnd = new byte[] { 0xff, 0xff, 0xff };

            _serialPort.Write("rtc3=" + hour);
            _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

            _serialPort.Write("rtc4=" + min);
            _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

            _serialPort.Write("rtc5=" + sec);
            _serialPort.Write(bytesEnd, 0, bytesEnd.Length);
        }

        private static void checkPassword(SerialPort _serialPort, string command)
        {
            string subCommand = command.Substring(command.IndexOf("pass") + 4, 4);
            if (subCommand == "1234")
            {
                _serialPort.Write("page MainPage");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                dateReserv(_serialPort);
            }
            else
            {
                _serialPort.Write("t0.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t1.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t2.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t3.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);
            }
        }


        private static void dateReserv(SerialPort _serialPort)
        {
            var timeReserv = DbConnection.GetDate();
            if (timeReserv == null)
            {
                Console.WriteLine("Пусто");
            }
            if (timeReserv != null)
            {
                if (timeReserv.Length == 2)
                {
                    _serialPort.Write("t2.txt=\"" + timeReserv[0].ToString() + "\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                    _serialPort.Write("t5.txt=\"" + timeReserv[1].ToString() + "\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);
                }
                else
                {
                    _serialPort.Write("t2.txt=\"Нет\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                    _serialPort.Write("t5.txt=\"Нет\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);
                }
            }
            else
            {
                _serialPort.Write("t2.txt=\"Нет\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t5.txt=\"Нет\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);
            }
        }


        private static void addData(SerialPort _serialPort, string command)
        {
            int day = int.Parse(command.Substring(command.IndexOf("addDate") + 7, 2));
            int mouth = int.Parse(command.Substring(command.IndexOf("addDate") + 9, 2));
            int year =int.Parse("20"+command.Substring(command.IndexOf("addDate") + 11, 2));
            try
            {
                DateTime date = new DateTime(year, mouth, day);

                _serialPort.Write("page AddTimePage");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t0.txt=\"" + day.ToString() + "\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t1.txt=\"" + mouth.ToString() + "\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t2.txt=\""+ year.ToString() +"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

            }
            catch
            {
                _serialPort.Write("t0.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t1.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t2.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t3.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t4.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t5.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);
            }
            Console.WriteLine(day);
            Console.WriteLine(mouth);
            Console.WriteLine(year);
        }

        private static void addTime(SerialPort _serialPort, string command)
        {
            string times = command.Substring(command.IndexOf("addTime") + 7);
            string[] array = times.Split(' ');

            int day = int.Parse(array[0]);
            int mouth = int.Parse(array[1]);
            int year = int.Parse(array[2]);

            int hourStart = int.Parse(array[3]);
            int minStart = int.Parse(array[4]);

            int hourEnd = int.Parse(array[5]);
            int minEnd = int.Parse(array[6]);

            try
            {
                DateTime dateStart = new DateTime(year, mouth, day, hourStart, minStart, 00);
                DateTime dateEnd = new DateTime(year, mouth, day, hourEnd, minEnd, 00);

                if (DbConnection.AddTime(dateStart, dateEnd))
                {
                    Console.WriteLine(dateStart);
                    Console.WriteLine(dateEnd);
                    _serialPort.Write("page FinalPage");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);
                }
                else
                {
                    _serialPort.Write("t3.txt=\"\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                    _serialPort.Write("t4.txt=\"\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                    _serialPort.Write("t5.txt=\"\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                    _serialPort.Write("t6.txt=\"\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                    _serialPort.Write("t7.txt=\"\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                    _serialPort.Write("t8.txt=\"\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                    _serialPort.Write("t9.txt=\"\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                    _serialPort.Write("t10.txt=\"\"");
                    _serialPort.Write(bytesEnd, 0, bytesEnd.Length);
                }

                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);

                _serialPort.Write("t3.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t4.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t5.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t6.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t7.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t8.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t9.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);

                _serialPort.Write("t10.txt=\"\"");
                _serialPort.Write(bytesEnd, 0, bytesEnd.Length);
            }
        }
    }
}
