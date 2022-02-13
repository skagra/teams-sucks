#ifndef _TUNE_DOT_H_
#define _TUNE_DOT_H_

#include <Arduino.h>

namespace TeamsSucks {
    class Player {
        private:
            uint8_t _tonePin;
            int _noteIndex = -1;
            int _numNotes = 0;
            unsigned long _noteStartMillis = 0;
            unsigned int *_activeFrequencies = (unsigned int*)0;
            unsigned long *_activeDurations = (unsigned long*)0;

        public:
            Player(uint8_t tonePin);
            void play(unsigned int frequencies[], unsigned long durations[], size_t numNotes);
            void tick();
    };
}

#endif

