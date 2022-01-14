using System.IO.Ports;
using NLog;

namespace TeamsSucks
{
   public class Program
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

      private static ProtocolReader _protocolReader;
      private static ProtocolWriter _protocolWriter;

      private static TeamsController _teamsController = new TeamsController();

      static void ProcessDebugMessage(string message)
      {
         _logger.Debug("Debug: {0}", message);
      }

      static void ProcessToggleMute()
      {
         _logger.Debug("Toggle mute");
         if (!_teamsController.ToggleMute())
         {
            _protocolWriter.SendError();
         }

      }

      static void ProcessToggleCamera()
      {
         _logger.Debug("Toggle camera");
         if (!_teamsController.ToggleCamera())
         {
            _protocolWriter.SendError();
         }
      }

      static void ProcessCycleWindows()
      {
         _logger.Debug("Cycle windows");
         if (!_teamsController.CycleTeamsWindows())
         {
            _protocolWriter.SendError();
         }
      }

      private const int READ_BUFFER_SIZE = 1024;
      private const int WRITE_BUFFER_SIZE = 1024;
      private const int READ_TIMEOUT_MILLIS = 100;
      private const int WRITE_TIMEOUT_MILLIS = 100;

      private const string PORT_NAME = "COM5";
      private const int BAUD_RATE = 9600;
      private const Parity PARITY = Parity.None;
      private const int DATA_BITS = 8;
      private const StopBits STOP_BITS = StopBits.One;

      static void Main(string[] Arguments)
      {
         var serialPort = new SerialPort(PORT_NAME, BAUD_RATE, PARITY, DATA_BITS, STOP_BITS);

         serialPort.ReadBufferSize = READ_BUFFER_SIZE;
         serialPort.WriteBufferSize = WRITE_BUFFER_SIZE;
         serialPort.ReadTimeout = READ_TIMEOUT_MILLIS;
         serialPort.WriteTimeout = WRITE_TIMEOUT_MILLIS;

         serialPort.Open();

         _protocolReader = new ProtocolReader(ProcessCycleWindows, ProcessToggleMute, ProcessToggleCamera, ProcessDebugMessage, serialPort);
         _protocolWriter = new ProtocolWriter(serialPort);

         _protocolReader.Start();

         Console.ReadLine();

         _protocolReader.Stop();
         serialPort.Close();
      }
   }
}