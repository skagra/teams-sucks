
using System.Net.Sockets;

namespace TeamsSucks
{
   public class Program
   {
      private const string DEVICE_NAME = "HC-05";
      private const string DEVICE_PASSWORD = "1234";

      public static void Main(string[] args)
      {
         using (var client = new BluetoothManager(DEVICE_NAME, DEVICE_PASSWORD))
         {
            var stream = (NetworkStream)client.Connect();
            while (true)
            {
               if (stream.DataAvailable)
               {
                  Console.WriteLine(stream.ReadByte());
               }
            }
         }
      }
   }
}