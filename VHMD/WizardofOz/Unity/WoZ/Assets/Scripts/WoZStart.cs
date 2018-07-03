using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;
using System.Timers;
using UnityEngine.UI;

public class WoZStart : MonoBehaviour {

	// Serial - Belt/Headband
	public static string COMPortBelt = "COM3";
	public static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBelt = false; //if port is open or not

	public char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '1', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};

	public float ArrowUpDown;
	public float ArrowRightLeft;
	public float X;
	public float Y;
	public float Xf; // Foveal 
	public float Yf; // Foveal 
	public int iddle = 1;
	public int cont = 0;
	public int clicked = 0;
	public int dirnow = 0;
	public int dirbefore = 0;
	public int fovnow = 0;
	public int fovbefore = 0;

	// Use this for initialization
	void Start () {
		OpenConnectionBelt();
	}
	
	// Update is called once per frame
	void Update () {

		GetInputs ();
		GetDirection ();
		GetFovealTouch ();

		cont++;
		if (cont > 50) {
			cont = 0; clicked = 0;
			if (dirnow == 0 && fovnow == 0 && iddle == 0){ iddle = 1; DisplayDirection(0); }
		}
	}

	void Send(){
		try{
			_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
			print ("<logWOZ> <sys> Submitted to Arduino");
		}catch{
			print ("<logWOZ> <sys> ERROR Could not write in Serial Port");
		}
	}


	void DisplayAlert(){
		SendToArduino[0] = '1'; // ON 		0-1
		SendToArduino[2] = '8'; // MOTOR 	1-8
		SendToArduino[4] = '0'; // FREQ 	0-2
		SendToArduino[6] = '3'; // QtdBURST	1-n
		SendToArduino[8] = '2';	// BURSTDur	0-2
		SendToArduino[10] = '2'; // INTERBURSTDur	0-2

		Send ();
	}

	void DisplayItinerary(int type){

		SendToArduino[0] = '1'; // ON 		0-1
		SendToArduino[4] = '0'; // FREQ 	0-2
		SendToArduino[6] = '2'; // QtdBURST	1-n
		SendToArduino[8] = '2';	// BURSTDur	0-2
		SendToArduino[10] = '2'; // INTERBURSTDur	0-2

		switch(type){
		case 1:
			SendToArduino[2] = '1'; // MOTOR 	1-8
			break;
		case 2:
			SendToArduino[2] = '4'; // MOTOR 	1-8
			break;
		case 3:
			SendToArduino[2] = '7'; // MOTOR 	1-8
			break;
		case 4:
			SendToArduino[2] = '8'; // MOTOR 	1-8
			break;
		}
			
		Send ();
	}

	void DisplayDirection(int type){

		SendToArduino[0] = '2'; // ON 		0-2
		SendToArduino[4] = '0'; // FREQ 	0-2

		switch(type){
		case 0:
			SendToArduino[0] = '0'; // ON 		0-2
			SendToArduino[2] = '0'; // MOTOR 	1-8
			break;
		case 1:
			SendToArduino[2] = '1'; // MOTOR 	1-8
			iddle = 0;
			break;
		case 2:
			SendToArduino[2] = '2'; // MOTOR 	1-8
			iddle = 0;
			break;
		case 3:
			SendToArduino[2] = '4'; // MOTOR 	1-8
			iddle = 0;
			break;
		case 4:
			SendToArduino[2] = '6'; // MOTOR 	1-8
			iddle = 0;
			break;
		case 5:
			SendToArduino[2] = '7'; // MOTOR 	1-8
			iddle = 0;
			break;
		case 6:
			SendToArduino[2] = '8'; // MOTOR 	1-8
			iddle = 0;
			break;
		}

		Send ();
	}

	void DisplayDirectionFOV(int type){
		print ("Displaying");
		SendToArduino[0] = '2'; // ON 		0-2
		SendToArduino[4] = '0'; // FREQ 	0-2

		switch(type){
		case 0:
			SendToArduino[0] = '0'; // ON 		0-2
			SendToArduino[2] = '0'; // MOTOR 	1-8
			break;
		case 1:
			SendToArduino[2] = '2'; // MOTOR 	1-8
			iddle = 0;
			break;
		case 2:
			SendToArduino[2] = '3'; // MOTOR 	1-8
			iddle = 0;
			break;
		case 3:
			SendToArduino[2] = '4'; // MOTOR 	1-8
			iddle = 0;
			break;
		case 4:
			SendToArduino[2] = '5'; // MOTOR 	1-8
			iddle = 0;
			break;
		case 5:
			SendToArduino[2] = '6'; // MOTOR 	1-8
			iddle = 0;
			break;
		}

		Send ();
	}

	void GetDirection(){
		X = Input.GetAxis ("Horizontal");
		Y = Input.GetAxis ("Vertical");

		if (X == -1){
			if(Y == 1){ dirnow = 2; if (dirbefore != dirnow) { DisplayDirection(2); dirbefore = dirnow; iddle = 0; } } // NW
			if (Y < 0.3 && Y >= -1){ dirnow = 1; if (dirbefore != dirnow) { DisplayDirection(1); dirbefore = dirnow; iddle = 0; } } // West
			//if(Y == -1){ SendToArduino[MotorPosition[?]] = '1';} // SW
		} 
		else if (X == 1){
			if(Y == 1){ dirnow = 4; if (dirbefore != dirnow) { DisplayDirection(4); dirbefore = dirnow; iddle = 0; } }	// NE
			if (Y < 0.3 && Y >= -1){ dirnow = 5; if (dirbefore != dirnow) { DisplayDirection(5); dirbefore = dirnow; iddle = 0; } }	// East
			//if(Y == -1){ SendToArduino[MotorPosition[?]] = '1';}	// SE
		}
		else if (X > -0.5 && X < 0.5){
			if(Y == 1){ dirnow = 3; if (dirbefore != dirnow) { DisplayDirection(3); dirbefore = dirnow; iddle = 0; } }	// North
			//if(Y == -1){ dirnow = 6; if (dirbefore != dirnow) { DisplayDirection(6); dirbefore = dirnow; iddle = 0; } } // South
			if (Y > -0.5 && Y < 0.5){ dirnow = 0; if (dirbefore != dirnow) { DisplayDirection(0); dirbefore = dirnow; } }
		}
	}

	// POINTING (Tactile Fovea)
	void GetFovealTouch(){
		Xf = Input.GetAxis ("HorFov");
		Yf = Input.GetAxis ("VerFov");

		if ((Xf >= -1 && Xf < -0.9) && (Yf <= 0.1 && Yf >= -0.9)) {fovnow = 1; if (fovbefore != fovnow) { DisplayDirectionFOV(1); fovbefore = fovnow; iddle = 0; }} 	// West
		else if ((Xf >= -0.9 && Xf < -0.3) && (Yf <= 1 && Yf > 0.3)) { fovnow = 2; if (fovbefore != fovnow) { DisplayDirectionFOV(2); fovbefore = fovnow; iddle = 0; }} // NW
		else if ((Xf >= -0.1 && Xf <= 0.1) && (Yf <= 1 && Yf >= 0.9)) { fovnow = 3; if (fovbefore != fovnow) { DisplayDirectionFOV(3); fovbefore = fovnow; iddle = 0; }} // North
		else if ((Xf > 0.3 && Xf <= 0.9) && (Yf <= 1 && Yf > 0.3)) { fovnow = 4; if (fovbefore != fovnow) { DisplayDirectionFOV(4); fovbefore = fovnow; iddle = 0; }} 	// NE
		else if ((Xf > 0.9 && Xf <= 1) && (Yf <= 0.1 && Yf >= -0.9)) { fovnow = 5; if (fovbefore != fovnow) { DisplayDirectionFOV(5); fovbefore = fovnow; iddle = 0; }} // East
		else if (Xf < 0.3 && Yf < 0.3) { fovnow = 0; if (fovbefore != fovnow) { DisplayDirectionFOV(0); fovbefore = fovnow; }}
	}

	void GetInputs(){

		// Stop
		if (Input.GetButtonDown("JoystickButton0")){
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				DisplayAlert ();
			}
		}
			
		// Clean
		if (Input.GetButtonDown("JoystickButton1")){
			DisplayDirection(0);
		}

		// Keep distance 
		if (Input.GetButtonDown("JoystickButton2")){
			//print ("X");
		}

		// Explore 
		if (Input.GetButtonDown ("JoystickButton3")){
			//print ("Y");
		}


		// Arrows trigger itinerary
		ArrowRightLeft = Input.GetAxis ("Hor");
		ArrowUpDown = Input.GetAxis ("Ver");

		if (ArrowRightLeft == 1){
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				DisplayItinerary (3);
			}
			//Display.text = "TURN TO THE LEFT";
		}
		else if (ArrowRightLeft == -1){
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				DisplayItinerary (1);
			}
			//Display.text = "TURN TO THE RIGHT";
		}
		else if (ArrowUpDown == 1){
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				DisplayItinerary (2);
			}
			//Display.text = "GO FORWARD";
		}
		else if (ArrowUpDown == -1){
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				DisplayItinerary (4);
			}
			//Display.text = "TURN BACK";
		}
	}

	public static void OpenConnectionBelt(){
		if (_SerialPortBelt != null){
			if (_SerialPortBelt.IsOpen){
				_SerialPortBelt.Close();
				print("<logWOZ> <sys> Closing port, because it was already open!");
			} else{
				//very important!, this opens the connection
				_SerialPortBelt.Open();
				//sets the timeout value (how long it takes before timeout error occurs)
				//zet de timeout niet te hoog, dit vertraagd Unity enorm en kan zelfs voor crashes zorgen.
				//_SerialPortBelt.ReadTimeout = 1000;
				//_SerialPortBelt.WriteTimeout = 50;
				portopenBelt = true;
			}
		}
		else{
			if (_SerialPortBelt.IsOpen){ print("<logWOZ> <sys> Port is already open"); }
			else{ print("<logWOZ> <sys> Port == null"); }
		}
	}

	public static void CloseConnection(){
		if (portopenBelt == true) {
			try{
				_SerialPortBelt.Close();
				print("<logWOZ> <sys> Closing port: " + COMPortBelt);
			}catch{
				print("<logWOZ> <sys> ERROR in closing" + COMPortBelt);
			}
		}
	}
		
	void OnApplicationQuit() {
		//clean...
		DisplayDirection(0);
		//and close
		CloseConnection();
		portopenBelt = false;
	}
}
