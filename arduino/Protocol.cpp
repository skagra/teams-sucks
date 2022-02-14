#include "Protocol.h"

#ifdef _USE_BT_
#include <SoftwareSerial.h>
#define SERIAL_INF _blueTooth
#else
#define SERIAL_INF (&Serial)
#endif

using namespace TeamsSucks;

Protocol::Protocol(void (*errorCallback)(void *), void *clientData
#ifdef _USE_BT_
   , uint8_t pinBtRx, uint8_t pinBtTx
#endif
)
{
   _errorCallback = errorCallback;
   _clientData = clientData;

#ifdef _USE_BT_
   pinMode(pinBtRx, INPUT);
   pinMode(pinBtTx, OUTPUT);
   _blueTooth = new SoftwareSerial(pinBtRx, pinBtTx);
#endif

   SERIAL_INF->begin(_SERIAL_BAUD_RATE);
}

void Protocol::sendDebug(const char *message)
{
   SERIAL_INF->write(1 + strlen(message));
   SERIAL_INF->write(_OPCODE_DEBUG);
   SERIAL_INF->print(message);
   SERIAL_INF->flush();
}

void Protocol::_sendOpCode(byte opCode)
{
   SERIAL_INF->write(1);
   SERIAL_INF->write(opCode);
   SERIAL_INF->flush();
}

void Protocol::sendCycleWindows()
{
   _sendOpCode(_OPCODE_CYCLE_WINDOWS);
}

void Protocol::sendToggleMute()
{
   _sendOpCode(_OPCODE_TOGGLE_MUTE);
}

void Protocol::sendToggleCamera()
{
   _sendOpCode(_OPCODE_TOGGLE_CAMERA);
}

void Protocol::tick()
{
   byte currentByte;
   if (SERIAL_INF->available())
   {
      currentByte = SERIAL_INF->read();
      if (!_currentlyReading)
      {
         _serialBufferIndex = 0;
         if (currentByte > 0)
         {
            _totalBytesToRead = currentByte;
            _currentlyReading = true;
         }
      }
      else
      {
         serialBuffer[_serialBufferIndex] = currentByte;
         _serialBufferIndex++;
         if (_serialBufferIndex == _totalBytesToRead)
         {
            _currentlyReading = false;
            switch (serialBuffer[0])
            {
            case _OPCODE_ERROR:
               _errorCallback(_clientData);
               break;
            default:
               // ERROR
               break;
            }
         }
      }
   }
}