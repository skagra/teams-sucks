#ifndef _OPCODES_DOT_H_
#define _OPCODES_DOT_H_

#include <Arduino.h>

#ifdef _USE_BT_
#include <SoftwareSerial.h>
#endif

namespace TeamsSucks {
    class Protocol {
        private:
            static const unsigned long _SERIAL_BAUD_RATE = 9600;
            static const int _SERIAL_INF_READ_BUFFER_SIZE = 100;

            static const byte _OPCODE_CYCLE_WINDOWS = 0x01;
            static const byte _OPCODE_TOGGLE_MUTE = 0x02;
            static const byte _OPCODE_TOGGLE_CAMERA = 0x03;
            static const byte _OPCODE_ERROR = 0x04;
            static const byte _OPCODE_DEBUG = 0xFF;

            void (*_errorCallback)(void*);
            void *_clientData;

            byte serialBuffer[_SERIAL_INF_READ_BUFFER_SIZE];
            int _serialBufferIndex = 0;
            bool _currentlyReading = false;
            byte _totalBytesToRead = 0;
#ifdef _USE_BT_
            SoftwareSerial *_blueTooth; 
#endif
            void _sendOpCode(byte opCode);

        public:
            Protocol(void (*_errorCallback)(void*), void *clientData
#ifdef _USE_BT_
                ,
                uint8_t pinBtRx,
                uint8_t pinBtTx
#endif
            );
            void sendDebug(const char *message);
            void sendCycleWindows();
            void sendToggleMute();
            void sendToggleCamera();
            void tick();
    };
}

#endif
