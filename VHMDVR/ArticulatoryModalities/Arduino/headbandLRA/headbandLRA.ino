#include <Wire.h>
#include "Adafruit_DRV2605.h"

Adafruit_DRV2605 drv;
// variables motors
char serInString[40];  // the received string with output information max 40 characters
int stringLength = 13;  // expected length of string received from Flash
int serInStringLength; // length of string received by Flash.
int digOut = 8;        // Number of digital outputs
int digOutputs[8];    // array that stores which pins are used as digital outputs max 12.
unsigned long changeTime2;
int erasef = 1;
int frequency[10];
int burstDur[24];
int interburstDur[3];

void setup() {
  Serial.begin(57600);
  //Serial.println("DRV test");

  // DRIVER
  drv.begin();
  drv.useLRA();
  drv.setMode(DRV2605_MODE_PWMANALOG);

  drv.selectLibrary(0x06);
  drv.writeRegister8(0x20, 0x00);
  drv.writeRegister8(0x1D, 0x00);
  
  // The user can enter the PWM mode by setting the MODE[2:0] bit to 3 in register 0x01
  // and bye setting the N_PWM_ANALOG bit to 0 in register 0x1D.
  // To configure the DRV2605L device in LRA open-loop operation, the LRA must be selected by writing the N_ERM_LRA bit to 1 in register 0x1A, 
  // and the LRA_OPEN_LOOP bit to 1 in register 0x1D.
  
   
  // closed-loop
  //drv.writeRegister8(0x1D, 0x00); 
  // open-loop
  //drv.writeRegister8(0x1A, 0x01); 
  //drv.writeRegister8(0x1D, 0x01);

  // OUTPUT
  for (int i =0; i < digOut; i++) {
   pinMode(i+2, OUTPUT);
   analogWrite(i+2, 0);
  }

  digOutputs[0] = 2;
  digOutputs[1] = 7;
  digOutputs[2] = 8;
  digOutputs[3] = 5;
  digOutputs[4] = 3;
  digOutputs[5] = 6;
  digOutputs[6] = 9;
  digOutputs[7] = 4;

  // PARAMETERS
  frequency[0] = 0;
  frequency[1] = 28;
  frequency[2] = 56;
  frequency[3] = 85;
  frequency[4] = 113;
  frequency[5] = 141;
  frequency[6] = 170;
  frequency[7] = 198;
  frequency[8] = 226;
  frequency[9] = 255;
  
  /*burstDur[0] = 200;
  burstDur[1] = 300;
  burstDur[2] = 500;*/
  burstDur[0] = 50;
  burstDur[1] = 100;
  burstDur[2] = 150;
  burstDur[3] = 200;
  burstDur[4] = 250;
  burstDur[5] = 300;
  burstDur[6] = 350;
  burstDur[7] = 400;
  burstDur[8] = 450;
  burstDur[9] = 500;
  burstDur[10] = 550;
  burstDur[11] = 600;
  burstDur[12] = 650;
  burstDur[13] = 700;
  burstDur[14] = 750;
  burstDur[15] = 800;
  burstDur[16] = 850;
  burstDur[17] = 900;
  burstDur[18] = 950;
  burstDur[19] = 1000;
  burstDur[20] = 1500;
  burstDur[21] = 2000;
  burstDur[22] = 2500;
  burstDur[23] = 3000;
  
  interburstDur[0] = 0;
  interburstDur[1] = 200;
  interburstDur[2] = 500;
}

// Function to read a string from the serialport and store it in an array
void readSerialString (char *strArray){
 int i = 0;
 if(Serial.available()){
  while (Serial.available()) {
    strArray[i] = Serial.read();
    i++;
  }
 }
 serInStringLength=i;
}

// Function to find out wether the array is empty or not
boolean isStringEmpty(char *strArray){
  if (strArray[0] == 0)   return true;
  else                    return false;
}

void eraseStrings(){ // erase contents in the serInString array
  for (int i=0; i < 40; i++) {
    serInString[i]=0;
  }
}

void updateArduinoOutputs(){ 

  if(serInString[0] == '0'){
    erasef = 1;
    for (int i =0; i < digOut; i++) {
      analogWrite(i+2, 0);
    }
  }
  
  if(serInString[0] == '1'){
    erasef = 1;  
    for (int i =0; i < int(serInString[6])-48 ; i++) {
      analogWrite(digOutputs[int(serInString[2])-49], frequency[int(serInString[4])-48]);
      delay(burstDur[int(serInString[8])-65]);
      analogWrite(digOutputs[int(serInString[2])-49], 0);
      delay(interburstDur[int(serInString[10])-48]);
    }
  }

  if(serInString[0] == '2'){
    erasef = 0;
    for (int i =0; i < digOut; i++) {
      analogWrite(i+2, 0);
    }
    analogWrite(digOutputs[int(serInString[2])-49], frequency[int(serInString[4])-48]);
  }

  if(serInString[0] == '3'){
    erasef = 1;  
    for (int i = int(serInString[2])-49; i <= int(serInString[12])-49 ; i++) {
      if(i != 2 && i != 4){
        analogWrite(digOutputs[i], frequency[int(serInString[4])-48]);
        delay(burstDur[int(serInString[8])-65]);
        analogWrite(digOutputs[i], 0);
        delay(interburstDur[int(serInString[10])-48]);
      } 
    }
  }

  if(serInString[0] == '4'){
    erasef = 1;  
    for (int i = int(serInString[2])-49; i >= int(serInString[12])-49 ; i--) {
      if(i != 2 && i != 4){
        analogWrite(digOutputs[i], frequency[int(serInString[4])-48]);
        delay(burstDur[int(serInString[8])-65]);
        analogWrite(digOutputs[i], 0);
        delay(interburstDur[int(serInString[10])-48]);
      }
    }
  }
  
}

void loop() {
    
  if((millis() - changeTime2)> 50){
  // read the serial port and create a string out of what you read
  readSerialString(serInString);

  // check if there data is received if isStringEmpty is true do nothing.
  if(isStringEmpty(serInString) == false) {
    updateArduinoOutputs();
    if(erasef == 1) eraseStrings();
  }
  changeTime2 = millis();
  // short delay before making sure serial.read is not called to soon again
  //delay(20);
  }

}
