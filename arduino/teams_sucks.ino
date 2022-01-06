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
