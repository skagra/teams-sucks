using System.Net.Sockets;

namespace TeamsSucks
{
   public class BluetoothComms : IComms
   {
      private readonly NetworkStream _btStream;
      private readonly BluetoothManager _btManager;

      public BluetoothComms(string deviceName, string devicePassword)
      {
         var _btManager = new BluetoothManager(deviceName, devicePassword);
         _btStream = (NetworkStream)_btManager.Connect();
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
            catch (IOException)
            {
               // Ignore
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