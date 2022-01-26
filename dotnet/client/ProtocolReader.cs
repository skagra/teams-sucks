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
      private readonly Action _cycleWindowsCallback;
      private readonly Action _toggleMuteCallback;
      private readonly Action _toggleCameraCallback;
      private readonly Action<string> _debugCallback;
      private Func<byte[], int, int, int> _reader;

      public ProtocolReader(Action cycleWindowsCallback, Action toggleMuteCallback, Action toggleCameraCallback, Action<string> debugCallback,
          Func<byte[], int, int, int> reader)
      {

         _cycleWindowsCallback = cycleWindowsCallback;
         _toggleMuteCallback = toggleMuteCallback;
         _toggleCameraCallback = toggleCameraCallback;
         _debugCallback = debugCallback;
         _reader = reader;
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
         // stop thread etc.
      }

      private void ReadMessage()
      {
         // Read the size byte
         var byteBuffer = new byte[1];
         _reader(byteBuffer, 0, 1);
         byte sizeByte = byteBuffer[0];

         // Read the rest of the packet
         byte[] buffer = new byte[sizeByte];
         var read = 0;
         while (read < sizeByte)
         {
            read += _reader(buffer, read, sizeByte - read);
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
   }
}