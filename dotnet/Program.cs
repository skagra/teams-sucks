using System;
using System.IO.Ports;
using System.Threading;

namespace TeamsSucks
{
    class Program
    {
        private static readonly TeamsController _teamsController=new TeamsController();

        static void ProcessDebugMessage(string message) {
            Console.WriteLine($"Debug: {message}");
        }

        static void ProcessToggleMute() {
            Console.WriteLine("Toggle mute");
            _teamsController.ToggleMute();
        }

        static void ProcessToggleCamera() {
            Console.WriteLine("Toggle camera");
            _teamsController.ToggleCamera();
        }

        static void ProcessBringToFront() {
            Console.WriteLine("Bring to front");
            _teamsController.BringToFront();
        }

        private const int READ_BUFFER_SIZE=1024;
        private const int WRITE_BUFFER_SIZE=1024;
        private const int READ_TIMEOUT_MILLIS=100;
        private const int WRITE_TIMEOUT_MILLIS=100;

        private const string PORT_NAME="COM3";
        private const int BAUD_RATE=9600;
        private const Parity PARITY=Parity.None;
        private const int DATA_BITS=8;
        private const StopBits STOP_BITS=StopBits.One;

        static void Main(string[] Arguments)
        {
            var inputPort = new SerialPort(PORT_NAME, BAUD_RATE, PARITY, DATA_BITS, STOP_BITS);

            inputPort.ReadBufferSize=READ_BUFFER_SIZE;
            inputPort.WriteBufferSize=WRITE_BUFFER_SIZE;
            inputPort.ReadTimeout=READ_TIMEOUT_MILLIS;
            inputPort.WriteTimeout=WRITE_TIMEOUT_MILLIS;

            inputPort.Open();

            var reader=new SerialReader(ProcessBringToFront, ProcessToggleMute, ProcessToggleCamera, ProcessDebugMessage, inputPort);    
            reader.Start();

        // var writer=new SerialWriter(inputPort);
            Console.ReadLine();
        
            reader.Stop();
            inputPort.Close();
        }
    }
}