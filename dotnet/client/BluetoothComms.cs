using System.Net.Sockets;
using NLog;

namespace TeamsSucks
{
   public class BluetoothComms : IComms
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
      private readonly NetworkStream _btStream;
      private readonly BluetoothManager _btManager;

      public BluetoothComms(string deviceName, string devicePassword)
      {
         _btManager = new BluetoothManager(deviceName, devicePassword);
         _btStream = _btManager.Connect() as NetworkStream ?? throw new InvalidCastException();
      }

      public int Read(byte[] buffer, int offset, int count)
      {
         int result = 0;
         while (result == 0)
         {
            try
            {
               // Blocks forever by default
               result = _btStream.Read(buffer, offset, count);
            }
            catch (IOException e)
            {
               _logger.Warn(e.Message);
            }
         }

         return result;
      }

      public void Write(byte[] buffer, int offset, int count)
      {
         _btStream.Write(buffer, offset, count);
      }
   }
}