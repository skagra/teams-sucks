using System;
using System.IO.Ports;
using System.Threading;
using System.Text;

public class SerialWriter
{
    private const byte OP_CODE_DEBUG=0xFF;

    private SerialPort _outputPort;

    public SerialWriter(SerialPort serialPort) {
        
        _outputPort=serialPort;
    }

    public void SendHello() {
        _outputPort.Write("H");
    }
}