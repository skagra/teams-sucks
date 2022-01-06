#include <Arduino.h>
#line 1 "e:\\Development\\teams-sucks\\arduino\\teams_sucks.ino"
#line 1 "e:\\Development\\teams-sucks\\arduino\\teams_sucks.ino"
void setup();
#line 6 "e:\\Development\\teams-sucks\\arduino\\teams_sucks.ino"
void loop();
#line 1 "e:\\Development\\teams-sucks\\arduino\\teams_sucks.ino"
void setup()
{
	Serial.begin(9600);
}

void loop()
{
	if (Serial.available()) {
		Serial.print((long)millis);
		delay(1000);
	}
}

