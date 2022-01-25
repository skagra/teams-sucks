
namespace TeamSucks
{
    public class Program
    {
        private const string DEVICE_NAME = "HC-05";
        private const string DEVICE_PASSWORD = "1234";

        public static void Main(string[] args)
        {
            using (var client = new BluetoothManager(DEVICE_NAME, DEVICE_PASSWORD))
            {
                var stream = client.Connect();
                var reader = new StreamReader(stream, System.Text.Encoding.ASCII);

                Console.WriteLine("Listening");
                while (true)
                {
                    Console.WriteLine(reader.ReadLine());
                }
            }
        }
    }
}