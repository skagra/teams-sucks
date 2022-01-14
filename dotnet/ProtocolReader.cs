using System.IO.Ports;
using System.Text;
using NLog;

namespace TeamsSucks
{
   public class ProtocolReader
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

      private const byte OPCODE_CYCLE_WINDOWS = 0x01;
      private const byte OPCODE_TOGGLE_MUTE = 0x02;
      private const byte OPCODE_TOGGLE_CAMERA = 0x03;
      private const byte OPCODE_DEBUG = 0xFF;

      private readonly SerialPort _serialPort;

      private readonly Action _cycleWindowsCallback;
      private readonly Action _toggleMuteCallback;
      private readonly Action _toggleCameraCallback;
      private readonly Action<string> _debugCallback;

      public ProtocolReader(Action cycleWindowsCallback, Action toggleMuteCallback, Action toggleCameraCallback, Action<string> debugCallback, SerialPort serialPort)
      {

         _cycleWindowsCallback = cycleWindowsCallback;
         _toggleMuteCallback = toggleMuteCallback;
         _toggleCameraCallback = toggleCameraCallback;
         _debugCallback = debugCallback;
         _serialPort = serialPort;
      }

      public void Start()
      {
         Task.Run(() =>
         {
            while (true)
            {
               try
               {
                  ReadMessage();
               }
               catch (Exception e)
               {
                  _logger.Error(e);
               }
            }
         });
      }

      public void Stop()
      {
         _serialPort.Dispose();
         //_inputPort = null;
         // stop thread etc.
      }

      private void ReadMessage()
      {
         if (_serialPort.BytesToRead > 0)
         {
            // Get length of packet (not including length byte)
            byte sizeByte = (byte)_serialPort.ReadByte();
            byte[] buffer = new byte[sizeByte];

            try
            {
               // Read the rest of the packet
               var read = 0;
               while (read < sizeByte)
               {
                  read += _serialPort.Read(buffer, read, sizeByte - read);
               }

               // Get the op code
               byte opCode = buffer[0];

               _logger.Debug("Incoming protocol '{0}'", (BitConverter.ToString(buffer)));

               switch (opCode)
               {
                  case OPCODE_CYCLE_WINDOWS:
                     _cycleWindowsCallback();
                     break;
                  case OPCODE_TOGGLE_MUTE:
                     _toggleMuteCallback();
                     break;
                  case OPCODE_TOGGLE_CAMERA:
                     _toggleCameraCallback();
                     break;
                  case OPCODE_DEBUG:
                     _debugCallback(new ASCIIEncoding().GetString(buffer, 1, sizeByte - 1));
                     break;
                  default:
                     _logger.Error("Invalid OPCODE {0}", opCode);
                     break;
               }
            }
            catch (TimeoutException)
            {
               _logger.Error("Read timeout");
            }
         }
      }
   }
}