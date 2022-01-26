// Define to build for Bluetooth else serial over USB
#define _USE_BT_

#include <LiquidCrystal.h>

#ifdef _USE_BT_
#include <SoftwareSerial.h>
#endif

// Pins

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

#ifdef _USE_BT_
const uint8_t PIN_BT_RX = 12;
const uint8_t PIN_BT_TX = 13;
SoftwareSerial blueTooth = SoftwareSerial(PIN_BT_RX, PIN_BT_TX);
#define SERIAL_INF blueTooth
#else
#define SERIAL_INF Serial
#endif

// Misc
const unsigned long DEBOUNCE_DELAY = 100;

// Sound -->

int currentNoteIndex = -1;
int currentNumNotes = 0;
unsigned long currentNoteStartMillis = 0;
unsigned int *activeFrequencies;
unsigned long *activeDurations;

void play(unsigned int frequencies[], unsigned long durations[], size_t numNotes)
{

   currentNoteIndex = 0;
   activeFrequencies = frequencies;
   activeDurations = durations;
   currentNumNotes = numNotes;
   currentNoteStartMillis = millis();

   tone(PIN_TONE, activeFrequencies[0], activeDurations[0]);
}

void playTick()
{
   if (currentNoteIndex != -1)
   {
      unsigned long now = millis();
      if (now >= currentNoteStartMillis + activeDurations[currentNoteIndex])
      {
         currentNoteIndex++;
         if (currentNoteIndex < currentNumNotes)
         {
            currentNoteStartMillis = now;
            tone(PIN_TONE, activeFrequencies[currentNoteIndex], activeDurations[currentNoteIndex]);
         }
         else
         {
            currentNoteIndex = -1;
         }
      }
   }
}

// <-- Sound

// LCD Display -->
LiquidCrystal lcd = LiquidCrystal(PIN_LCD_REGISTER_SELECT, PIN_LCD_ENABLE, 
   PIN_LCD_DATA_4, PIN_LCD_DATA_5, PIN_LCD_DATA_6, PIN_LCD_DATA_7);

void setupLCD()
{
   // Set up LCD display
   lcd.begin(16, 2);
   lcd.clear();
}

const char *LCD_WIPE_LINE = "                ";

void message(const char *message)
{
   lcd.setCursor(0, 0);
   lcd.print(LCD_WIPE_LINE);
   lcd.setCursor(0, 0);
   lcd.print(message);
}

const unsigned long startTimeMillis = millis();

const char *helpMessages[] = {"Teams Sucks", "1 => Cycle Wins", "2 => Toggle Mute", "3 => Toggle Cam"};
const int helpThresholdMillis = 3000;
int currentHelpIndex = 0;
int numHelpMessages = sizeof(helpMessages) / sizeof(char *);
unsigned long lastChangedMillis = 0;

void updateHelp()
{
   unsigned long currentMillis = millis();
   if (lastChangedMillis == 0)
   {
      message(helpMessages[currentHelpIndex]);
      lastChangedMillis = currentMillis;
   }
   else
   {
      if (currentMillis - lastChangedMillis > helpThresholdMillis)
      {
         currentHelpIndex++;
         if (currentHelpIndex == numHelpMessages)
         {
            currentHelpIndex = 0;
         }
         lastChangedMillis = currentMillis;
         message(helpMessages[currentHelpIndex]);
      }
   }
}

unsigned long statusLastSet = 0;
bool statusIsBlank = true;

void statusMessage(const char *message)
{
   lcd.setCursor(0, 1);
   lcd.print(LCD_WIPE_LINE);
   lcd.setCursor(0, 1);
   lcd.print(message);
   statusLastSet = millis();
   statusIsBlank = false;
}

const unsigned long statusPersisenceThresholdMillis = 1000;

void updateStatus()
{
   if (!statusIsBlank)
   {
      if (millis() - statusLastSet > statusPersisenceThresholdMillis)
      {
         lcd.setCursor(0, 1);
         lcd.print(LCD_WIPE_LINE);
         statusIsBlank = true;
      }
   }
}

void lcdTick()
{
   updateHelp();
   updateStatus();
}

// <-- LCD Display

// Protocol -->

// OpCodes
const byte OPCODE_CYCLE_WINDOWS = 0x01;
const byte OPCODE_TOGGLE_MUTE = 0x02;
const byte OPCODE_TOGGLE_CAMERA = 0x03;
const byte OPCODE_ERROR = 0x04;
const byte OPCODE_DEBUG = 0xFF;

void sendDebug(char *message)
{
   SERIAL_INF.write(1 + strlen(message));
   SERIAL_INF.write(OPCODE_DEBUG);
   SERIAL_INF.print(message);
   SERIAL_INF.flush();
}

void sendOpCode(byte opCode)
{
   SERIAL_INF.write(1);
   SERIAL_INF.write(opCode);
   SERIAL_INF.flush();
}

void sendCycleWindows()
{
   sendOpCode(OPCODE_CYCLE_WINDOWS);
}

void sendToggleMute()
{
   sendOpCode(OPCODE_TOGGLE_MUTE);
}

