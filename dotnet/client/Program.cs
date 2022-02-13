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
      private const string BT_DEVICE_NAME="HC-05";
      private const string BT_DEVICE_PASSWORD="1234";

      private const bool USE_BLUETOOTH=false;

      static void Main(string[] Arguments)
      {
         _logger.Info("Teams Sucks");
         
         IComms comms;
         if (USE_BLUETOOTH)
         {
            _logger.Info("Using Bluetooth");
            comms = new BluetoothComms(BT_DEVICE_NAME, BT_DEVICE_PASSWORD);
         }
         else
         {
            _logger.Info("Using USB");
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

         _logger.Info("Ready to talk");

         Console.ReadLine();

         _logger.Info("Closing down");

         _protocolReader.Stop();
      }
   }
}
