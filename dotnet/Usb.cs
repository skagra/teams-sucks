using System;
using System.IO.Ports;
using System.Threading;


class Programxx
{
    static void Mainxx(string[] Arguments)
    {
        PinReader Reader = new PinReader();
        
        Reader.Start("COM3", 9600, 8, Parity.None, StopBits.One);

        while (true)
        {
            PinReader.PinEvent CurrentEvent = Reader.Tick();

            if (CurrentEvent != null)
            {
                Console.WriteLine($"Pin number: { CurrentEvent.PinNumber } -- IsActivatedEvent: { CurrentEvent.IsActivatedEvent }");
            }

            Thread.Sleep(1);
        }
    }
}

public class PinReader
{
    public class PinEvent
    {
        public int PinNumber;

        public bool IsActivatedEvent;

        public PinEvent(int PinNumber, bool IsActivatedEvent)
        {
            this.PinNumber = PinNumber;

            this.IsActivatedEvent = IsActivatedEvent;
        }
    }

    const byte SentinelByte = 255;

    bool SentinelByteSeen, PinNumberByteSeen;

    int CurrentPinNumber;

    SerialPort InputPort;

    public void Start(string PortName, int BaudRate, int DataBitCount, Parity ParitySetting, StopBits StopBitSetting)
    {
        InputPort = new SerialPort(PortName, BaudRate, ParitySetting, DataBitCount, StopBitSetting);

        InputPort.ReadBufferSize = 1024 * 1024 * 4;

        InputPort.Open();
    }

    public void Stop()
    {
        InputPort.Dispose();

        InputPort = null;
    }

    public PinEvent Tick()
    {
        while (InputPort.BytesToRead > 0)
        {
            int CurrentByte = InputPort.ReadByte();

            if (CurrentByte == SentinelByte)
            {
                SentinelByteSeen = true;

                PinNumberByteSeen = false;

                continue;
            }

            if (SentinelByteSeen)
            {
                if (PinNumberByteSeen)
                {
                    SentinelByteSeen = false;

                    bool IsActivatedEvent = (CurrentByte == 0) ? (false) : (true);

                    return (new PinEvent(CurrentPinNumber, IsActivatedEvent));
                }

                PinNumberByteSeen = true;

                CurrentPinNumber = CurrentByte;
            }
        }

        return (null);
    }

    public void Reset()
    {
        while (InputPort.BytesToRead > 0)
        {
            InputPort.ReadByte();
        }

        SentinelByteSeen = false;
    }
}