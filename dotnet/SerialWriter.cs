using System.IO.Ports;

namespace TeamsSucks;

public class SerialWriter
{
   private const byte OPCODE_ERROR = 0x04;
   private readonly SerialPort _serialPort;

   public SerialWriter(SerialPort serialPort)
   {
      _serialPort = serialPort;
   }

   public void SendError()
   {
      _serialPort.Write(new byte[] { 1, OPCODE_ERROR }, 0, 2);
   }

}