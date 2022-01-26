using NLog;
namespace TeamsSucks
{
   public class Program
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
      private static ProtocolReader? _protocolReader;
      private static ProtocolWriter? _protocolWriter;
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

      private static readonly string[] ARDUINO_NAMES = { "CH340" };

      static void Main(string[] Arguments)
      {
         _logger.Info($"Searching for Arduino named {string.Join(", ", ARDUINO_NAMES)}");

         bool useBluetooth = true;

         IComms comms;
         if (useBluetooth)
         {
            comms = new BluetoothComms("HC-05", "1234");
         }
         else
         {
            comms = new SerialComms(ARDUINO_NAMES, 9600);
         }

         _protocolReader = new ProtocolReader(
             ProcessCycleWindows,
             ProcessToggleMute,
             ProcessToggleCamera,
             ProcessDebugMessage,
             comms.Read
         );
         _protocolWriter = new ProtocolWriter(comms.Write);

         _protocolReader.Start();

         _logger.Info("Listening for protocol from Arduino");

         Console.ReadLine();

         _logger.Info("Closing down");

         _protocolReader.Stop();
      }
   }
}
