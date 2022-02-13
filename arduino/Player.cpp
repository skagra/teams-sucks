#include "Player.h"

using namespace TeamsSucks;

Player::Player(uint8_t tonePin) {
    _tonePin=tonePin;
     pinMode(_tonePin, OUTPUT);
};

void Player::play(unsigned int frequencies[], unsigned long durations[], size_t numNotes)
{
   _noteIndex = 0;
   _activeFrequencies = frequencies;
   _activeDurations = durations;
   _numNotes = numNotes;
   _noteStartMillis = millis();

   tone(_tonePin, _activeFrequencies[0], _activeDurations[0]);
}

void Player::tick()
{
   if (_noteIndex != -1)
   {
      unsigned long now = millis();
      if (now >= _noteStartMillis + _activeDurations[_noteIndex])
      {
         _noteIndex++;
         if (_noteIndex < _numNotes)
         {
            _noteStartMillis = now;
            tone(_tonePin, _activeFrequencies[_noteIndex], _activeDurations[_noteIndex]);
         }
         else
         {
            _noteIndex = -1;
         }
      }
   }
}