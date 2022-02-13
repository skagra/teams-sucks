#include "button.h"

Button::Button(byte buttonPin, void (*callback)(void*), void *clientData) {
    _buttonPin=buttonPin;
    pinMode(_buttonPin, INPUT);
    _callback=callback;
    _clientData=clientData;
}

void Button::tick() {
    if (digitalRead(_buttonPin) == HIGH)
    {
        if (millis() >= _lastPressedMillis + _DEBOUNCE_DELAY_MILLIS) {     
            if (!_currentlyPressed)
            {
                _currentlyPressed = true;
                _lastPressedMillis = millis();
                _callback(_clientData);
            }
            else
            {
                _currentlyPressed = false;
            }
        }
    }
}