
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using NLog;

namespace TeamsSucks
{
   public class BluetoothManager : IDisposable
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

      private readonly string _deviceName;
      private readonly string _devicePassword;
      private readonly bool _attemptToPair;

      private readonly BluetoothClient _client = new BluetoothClient();
      private Stream? _stream;

      public BluetoothManager(string deviceName, string devicePassword, bool attemptToPair = true)
      {
         _deviceName = deviceName;
         _devicePassword = devicePassword;
         _attemptToPair = attemptToPair;
      }

      private BluetoothAddress? FindPairedDevice()
      {
         BluetoothAddress? result = null;

         _logger.Info("Searching for paired device '{0}'", _deviceName);
         var device = _client.PairedDevices.FirstOrDefault(d => d.DeviceName == _deviceName);
         if (device != null)
         {
            result = device.DeviceAddress;
            _logger.Info("Device found with address '{0}'", result);
         }
         else
         {
            _logger.Info("Device NOT found");
         }

         return result;
      }

      private BluetoothAddress? PairDevice()
      {
         BluetoothAddress? result = null;

         _logger.Info("Searching for device '{0}' to pair", _deviceName);

         var device = _client.DiscoverDevices().FirstOrDefault(d => d.DeviceName == _deviceName);

         if (device != null)
         {
            _logger.Info("Device found with address '{0}'", device.DeviceAddress);

            if (!device.Authenticated)
            {
               _logger.Info("Attempting to pair");

               if (BluetoothSecurity.PairRequest(device.DeviceAddress, _devicePassword))
               {
                  result = device.DeviceAddress;
                  _logger.Info("Pairing successful");
               }
               else
               {
                  _logger.Info("Pairing failed");
               }
            }
         }
         else
         {
            _logger.Info("Device NOT found");
         }

         return result;
      }

      private BluetoothAddress? FindOrPairDevice()
      {
         BluetoothAddress? result = null;

         result = FindPairedDevice();
         if (result == null && _attemptToPair)
         {
            result = PairDevice();
         }

         return result;
      }

      public Stream? Connect()
      {
         if (_stream == null)
         {
            var address = FindOrPairDevice();

            if (address != null)
            {
               _logger.Info("Connecting");
               _client.Connect((BluetoothAddress)address, BluetoothService.SerialPort);
               _stream = _client.GetStream();
               _logger.Info("Connected");
            }
            else
            {
               _logger.Info("Could not connect to device '{0}'", _deviceName);
            }
         }

         return _stream;
      }

      public void Dispose()
      {
         if (_stream != null)
         {
            _stream.Close();
         }
         _client.Close();
         _client?.Dispose();
         GC.SuppressFinalize(this);
      }
   }
}