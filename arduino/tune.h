#ifndef _TUNE_DOT_H
#define _TUNE_DOT_H

#include <Arduino.h>

class Tune {
    private:
        byte _tonePin;
        int _noteIndex = -1;
        int _numNotes = 0;
        unsigned long _noteStartMillis = 0;
        unsigned int *_activeFrequencies = (unsigned int*)0;
        unsigned long *_activeDurations = (unsigned long*)0;
    public:
        Tune(byte tonePin);
        void play(unsigned int frequencies[], unsigned long durations[], size_t numNotes);
        void tick();
};

#endif