void sendToggleCamera()
{
   sendOpCode(OPCODE_TOGGLE_CAMERA);
}

void handleError()
{
   playError();
}

const int SERIAL_INF_READ_BUFFER_SIZE = 100;
byte serialBuffer[SERIAL_INF_READ_BUFFER_SIZE];
int serialBufferIndex = 0;
bool currentlyReading = false;
byte totalBytesToRead = 0;

void readAndDispatchTick()
{
   byte currentByte;
   if (SERIAL_INF.available())
   {
      currentByte = SERIAL_INF.read();
      if (!currentlyReading)
      {
         serialBufferIndex = 0;
         if (currentByte > 0)
         {
            totalBytesToRead = currentByte;
            currentlyReading = true;
         }
      }
      else
      {
         serialBuffer[serialBufferIndex] = currentByte;
         serialBufferIndex++;
         if (serialBufferIndex == totalBytesToRead)
         {
            currentlyReading = false;
            switch (serialBuffer[0])
            {
            case OPCODE_ERROR:
               handleError();
               break;
            default:
               // ERROR
               break;
            }
         }
      }
   }
}

// <-- Protocol

unsigned int cycleWindowsFrequencies[] = {250, 750, 1250, 750, 250};
unsigned long cycleWindowsDurations[] = {100, 100, 100, 100, 100};
void playCycleWindows()
{
   play(cycleWindowsFrequencies, cycleWindowsDurations, sizeof(cycleWindowsFrequencies) / sizeof(unsigned int));
}

unsigned int toggleMuteFrequencies[] = {250, 750, 1250};
unsigned long toggleMuteDurations[] = {100, 100, 100};
void playButtonToggleMute()
{
   play(toggleMuteFrequencies, toggleMuteDurations, sizeof(toggleMuteFrequencies) / sizeof(unsigned int));
}

unsigned int toggleCameraFrequencies[] = {1250, 750, 250};
unsigned long toggleCameraDurations[] = {100, 100, 100};
void playButtonToggleCamera()
{
   play(toggleCameraFrequencies, toggleCameraDurations, sizeof(toggleCameraFrequencies) / sizeof(unsigned int));
}

unsigned int errorFrequencies[] = {250, 100};
unsigned long errorDurations[] = {500, 500};
void playError()
{
   play(errorFrequencies, errorDurations, sizeof(errorFrequencies) / sizeof(unsigned int));
}

// <-- Sound

// Initialization -->

// Serial port settings
//const long SERIAL_WAIT_DELAY = 1000;
const unsigned long SERIAL_BAUD_RATE = 9600;

unsigned int bootFrequencies[] = {250, 500, 750, 1250, 1500};
unsigned long bootDurations[] = {100, 100, 100, 100, 100};

void setup()
{
   // Initialise the LCD display
   setupLCD();

#ifdef _USE_BT_
   pinMode(PIN_BT_RX, INPUT);
   pinMode(PIN_BT_TX, OUTPUT);
#endif

   SERIAL_INF.begin(SERIAL_BAUD_RATE);

   // Set up pins

   // Bring to front
   pinMode(PIN_CYCLE_WINDOWS, INPUT);
   pinMode(PIN_TOGGLE_MUTE, INPUT);
   pinMode(PIN_TOGGLE_CAMERA, INPUT);

#ifdef _USE_BT_
   statusMessage("Bluetooth comms");
#else
   statusMessage("Serial comms");
#endif

   play(bootFrequencies, bootDurations, sizeof(bootFrequencies) / sizeof(unsigned int));
}

// <-- Initialization

bool cycleWindowsPressed = false;
bool toggleMutePressed = false;
bool toggleMuteCameraPressed = false;

unsigned long lastKeyPressedMillis = 0;

void loop()
{
   lcdTick();
   playTick();

   if (millis() >= lastKeyPressedMillis + DEBOUNCE_DELAY || lastKeyPressedMillis == 0)
   {
      if (digitalRead(PIN_CYCLE_WINDOWS) == HIGH)
      {
         if (!cycleWindowsPressed)
         {
            sendCycleWindows();
            statusMessage("Cycle windows");
            cycleWindowsPressed = true;
            playCycleWindows();
         }
      }
      else
      {
         cycleWindowsPressed = false;
      }

      if (digitalRead(PIN_TOGGLE_MUTE) == HIGH)
      {
         if (!toggleMutePressed)
         {
            sendToggleMute();
            statusMessage("Toggle mute");
            toggleMutePressed = true;
            playButtonToggleMute();
         }
      }
      else
      {
         toggleMutePressed = false;
      }

      if (digitalRead(PIN_TOGGLE_CAMERA) == HIGH)
      {
         if (!toggleMuteCameraPressed)
         {
            sendToggleCamera();
            statusMessage("Toggle camera");
            toggleMuteCameraPressed = true;
            playButtonToggleCamera();
         }
      }
      else
      {
         toggleMuteCameraPressed = false;
      }
      lastKeyPressedMillis = millis();
   }

   readAndDispatchTick();
}
