using System.IO.Ports;
using NLog;

namespace TeamsSucks
{
   public class ProtocolWriter
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

      private const byte OPCODE_ERROR = 0x04;

      private Action<byte[], int, int> _writer;

      public ProtocolWriter(Action<byte[], int, int> writer)
      {
         _writer = writer;
      }

      public void SendError()
      {
         _writer(new byte[] { 1, OPCODE_ERROR }, 0, 2);
      }
   }
}