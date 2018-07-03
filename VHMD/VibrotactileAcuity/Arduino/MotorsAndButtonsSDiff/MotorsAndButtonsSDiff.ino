// variables motors
char serInString[40];  // the received string with output information max 40 characters
int stringLength = 1;  // expected length of string received from Flash
int serInStringLength; // length of string received by Flash.
int digOut = 12;        // Number of digital outputs
int digOutputs[12];    // array that stores which pins are used as digital outputs max 12.
unsigned long changeTime2;
// bursts and interval
unsigned long burst1M;
unsigned long burst2M;
unsigned long intervalM;
int cycle = 5;
int timeToBurst = 50;
int outputOn = 2;
int frequency = 200;
//int amplitude = 127; //50% duty cicle ~220Hz
int amplitude = 77; //30% duty cicle ~150Hz
//int amplitude = 63; //25% duty cicle ~130Hz
//int amplitude = 47; //18.3% duty cicle ~100Hz
int burstOn = 0;
char delaySwitch = '2';
// variables buttons
int button[4];  
int state[4];
unsigned long changeTime;
// reaction time
unsigned long previousStimulus = 0;
unsigned long stimulus = 0;
unsigned long answer = 0;
char lastChar = '0';
int tempMotorOn = 1000;
int motorLigado = 0;

int stmRef = 0;
int stmDiff = 0;
int posDiff = 0;
int stmOn = 0;
int startmillis = 0;

int intervalTest = 2000;

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

void updateArduinoOutputs(){
  int serInStringPosition = 0;
  
  for (int i=0; i < digOut; i++) {
    switch(serInString[serInStringPosition]){
      case '1':
        stmDiff = digOutputs[i];
        posDiff = 0;
        stimulus = 0;
        break;
      case '2':
        stmDiff = digOutputs[i];
        posDiff = 1;
        stimulus = 0;
        break;
      case '5':
        stmRef = digOutputs[i];
        stimulus = 0;
        if(stmDiff == 0) stmDiff = digOutputs[i];
        break;
    }
    serInStringPosition += 2;
  }
  
  delaySwitch = serInString[24];
}

void eraseStrings(){ // erase contents in the serInString array
  for (int i=0; i < 40; i++) {
    serInString[i] = '0';
  }
}

void interval(){
  switch(delaySwitch){
    case '1': 
      intervalTest = 700;
      break;
     case '2': 
      intervalTest = 1000;
      break;
     case '3': 
      intervalTest = 1300;
      break;
  }
}

void readDButtons () {
  for (int i = 0; i < 4; i++) {
    state[i] = digitalRead(button[i]);
  }
  for (int i = 0; i < 4; i++) {
    if(state[i] == HIGH && (millis() - changeTime)>200){
     answer = millis() - stimulus;
     /*if(stimulus > 1){
        stimulus = 1;*/
        if(i == 0) Serial.println("left " + String(answer));
        if(i == 1) Serial.println(String(lastChar) + ":North " + String(answer));
        if(i == 2) Serial.println("right " + String(answer)); 
        if(i == 3) Serial.println(String(lastChar) + ":South " + String(answer));
      //}
      changeTime = millis();
    }
  }
}

void loop () {
  
  if(stmRef != 0){
    if(startmillis == 0){
      
      // start counting
      burst1M = millis();
      burst2M = burst1M;
      intervalM = burst1M;
      interval();
      
      // change state
      startmillis = 1;
    }else if(startmillis == 1){
      
      if((millis() - burst1M) <= 500){
        if(posDiff == 0) stmOn = stmDiff;
        else stmOn = stmRef;
        analogWrite(stmOn, amplitude); // first vibration
        
      }else if((millis() - intervalM) <= (intervalTest + 500)){
        analogWrite(stmOn, 0);

      }else if((millis() - burst2M) <= (intervalTest + 1000)){
        if(posDiff == 1) stmOn = stmDiff;
        else stmOn = stmRef;
        analogWrite(stmOn, amplitude); // second vibration
        if(stimulus == 0){ stimulus = millis(); }
        
      }else{
        analogWrite(stmOn, 0);
        
        stmOn = 0;
        stmDiff = 0;
        stmRef = 0;
        posDiff = 0;
        startmillis = 0;
      }
    }  
  }
  
  readDButtons ();
 
  if((millis() - changeTime2)>1500){
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



