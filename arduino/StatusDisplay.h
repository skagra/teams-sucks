#ifndef _STATUS_DISPLAY_DOT_H_
#define _STATUS_DISPLAY_DOT_H_

#include <LiquidCrystal.h>

namespace TeamsSucks {
    class StatusDisplay {
        private:
            static const int _helpThresholdMillis = 3000;
            static const unsigned long _statusPersisenceThresholdMillis = 1000;
            
            const char *_LCD_WIPE_LINE = "                ";
            const char **_helpMessages;

            int _currentHelpIndex = 0;
            int _numHelpMessages;
            unsigned long _lastChangedMillis = 0;
            unsigned long _statusLastSet = 0;
            bool _statusIsBlank = true;

            LiquidCrystal *_lcd;
            void _updateHelpMessage();
            void _updateStatusMessage();
            void _setHelpMessage(const char* message);
        public:
            StatusDisplay(const char **helpMessages, int numHelpMessages, uint8_t pinRegisterSelect, uint8_t pinLcdEnable, 
                uint8_t pinLcdData5, uint8_t pinLcdData6, uint8_t pinLcdData7, uint8_t pinLcdData8);
            void setStatusMessage(const char *message);
            void tick();
    };
}

#endif
