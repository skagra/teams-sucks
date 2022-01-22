using System.IO.Ports;
using NLog;
using System.Management;
using System.Text.RegularExpressions;

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

        private const int BAUD_RATE = 9600;
        private const Parity PARITY = Parity.None;
        private const int DATA_BITS = 8;
        private const StopBits STOP_BITS = StopBits.One;

        private static readonly string[] ARDUINO_NAMES = { "CH340" };

#pragma warning disable CA1416 // Validate platform compatibility
        private static string FindAnduinoComPort()
        {
            string result = null;
            var regex = new Regex($"USB-SERIAL ({string.Join("|", ARDUINO_NAMES)}) \\((.+)\\)");

            var scope = new ManagementScope();
            var query = new SelectQuery("SELECT * FROM Win32_PnPEntity");
            var searcher = new ManagementObjectSearcher(scope, query);

            foreach (var item in searcher.Get())
            {
                if (((string)item["PNPClass"]) == "Ports")
                {
                    var name = (string)item["Name"];
                    if (name != null)
                    {
                        var parts = regex.Match(name);
                        if (parts.Success)
                        {
                            result = parts.Groups[2].Value;
                        }
                    }
                }
            }

            return result;
        }
#pragma warning restore CA1416 // Validate platform compatibility

        static void Main(string[] Arguments)
        {
            _logger.Info($"Searching for Arduino's named {string.Join(", ", ARDUINO_NAMES)}");
            var portName = FindAnduinoComPort();
            if (portName != null)
            {
                _logger.Info($"Arduino found on port {portName}");

                var serialPort = new SerialPort(portName, BAUD_RATE, PARITY, DATA_BITS, STOP_BITS)
                {
                    ReadBufferSize = READ_BUFFER_SIZE,
                    WriteBufferSize = WRITE_BUFFER_SIZE,
                    ReadTimeout = READ_TIMEOUT_MILLIS,
                    WriteTimeout = WRITE_TIMEOUT_MILLIS
                };

                serialPort.Open();

                _protocolReader = new ProtocolReader(
                    ProcessCycleWindows,
                    ProcessToggleMute,
                    ProcessToggleCamera,
                    ProcessDebugMessage,
                    serialPort
                );
                _protocolWriter = new ProtocolWriter(serialPort);

                _protocolReader.Start();

                _logger.Info("Listening for protocol from Arduino");

                Console.ReadLine();

                _logger.Info("Closing down");

                _protocolReader.Stop();
                serialPort.Close();
            }
            else
            {
                 _logger.Error("Arduino NOT found.");
            }
        }
    }
}
