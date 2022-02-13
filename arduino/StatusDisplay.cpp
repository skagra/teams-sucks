#include "StatusDisplay.h"

#include <Arduino.h>

using namespace TeamsSucks;

StatusDisplay::StatusDisplay(const char **helpMessages, int numHelpMessages,
   uint8_t pinRegisterSelect, uint8_t pinLcdEnable,
   uint8_t pinLcdData5, uint8_t pinLcdData6, uint8_t pinLcdData7, uint8_t pinLcdData8)
{
   _helpMessages=helpMessages;
   _numHelpMessages=numHelpMessages;
   
   _lcd = new LiquidCrystal(pinRegisterSelect,  pinLcdEnable,
        pinLcdData5, pinLcdData6, pinLcdData7, pinLcdData8);
    
   _lcd->begin(16, 2);
   _lcd->clear();
}

void StatusDisplay::_setHelpMessage(const char *message)
{
   _lcd->setCursor(0, 0);
   _lcd->print(_LCD_WIPE_LINE);
   _lcd->setCursor(0, 0);
   _lcd->print(message);
}

void StatusDisplay::setStatusMessage(const char *message)
{
   _lcd->setCursor(0, 1);
   _lcd->print(_LCD_WIPE_LINE);
   _lcd->setCursor(0, 1);
   _lcd->print(message);
   _statusLastSet = millis();
   _statusIsBlank = false;
}

void StatusDisplay::_updateHelpMessage()
{
   unsigned long now = millis();
   if (_lastChangedMillis == 0)
   {
      _setHelpMessage(_helpMessages[_currentHelpIndex]);
      _lastChangedMillis = now;
   }
   else
   {
      if (now - _lastChangedMillis > _helpThresholdMillis)
      {
         _currentHelpIndex++;
         if (_currentHelpIndex == _numHelpMessages)
         {
            _currentHelpIndex = 0;
         }
         _lastChangedMillis = now;
         _setHelpMessage(_helpMessages[_currentHelpIndex]);
      }
   }
}

void StatusDisplay::_updateStatusMessage()
{
   if (!_statusIsBlank)
   {
      if (millis() - _statusLastSet > _statusPersisenceThresholdMillis)
      {
         _lcd->setCursor(0, 1);
         _lcd->print(_LCD_WIPE_LINE);
         _statusIsBlank = true;
      }
   }
}

void StatusDisplay::tick()
{
   _updateHelpMessage();
   _updateStatusMessage();
}