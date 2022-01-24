
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

public class Program
{
    private const string DEVICE_NAME = "HC-05";

    public static void Main(string[] args)
    {
        var client = new BluetoothClient();

        Console.Write($"Searching for {DEVICE_NAME}...");
        BluetoothDeviceInfo arduino = null;
        foreach(var device in client.DiscoverDevices())
        {
            if (device.DeviceName == DEVICE_NAME)
            {
                arduino = device;
                break;
            }
        }

        if (arduino != null)
        {
            Console.WriteLine("found");

            Console.WriteLine($"Address: {arduino.DeviceAddress}");

            if (!arduino.Authenticated)
            {
                BluetoothSecurity.PairRequest(arduino.DeviceAddress, "1234");
            }

            arduino.Refresh();

            Console.Write("Connecting...");
            client.Connect(arduino.DeviceAddress, BluetoothService.SerialPort);
            Console.WriteLine("connected");

            var stream = client.GetStream();
           
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.ASCII);
            
            Console.WriteLine("Listening");
            while (true)
            {
                Console.WriteLine(reader.ReadLine());
            }
            reader.Close();
        }
        else
        {
            Console.WriteLine("NOT found");
        }
    }
}