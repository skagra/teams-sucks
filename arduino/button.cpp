#include "Button.h"

using namespace TeamsSucks;

Button::Button(byte buttonPin, void (*callback)(void*), void *clientData) {
    _buttonPin=buttonPin;
    pinMode(_buttonPin, INPUT);
    _callback=callback;
    _clientData=clientData;
}

void Button::tick() {
    if (digitalRead(_buttonPin) == HIGH)
    {
        unsigned long now = millis();
        if (now >= _lastPressedMillis + _DEBOUNCE_DELAY_MILLIS) {     
            if (!_currentlyPressed)
            {
                _currentlyPressed = true;
                _lastPressedMillis = now;
                _callback(_clientData);
            }
            else
            {
                _currentlyPressed = false;
            }
        }
    }
}