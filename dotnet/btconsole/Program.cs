
using System.Net.Sockets;

namespace TeamsSucks
{
   public class Program
   {
      private const string DEVICE_NAME = "HC-05";
      private const string DEVICE_PASSWORD = "1234";

      public static void Main()
      {
         using (var client = new BluetoothManager(DEVICE_NAME, DEVICE_PASSWORD))
         {
            var stream = client.Connect() as NetworkStream ?? throw new ArgumentNullException(nameof(client));
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