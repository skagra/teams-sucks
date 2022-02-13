#include "protocol.h"

using namespace TeamsSucks;

#undef _USE_BT_

#ifdef _USE_BT_
#include <SoftwareSerial.h>
#endif

#ifdef _USE_BT_
const uint8_t PIN_BT_RX = 12;
const uint8_t PIN_BT_TX = 13;
SoftwareSerial blueTooth = SoftwareSerial(PIN_BT_RX, PIN_BT_TX);
#define SERIAL_INF blueTooth
#else
#define SERIAL_INF Serial
#endif

Protocol::Protocol() {
   SERIAL_INF.begin(_SERIAL_BAUD_RATE);

#ifdef _USE_BT_ 
    pinMode(PIN_BT_RX, INPUT);
    pinMode(PIN_BT_TX, OUTPUT);
#endif
}

void Protocol::sendDebug(const char *message)
{
   SERIAL_INF.write(1 + strlen(message));
   SERIAL_INF.write(_OPCODE_DEBUG);
   SERIAL_INF.print(message);
   SERIAL_INF.flush();
}

void Protocol::_sendOpCode(byte opCode)
{
   SERIAL_INF.write(1);
   SERIAL_INF.write(opCode);
   SERIAL_INF.flush();
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

void Protocol::tick() {
    
}

// void Protocol::handleError()
// {
//    tune.play(errorFrequencies, errorDurations, sizeof(errorFrequencies) / sizeof(unsigned int));
// }

// void readAndDispatchTick()
// {
//    byte currentByte;
//    if (SERIAL_INF.available())
//    {
//       currentByte = SERIAL_INF.read();
//       if (!currentlyReading)
//       {
//          serialBufferIndex = 0;
//          if (currentByte > 0)
//          {
//             totalBytesToRead = currentByte;
//             currentlyReading = true;
//          }
//       }
//       else
//       {
//          serialBuffer[serialBufferIndex] = currentByte;
//          serialBufferIndex++;
//          if (serialBufferIndex == totalBytesToRead)
//          {
//             currentlyReading = false;
//             switch (serialBuffer[0])
//             {
//             case OPCODE_ERROR:
//                handleError();
//                break;
//             default:
//                // ERROR
//                break;
//             }
//          }
//       }
//    }
// }