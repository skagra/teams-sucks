
#include "pinout.h"
#include <LiquidCrystal.h>


// Serial port
const long SERIAL_WAIT_DELAY=1000; 
const unsigned long SERIAL_BAUD_RATE=9600;

// Misc
const unsigned long DEBOUNCE_DELAY=100;

// OpCodes
const byte OPCODE_DEBUG=0xFF;

LiquidCrystal lcd=LiquidCrystal(PIN_LCD_REGISTER_SELECT, PIN_LCD_ENABLE, PIN_LCD_DATA_4,  PIN_LCD_DATA_5,  PIN_LCD_DATA_6,  PIN_LCD_DATA_7);

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

const char* helpMessages[]= { "1 => Front", "2 => (Un)mute", "3 => Camera"};
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


void setup()
{
	// Initialise the LCD display
	setupLCD();

	// Welcome message
	message("Teams Sucks");

	// Set up serial over USB
	Serial.begin(SERIAL_BAUD_RATE);
	while (!Serial) {
		delay(SERIAL_WAIT_DELAY);
  	}

	// Set up pins

	// Bring to front
	pinMode(PIN_BRING_TO_FRONT, INPUT);

	delay(1000);

	statusMessage("Hello there!");
}

bool broughtToTop=false;

void loop()
{	
	lcdTick();

	if (digitalRead(PIN_BRING_TO_FRONT)==HIGH) {
		if (!broughtToTop) {
			Serial.write(2);
			Serial.write(OPCODE_DEBUG);
			Serial.print("Q"); 
			broughtToTop=true;
			Serial.flush();
		}
	}
	else
	{
		broughtToTop=false;
	}

	delay(DEBOUNCE_DELAY);
}

void serialEvent() {
	int charRead=Serial.read();
	lcd.print("H");
}