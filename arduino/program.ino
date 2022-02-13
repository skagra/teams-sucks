#include "Player.h"
#include "Button.h"
#include "StatusDisplay.h"
#include "Protocol.h"

using namespace TeamsSucks;

// Pins -->

// Switches
const uint8_t PIN_CYCLE_WINDOWS = 8;
const uint8_t PIN_TOGGLE_MUTE = 9;
const uint8_t PIN_TOGGLE_CAMERA = 10;

// Display
const uint8_t PIN_LCD_REGISTER_SELECT = 2;
const uint8_t PIN_LCD_ENABLE = 3;
const uint8_t PIN_LCD_DATA_4 = 4;
const uint8_t PIN_LCD_DATA_5 = 5;
const uint8_t PIN_LCD_DATA_6 = 6;
const uint8_t PIN_LCD_DATA_7 = 7;

// Tone
const uint8_t PIN_TONE = 11;

// <-- Pins

// Sounds -->

unsigned int cycleWindowsFrequencies[] = {250, 750, 1250, 750, 250};
unsigned long cycleWindowsDurations[] = {100, 100, 100, 100, 100};

unsigned int toggleMuteFrequencies[] = {250, 750, 1250};
unsigned long toggleMuteDurations[] = {100, 100, 100};

unsigned int toggleCameraFrequencies[] = {1250, 750, 250};
unsigned long toggleCameraDurations[] = {100, 100, 100};

unsigned int errorFrequencies[] = {250, 100};
unsigned long errorDurations[] = {500, 500};

unsigned int bootFrequencies[] = {250, 500, 750, 1250, 1500};
unsigned long bootDurations[] = {100, 100, 100, 100, 100};

// <-- Sounds

// Help messages
const char* helpMessages[] = {"Teams Sucks", "1 => Cycle Wins", "2 => Toggle Mute", "3 => Toggle Cam"};

// Tune
Player *player;

// Status display
StatusDisplay *statusDisplay;

// Protocol
Protocol *protocol;

// Buttons
Button *cycleWindowsButton;
Button *toggleMuteButton;
Button *toggleCameraButton;

// Button press callbacks
void CycleWindowsCallback(void *clientData) {
   protocol->sendCycleWindows();
   statusDisplay->setStatusMessage("Cycle windows");
   player->play(cycleWindowsFrequencies, cycleWindowsDurations, sizeof(cycleWindowsFrequencies) / sizeof(unsigned int));
}

void toggleMuteCallback(void *clientData) {
   protocol->sendCycleWindows();
   statusDisplay->setStatusMessage("Cycle windows");
   player->play(toggleCameraFrequencies, toggleCameraDurations, sizeof(toggleCameraFrequencies) / sizeof(unsigned int));
}

void toggleCameraCallback(void *clientData) {
   protocol->sendToggleCamera();
   statusDisplay->setStatusMessage("Toggle camera");
   player->play(toggleMuteFrequencies, toggleMuteDurations, sizeof(toggleMuteFrequencies) / sizeof(unsigned int));
}

void setup()
{
   player=new Player(PIN_TONE);

   statusDisplay=new StatusDisplay(helpMessages, sizeof(helpMessages) / sizeof(char*),
      PIN_LCD_REGISTER_SELECT, PIN_LCD_ENABLE,
      PIN_LCD_DATA_4, PIN_LCD_DATA_5, PIN_LCD_DATA_6, PIN_LCD_DATA_7);

   protocol=new Protocol();

   cycleWindowsButton=new Button(PIN_CYCLE_WINDOWS, CycleWindowsCallback, (void*)0);
   toggleMuteButton=new Button(PIN_TOGGLE_MUTE, toggleMuteCallback, (void*)0);
   toggleCameraButton=new Button(PIN_TOGGLE_CAMERA, toggleCameraCallback,  (void*)0);

   player->play(bootFrequencies, bootDurations, sizeof(bootFrequencies) / sizeof(unsigned int));
}

void loop()
{
   statusDisplay->tick();
   player->tick();

   cycleWindowsButton->tick();
   toggleMuteButton->tick();
   toggleCameraButton->tick();

   protocol->tick();
}
