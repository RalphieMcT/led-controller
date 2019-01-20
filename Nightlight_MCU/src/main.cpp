#include <Arduino.h>

void setup() {
    pinMode(PC13, OUTPUT); //onboard LED
	pinMode(PA8, OUTPUT); //Channel1_MOSFET
	pinMode(PA11, OUTPUT); //Channel2_MOSFET
	pinMode(PB8, OUTPUT); //Channel1
	pinMode(PB9, OUTPUT); //Channel2
	Serial.begin(115200);
    analogWrite(PB8, 255);
}

void loop() {
    int receivedChars[4] = {0, 0, 0, 0}; //PA8 (MOSFET), PA11 (MOSFET), PB8, PB9
    bool newData = false;

    if(Serial.available() > 0){
        newData = true;
        int rcIndex = 0;
        char delimiter = ',';
        char endChar = '\r';
        while(1){
            int readChar = Serial.read();
            if(readChar == endChar) break;
            if(readChar == -1) continue;
            if(readChar != delimiter){
                receivedChars[rcIndex] *= 10; //move decimal left
                receivedChars[rcIndex] += readChar -'0'; //convert to int
            }
            else{
                rcIndex++;
            }
        }
        if(newData){/*
            Serial.println("VVVVVVVVVVVVVVVVVVVV");
            Serial.print("arg1:\t");
            Serial.println(receivedChars[0]);
            Serial.print("arg2:\t");
            Serial.println(receivedChars[1]);
            Serial.print("arg3:\t");
            Serial.println(receivedChars[2]);
            Serial.print("arg4:\t");
            Serial.println(receivedChars[3]);
            Serial.println("^^^^^^^^^^^^^^^^^^^^");*/
            analogWrite(PA8, receivedChars[0]);
            analogWrite(PA11, receivedChars[1]);
            analogWrite(PB8, receivedChars[2]);
            analogWrite(PB9, receivedChars[3]);
        }
    }
}
