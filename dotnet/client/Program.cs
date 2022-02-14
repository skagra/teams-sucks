using NLog;

namespace TeamsSucks
{
   public class Program
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
      private static readonly TeamsController _teamsController = new();
      private static ProtocolReader? _protocolReader;
      private static ProtocolWriter? _protocolWriter;
      static void ProcessDebugMessage(string message)
      {
         _logger.Debug("Debug: {0}", message);
      }

      static void ProcessToggleMute()
      {
         _logger.Debug("Toggle mute");
         if (!_teamsController.ToggleMute())
         {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _protocolWriter.SendError();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
         }
      }

      static void ProcessToggleCamera()
      {
         _logger.Debug("Toggle camera");
         if (!_teamsController.ToggleCamera())
         {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _protocolWriter.SendError();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
         }
      }

      static void ProcessCycleWindows()
      {
         _logger.Debug("Cycle windows");
         if (!_teamsController.CycleTeamsWindows())
         {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _protocolWriter.SendError();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
         }
      }

      private static readonly string[] ARDUINO_NAMES = { "CH340" };
      private const string BT_DEVICE_NAME = "HC-05";
      private const string BT_DEVICE_PASSWORD = "1234";

      private enum CommsMode { BLUETOOTH, SERIAL };

      static void Main(string[] args)
      {
         _logger.Info("Teams Sucks");

         CommsMode commsMode = CommsMode.SERIAL;
         var numArgs = args.Length;
         if (numArgs > 0)
         {
            commsMode = args[0] switch
            {
               "--bt" => CommsMode.BLUETOOTH,
               "--serial" => CommsMode.SERIAL,
               _ => throw new ArgumentException("First flag must be --bt or --serial"),
            };
         }

         IComms comms;
         if (commsMode == CommsMode.BLUETOOTH)
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
