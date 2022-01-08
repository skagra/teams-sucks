using System;
using System.IO.Ports;
using System.Threading;
using System.Text;

public class SerialReader 
{
    private const byte OPCODE_BRING_TO_FRONT=0x01;
    private const byte OPCODE_TOGGLE_MUTE=0x02;
    private const byte OPCODE_TOGGLE_CAMERA=0x03;
    private const byte OPCODE_DEBUG=0xFF;

    private Task _readerTask;

    private SerialPort _inputPort;

    private readonly Action _toggleMuteCallback;
    private readonly Action<string> _debugCallback;

    public SerialReader(Action toggleMuteCallback, Action<string> debugCallback, SerialPort serialPort) {
        
        _toggleMuteCallback=toggleMuteCallback;
        _debugCallback=debugCallback;
        _inputPort=serialPort;
    }
    
    public void Start()
    {
        _readerTask=Task.Run(() => {
            while (true) {
                ReadMessage();
            }    
        });
    }

    public void Stop()
    {
        _inputPort.Dispose();
        _inputPort = null;
        // stop thread etc.
    }

    private void ReadMessage()
    {
        if (_inputPort.BytesToRead > 0)
        {
            // Get length of packet (not including length byte)
            byte sizeByte = (byte)_inputPort.ReadByte();
            byte[] buffer=new byte[sizeByte];
 
            try {
                // Read the rest of the packet
                var read=0;
                while (read<sizeByte) {
                    read+=_inputPort.Read(buffer, read, sizeByte-read);
                }

                // Get the op code
                byte opCode=buffer[0];

                Console.WriteLine(BitConverter.ToString(buffer));

                switch (opCode) {
                    case OPCODE_DEBUG:
                        _debugCallback(new ASCIIEncoding().GetString(buffer, 1, sizeByte-1));
                        break;
                    case OPCODE_TOGGLE_MUTE:
                        _toggleMuteCallback();
                        break;
                    default:
                        Console.WriteLine("ERROR");
                        break;
                }
            } catch (TimeoutException e) {
                Console.WriteLine("READ TIMEOUT");
            }
        }
    }
}