#ifndef _OPCODES_DOT_H_
#define _OPCODES_DOT_H_

#include <Arduino.h>

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

            byte serialBuffer[_SERIAL_INF_READ_BUFFER_SIZE];
            int _serialBufferIndex = 0;
            bool _currentlyReading = false;
            byte _totalBytesToRead = 0;

            void _sendOpCode(byte opCode);

        public:
            Protocol();
            void sendDebug(const char *message);
            void sendCycleWindows();
            void sendToggleMute();
            void sendToggleCamera();
        // void handleError();
            void tick();
    };
}

#endif
