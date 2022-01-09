
#include <LiquidCrystal.h>

// Pins
const uint8_t PIN_BRING_TO_FRONT=8;
const uint8_t PIN_TOGGLE_MUTE=9;
const uint8_t PIN_TOGGLE_CAMERA=10;
// Misc
const unsigned long DEBOUNCE_DELAY=100;

// LCD Display -->

// const uint8_t PIN_LCD_REGISTER_SELECT=8;
// const uint8_t PIN_LCD_ENABLE=12;
// const uint8_t PIN_LCD_DATA_4=5;
// const uint8_t PIN_LCD_DATA_5=4;
// const uint8_t PIN_LCD_DATA_6=3;
// const uint8_t PIN_LCD_DATA_7=2;

const uint8_t PIN_LCD_REGISTER_SELECT=2;
const uint8_t PIN_LCD_ENABLE=3;
const uint8_t PIN_LCD_DATA_4=4;
const uint8_t PIN_LCD_DATA_5=5;
const uint8_t PIN_LCD_DATA_6=6;
const uint8_t PIN_LCD_DATA_7=7;

LiquidCrystal lcd=LiquidCrystal(PIN_LCD_REGISTER_SELECT, PIN_LCD_ENABLE, PIN_LCD_DATA_4, PIN_LCD_DATA_5, PIN_LCD_DATA_6, PIN_LCD_DATA_7);

void setupLCD() {
	// Set up LCD display
	lcd.begin(16, 2);
	lcd.clear();
}

const char* LCD_WIPE_LINE="                ";

void message(const char* message) {
	lcd.setCursor(0, 0);
	lcd.print(LCD_WIPE_LINE);
	lcd.setCursor(0,0);
	lcd.print(message);
}

const unsigned long startTimeMillis=millis();

const char* helpMessages[]= { "Teams Sucks", "1 => Front", "2 => (Un)mute", "3 => Camera" };
const int helpThresholdMillis=3000;
int currentHelpIndex=0;
int numHelpMessages=sizeof(helpMessages)/sizeof(char*);
unsigned long lastChangedMillis=0;

void updateHelp() {
	unsigned long currentMillis=millis();
	if (lastChangedMillis==0) {
		message(helpMessages[currentHelpIndex]);
		lastChangedMillis=currentMillis;
	}
	else
	{
		if ( currentMillis - lastChangedMillis > helpThresholdMillis) {
			currentHelpIndex++;
			if (currentHelpIndex==numHelpMessages) {
				currentHelpIndex=0;
			}
			lastChangedMillis=currentMillis;
			message(helpMessages[currentHelpIndex]);
		}
	}
}

unsigned long statusLastSet=0;
bool statusIsBlank=true;

void statusMessage(const char* message) {
	lcd.setCursor(0, 1);
	lcd.print(LCD_WIPE_LINE);
	lcd.setCursor(0,1);
	lcd.print(message);
	statusLastSet=millis();
	statusIsBlank=false;
}

const unsigned long statusPersisenceThresholdMillis=1000;

void updateStatus() {
	if (!statusIsBlank) {
		if (millis() - statusLastSet > statusPersisenceThresholdMillis) {
			lcd.setCursor(0, 1);
			lcd.print(LCD_WIPE_LINE);
			statusIsBlank=true;
		}
	}
}

void lcdTick() {
	updateHelp();
	updateStatus();
}

// <-- LCD Display

// Protocol -->

// OpCodes
const byte OPCODE_BRING_TO_FRONT=0x01;
const byte OPCODE_TOGGLE_MUTE=0x02;
const byte OPCODE_TOGGLE_CAMERA=0x03;
const byte OPCODE_DEBUG=0xFF;

void sendDebug(char* message) {
	Serial.write(1+strlen(message));
	Serial.write(OPCODE_DEBUG);
	Serial.print(message); 
	Serial.flush();
}

void sendOpCode(byte opCode) {
	Serial.write(1);
	Serial.write(opCode);
	Serial.flush();
}

void sendBringToFront() {
	sendOpCode(OPCODE_BRING_TO_FRONT);
}

void sendToggleMute() {
	sendOpCode(OPCODE_TOGGLE_MUTE);
}

void sendToggleCamera() {
	sendOpCode(OPCODE_TOGGLE_CAMERA);
}

// <-- Protocol

// Initialization -->

// Serial port settings
const long SERIAL_WAIT_DELAY=1000; 
const unsigned long SERIAL_BAUD_RATE=9600;

void setup()
{
	// Initialise the LCD display
	setupLCD();

	// Set up serial over USB
	Serial.begin(SERIAL_BAUD_RATE);
	while (!Serial) {
		delay(SERIAL_WAIT_DELAY);
  	}

	// Set up pins

	// Bring to front
	pinMode(PIN_BRING_TO_FRONT, INPUT);
	pinMode(PIN_TOGGLE_MUTE, INPUT);
	pinMode(PIN_TOGGLE_CAMERA, INPUT);
}

// <-- Initialization

bool bringToFrontPressed=false;
bool toggleMutePressed=false;
bool toggleMuteCameraPressed=false;

void loop()
{	
	lcdTick();

	if (digitalRead(PIN_BRING_TO_FRONT)==HIGH) {
		if (!bringToFrontPressed) {
			sendBringToFront(); 
			statusMessage("Bring to front");
			bringToFrontPressed=true;
		}
	}
	else
	{
		bringToFrontPressed=false;
	} 

	if (digitalRead(PIN_TOGGLE_MUTE)==HIGH) {
		if (!toggleMutePressed) {
			sendToggleMute(); 
			statusMessage("Toggle mute");
			toggleMutePressed=true;
		}
	}
	else
	{
		toggleMutePressed=false;
	}

	if (digitalRead(PIN_TOGGLE_CAMERA)==HIGH) {
		if (!toggleMuteCameraPressed) {
			sendToggleCamera(); 
			statusMessage("Toggle camera");
			toggleMuteCameraPressed=true;
		}
	}
	else
	{
		toggleMuteCameraPressed=false;
	}

	delay(DEBOUNCE_DELAY);
}
