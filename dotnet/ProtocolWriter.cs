using System.IO.Ports;
using NLog;

namespace TeamsSucks
{
   public class ProtocolWriter
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

      private const byte OPCODE_ERROR = 0x04;
      private readonly SerialPort _serialPort;

      public ProtocolWriter(SerialPort serialPort)
      {
         _serialPort = serialPort;
      }

      public void SendError()
      {
         _serialPort.Write(new byte[] { 1, OPCODE_ERROR }, 0, 2);
      }

   }
}