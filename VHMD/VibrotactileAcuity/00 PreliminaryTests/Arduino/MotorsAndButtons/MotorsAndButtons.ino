// variables motors
char serInString[40];  // the received string with output information max 40 characters
int stringLength = 1;  // expected length of string received from Flash
int serInStringLength; // length of string received by Flash.
int digOut = 12;        // Number of digital outputs
int digOutputs[12];    // array that stores which pins are used as digital outputs max 12.
unsigned long changeTime2;
// frquency
unsigned long dTime;
unsigned long dTime2;
int cycle = 5;
int timeToBurst = 50;
int outputOn = 0;
int frequency = 200;
int amplitude = 255;
int burstOn = 0;
// variables buttons
int button[4];  
int state[4];
unsigned long changeTime;
// reaction time
unsigned long previousStimulus = 0;
unsigned long stimulus;
unsigned long answer;
char lastChar = '0';

int motorLigado = 0;

void setup() {
  Serial.begin(57600);    // start serial port
  
  // OUTPUT: Motors
  for (int i =0; i < digOut; i++) {
   pinMode(i+2, OUTPUT);
   digOutputs[i] = i+2;
   stringLength +=2; // output string part will have 2 characters
  }
  
  // INPUT: Buttons
  button[0] = A0;
  button[1] = A1;
  button[2] = A2;
  button[3] = A3;
  for (int i = 0; i < 4; i++) {
   pinMode(button[i], INPUT);
  }
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

int delta(int frequency){
  int delt = timeToBurst/(frequency/20) - cycle;
  return delt;
}

void burst(){
  if((millis() - dTime) > delta(frequency)){
    analogWrite(outputOn, amplitude);
    delay(cycle);
    analogWrite(outputOn, 0);
    dTime = millis();
  }
}

void triggerStimulus(int out, int freq, int ampl){
  outputOn = out;
  frequency = freq;
  amplitude = ampl;
}

void updateArduinoOutputs(){
  int serInStringPosition = 0;
  if(motorLigado == 0){
    for (int i=0; i < digOut; i++) {
      switch(serInString[serInStringPosition]){
        case '1':
          triggerStimulus(digOutputs[i], 40, 255);
          stimulus = millis();
          motorLigado = 1;
          break;
        case '2':
          triggerStimulus(digOutputs[i], 80, 255);
          stimulus = millis();
          motorLigado = 1;
          break;
        case '3':
          triggerStimulus(digOutputs[i], 160, 255);
          stimulus = millis();
          motorLigado = 1;
          break;
        case '4':
          triggerStimulus(digOutputs[i], 40, 155);
          stimulus = millis();
          motorLigado = 1;
          break;
        case '5':
          triggerStimulus(digOutputs[i], 80, 155);
          stimulus = millis();
          motorLigado = 1;
          break;
        case '6':
          triggerStimulus(digOutputs[i], 160, 155);
          stimulus = millis();
          motorLigado = 1;
          break;
        case '7':
          Serial.println("lighton");
          stimulus = millis();
          motorLigado = 2;
          break;
      }
      serInStringPosition += 2;
    }
    if(motorLigado == 1 || motorLigado == 2){
      lastChar = serInString[24];
    }
  }
}

void eraseStrings(){ // erase contents in the serInString array
  for (int i=0; i < 40; i++) {
    serInString[i]=0;
  }
}

void readDButtons () {
  for (int i = 0; i < 4; i++) {
    state[i] = digitalRead(button[i]);
  }
  for (int i = 0; i < 4; i++) {
    if(state[i] == HIGH && (millis() - changeTime)>200){
     answer = millis() - stimulus;
     if(stimulus != previousStimulus ){//&& answer < 1000
       previousStimulus = stimulus;
        if(i == 0) Serial.println(String(lastChar) + ":West " + String(answer));
        if(i == 1) Serial.println(String(lastChar) + ":North " + String(answer));
        if(i == 2) Serial.println(String(lastChar) + ":East " + String(answer)); 
        if(i == 3) Serial.println(String(lastChar) + ":South " + String(answer));
      }
      changeTime = millis();
    }
  }
}

void loop () {
  if(motorLigado == 1){
    if(burstOn == 0){
      dTime2 = millis();
      burstOn = 1;
    }
    if((millis() - dTime2) < 1000) burst();
    else {
      motorLigado = 0;
      burstOn = 0;
    }
  }
  
  if(motorLigado == 2){
    if(burstOn == 0){
      dTime2 = millis();
      burstOn = 1;
    }
    if((millis() - dTime2) > 1000){
      motorLigado = 0;
      burstOn = 0;
    }
  }
 
  readDButtons ();
  if((millis() - changeTime2)>500){
  // read the serial port and create a string out of what you read
  readSerialString(serInString);

  // check if there data is received if isStringEmpty is true do nothing.
  if(isStringEmpty(serInString) == false) {
    updateArduinoOutputs();
    eraseStrings();
  }
  changeTime2 = millis();
  // short delay before making sure serial.read is not called to soon again
  //delay(20);
  }
}



