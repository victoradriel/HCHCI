using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;
using System.Timers;
using UnityEngine.UI;

public class WoZ : MonoBehaviour {

	// Serial - Belt
	public static string COMPortBelt = "COM3";
	public static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBelt = false; //if port is open or not

	static string[] SpMotors = new string[] {"Motor1","Motor2","Motor3","Motor4","Motor5","Motor6","Motor7","Motor8"};
	static string[] UpViewMtrs = new string[] {"UMotor1","UMotor2","UMotor3","UMotor4","UMotor5","UMotor6","UMotor7","UMotor8"};
	public static int[] MotorPosition = new int[] {0,2,4,6,8,10,12,14};
	public static int[] MotorPosition2 = new int[] {0,2,4,6,8,10,12,14};
	public char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
	public char[] SendToArdPrevious = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
	public Matrix4x4 originalProjection;
	Camera camera;
	public float ArrowUpDown;
	public float ArrowRightLeft;
	public float X;
	public float Y;
	public float Xf; // Foveal 
	public float Yf; // Foveal 
	Text Display;
	public int cont = 0;
	public int clicked = 0;

	// Timer - Print tactons;
	public int enableDraw = 1;
	public System.Timers.Timer timerRhythm = new System.Timers.Timer(200);
	public int contCicle = 0;
	int seqOn = 0;
	int itineraryOn = 0;
	int itinIndex = 0;
	int alertOn = 0;
	int cClean = 0; // To clean display text
	int change = 0;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
		Display = GameObject.Find("Text").GetComponent<Text>();

		timerRhythm.Elapsed += new ElapsedEventHandler (EnableDraw);
		timerRhythm.Start ();

		OpenConnectionBelt();
	}

	// Update is called once per frame
	void Update () {

		cont++;
		if (cont > 50) {
			cont = 0;
			clicked = 0;
		}

		if (Input.GetKey (KeyCode.Alpha1)) {
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				print ("numero 1");
				SendToArduino[0] = '1';
				SendToArduino[2] = '1';
				SendToArduino[4] = '0';
				SendToArduino[6] = '1';
				SendToArduino[8] = '0';
				SendToArduino[10] = '0';
				try{
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					print ("arduino");
				}catch{
					print ("nope");
				}
			}
		}

		if (Input.GetKey (KeyCode.Alpha2)) {
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				print ("numero 2");
				SendToArduino[0] = '1';
				SendToArduino[2] = '2';
				SendToArduino[4] = '0';
				SendToArduino[6] = '1';
				SendToArduino[8] = '0';
				SendToArduino[10] = '0';
				try{
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					print ("arduino");
				}catch{
					print ("nope");
				}
			}
		}

		if (Input.GetKey (KeyCode.Alpha3)) {
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				print ("numero 3");
				SendToArduino[0] = '1';
				SendToArduino[2] = '3';
				SendToArduino[4] = '0';
				SendToArduino[6] = '1';
				SendToArduino[8] = '0';
				SendToArduino[10] = '0';
				try{
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					print ("arduino");
				}catch{
					print ("nope");
				}
			}
		}

		if (Input.GetKey (KeyCode.Alpha4)) {
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				print ("numero 3");
				SendToArduino[0] = '1';
				SendToArduino[2] = '4';
				SendToArduino[4] = '0';
				SendToArduino[6] = '1';
				SendToArduino[8] = '0';
				SendToArduino[10] = '0';
				try{
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					print ("arduino");
				}catch{
					print ("nope");
				}
			}
		}

		if (Input.GetKey (KeyCode.Alpha5)) {
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				print ("numero 3");
				SendToArduino[0] = '1';
				SendToArduino[2] = '5';
				SendToArduino[4] = '0';
				SendToArduino[6] = '1';
				SendToArduino[8] = '0';
				SendToArduino[10] = '0';
				try{
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					print ("arduino");
				}catch{
					print ("nope");
				}
			}
		}

		if (Input.GetKey (KeyCode.Alpha6)) {
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				print ("numero 3");
				SendToArduino[0] = '1';
				SendToArduino[2] = '6';
				SendToArduino[4] = '0';
				SendToArduino[6] = '1';
				SendToArduino[8] = '0';
				SendToArduino[10] = '0';
				try{
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					print ("arduino");
				}catch{
					print ("nope");
				}
			}
		}

		if (Input.GetKey (KeyCode.Alpha7)) {
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				print ("numero 3");
				SendToArduino[0] = '1';
				SendToArduino[2] = '7';
				SendToArduino[4] = '0';
				SendToArduino[6] = '1';
				SendToArduino[8] = '0';
				SendToArduino[10] = '0';
				try{
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					print ("arduino");
				}catch{
					print ("nope");
				}
			}
		}

		if (Input.GetKey (KeyCode.Alpha8)) {
			if (clicked == 0) {
				clicked = 1;
				cont = 0;
				print ("numero 3");
				SendToArduino[0] = '1';
				SendToArduino[2] = '8';
				SendToArduino[4] = '0';
				SendToArduino[6] = '1';
				SendToArduino[8] = '0';
				SendToArduino[10] = '0';
				try{
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					print ("arduino");
				}catch{
					print ("nope");
				}
			}
		}

		if(seqOn == 0){
			zeraString ();
			// update strings
			GetDirection ();	// Ver se eles competem 
			GetFovealTouch ();	// por recurso

			// see if strings were changed
			change = DidChange();

			// update array
			if(change == 1){
				ActivateMotor(0);
			}

			GetInputs ();
		}
		else{
			DisplaySequence();
		}

		cleanDsiplay();
	}

	int DidChange(){
		int chng = 0;
		for(int i = 0; i < 8; i++){
			if(SendToArduino[MotorPosition[i]] != SendToArdPrevious[MotorPosition[i]]){
				chng = 1;
				SendToArdPrevious[MotorPosition[i]] = SendToArduino[MotorPosition[i]];
			}
		}
		
		return chng;
	}


	void cleanDsiplay(){
		
		cClean++;
		if(cClean > 50 && seqOn == 0){
			cClean = 0;
			Display.text = " ";
		}
	}

	void DisplaySequence(){

		if(itineraryOn == 1)
			DisplayItinerary();

		if(alertOn == 1)
			DisplayAlert();
	}

	void DisplayItinerary(){

		if (SendToArduino[MotorPosition[itinIndex]] == '2') {

			if (enableDraw == 1) {			
				enableDraw = 0;
				contCicle++;

				switch (contCicle) {
					case 1:	
					case 3:	
						ActivateMotor(1);
						//_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
						break;
					case 2:	
					case 4:	
						zeraString();
						ActivateMotor(1);
						SendToArduino[MotorPosition[itinIndex]] = '2';
						//_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);	
						break;
					case 5:
						contCicle = 0;
						itineraryOn = 0;
						seqOn = 0;
						zeraString();
						Display.text = " ";
						break;				
				}
			
				timerRhythm.Elapsed += new ElapsedEventHandler (EnableDraw);
				timerRhythm.Start ();
			}
		}
	}

	void DisplayAlert(){

		if (SendToArduino[MotorPosition[7]] == '3') {
			
			if (enableDraw == 1) {			
				enableDraw = 0;
				contCicle++;
				
				switch (contCicle) {
				case 1:	
				case 3:	
				case 5:	
					ActivateMotor(1);
					//_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					break;
				case 2:	
				case 4:	
				case 6:	
					zeraString();
					ActivateMotor(1);
					SendToArduino[MotorPosition[7]] = '3';
					//_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);	
					break;
				case 7:
					contCicle = 0;
					alertOn = 0;
					seqOn = 0;
					zeraString();
					Display.text = " ";
					break;				
				}
				
				timerRhythm.Elapsed += new ElapsedEventHandler (EnableDraw);
				timerRhythm.Start ();
			}
		}
	}

	void EnableDraw(object source, ElapsedEventArgs e){
		// Código a ser executado quando o intervalo for atingido
		enableDraw = 1;
	}
	
	void ActivateMotor(int mode){
		GameObject motor;
		GameObject motor2;
		print ("testtt");

		try{
		_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
			print ("arduino");
		}catch{
			print ("nope");
		}

		for(int i = 0; i < 8; i++){
			if(SendToArduino[MotorPosition[i]] == '1' || SendToArduino[MotorPosition[i]] == '3'){
				motor = GameObject.Find(SpMotors[i]); 
				motor.GetComponent<Renderer>().material.color = Color.red;
				motor2 = GameObject.Find(UpViewMtrs[i]); 
				motor2.GetComponent<Renderer>().material.color = Color.red;
			}
			else if(SendToArduino[MotorPosition[i]] == '2'){
				motor = GameObject.Find(SpMotors[i]); 
				motor.GetComponent<Renderer>().material.color = Color.magenta;
				motor2 = GameObject.Find(UpViewMtrs[i]); 
				motor2.GetComponent<Renderer>().material.color = Color.magenta;
			}
			else{
				motor = GameObject.Find(SpMotors[i]); 
				motor.GetComponent<Renderer>().material.color = Color.white;
				motor2 = GameObject.Find(UpViewMtrs[i]); 
				motor2.GetComponent<Renderer>().material.color = Color.white;
			}
		}

		if (mode == 0) {
			print ("zera");
			zeraString ();
		}
	}

	void zeraString() {
		int i = 0;
		for(i = 0; i < 8; i++){
			SendToArduino[MotorPosition[i]] = '0';
		}
	}

	void GetInputs(){
		
		// Stop
		if (Input.GetButtonDown("JoystickButton0")){
			SendToArduino[MotorPosition[7]] = '3';
			seqOn = 1;
			alertOn = 1;
			Display.text = "STOP";
		}

		/*
		// Change function
		if (Input.GetButtonDown("JoystickButton1")){
			//Nothing by now
		}

		// Keep distance 
		if (Input.GetButtonDown("JoystickButton2")){
			SendToArduino [MotorPosition [7]] = '5';
			alertOn = 1;
		}

		// Explore 
		if (Input.GetButtonDown ("JoystickButton3")){
			SendToArduino [MotorPosition [7]] = '6';
			alertOn = 1;
		}
		*/
		
		// Arrows trigger itinerary
		ArrowRightLeft = Input.GetAxis ("Hor");
		ArrowUpDown = Input.GetAxis ("Ver");

		if (ArrowRightLeft == 1){
			SendToArduino[MotorPosition [6]] = '2';
			seqOn = 1;
			itineraryOn = 1;
			itinIndex = 6;
			Display.text = "TURN TO THE LEFT";
		}
		else if (ArrowRightLeft == -1){
			SendToArduino[MotorPosition [0]] = '2';
			seqOn = 1;
			itineraryOn = 1;
			itinIndex = 0;
			Display.text = "TURN TO THE RIGHT";
		}
		else if (ArrowUpDown == 1){
			SendToArduino[MotorPosition [3]] = '2';
			seqOn = 1;
			itineraryOn = 1;
			itinIndex = 3;
			Display.text = "GO FORWARD";
		}
		else if (ArrowUpDown == -1){
			SendToArduino[MotorPosition [7]] = '2';
			seqOn = 1;
			itineraryOn = 1;
			itinIndex = 7;
			Display.text = "TURN BACK";
		}
	}

	void GetDirection(){
		X = Input.GetAxis ("Horizontal");
		Y = Input.GetAxis ("Vertical");

		if (X == -1){
			if(Y == 1){ SendToArduino[MotorPosition[1]] = '1'; Display.text = "POINTING NW"; cClean = 0; } // NW
			if (Y < 0.3 && Y >= -1){ SendToArduino[MotorPosition[0]] = '1'; Display.text = "POINTING WEST"; } // West
			//if(Y == -1){ SendToArduino[MotorPosition[?]] = '1';} // SW
		} 
		else if (X == 1){
			if(Y == 1){ SendToArduino[MotorPosition[5]] = '1'; Display.text = "POINTING NE"; cClean = 0; }	// NE
			if (Y < 0.3 && Y >= -1){ SendToArduino[MotorPosition[6]] = '1'; Display.text = "POINTING EAST"; cClean = 0; }	// East
			//if(Y == -1){ SendToArduino[MotorPosition[?]] = '1';}	// SE
		}
		else if (X > -0.5 && X < 0.5){
			if(Y == 1){ SendToArduino[MotorPosition[3]] = '1'; Display.text = "POINTING NORTH"; cClean = 0; }	// North
			//if(Y == -1){ SendToArduino[MotorPosition[7]] = '1';} // South
		}

	}

	void GetFovealTouch(){
		Xf = Input.GetAxis ("HorFov");
		Yf = Input.GetAxis ("VerFov");

		if ((Xf >= -1 && Xf < -0.9) && (Yf <= 0.1 && Yf >= -0.9)) { 
			SendToArduino [MotorPosition [1]] = '1'; // West
			Display.text = "POINTING (Tactile Fovea)"; cClean = 0;
		} else if ((Xf >= -0.9 && Xf < -0.3) && (Yf <= 1 && Yf > 0.3)) {
			SendToArduino [MotorPosition [2]] = '1'; // NW
			Display.text = "POINTING (Tactile Fovea)"; cClean = 0;
		} else if ((Xf >= -0.1 && Xf <= 0.1) && (Yf <= 1 && Yf >= 0.9)) {
			SendToArduino [MotorPosition [3]] = '1';	// North
			Display.text = "POINTING (Tactile Fovea)"; cClean = 0;
		} else if ((Xf > 0.3 && Xf <= 0.9) && (Yf <= 1 && Yf > 0.3)) {
			SendToArduino [MotorPosition [4]] = '1';	// NE
			Display.text = "POINTING (Tactile Fovea)"; cClean = 0;
		} else if ((Xf > 0.9 && Xf <= 1) && (Yf <= 0.1 && Yf >= -0.9)) {
			SendToArduino [MotorPosition [5]] = '1';	// East
			Display.text = "POINTING (Tactile Fovea)"; cClean = 0;
		}
	}

	// main camera mirroring
	void OnPreCull () {
		camera.ResetWorldToCameraMatrix ();
		camera.ResetProjectionMatrix ();
		Vector3 x = new Vector3(-1, 1, 1);
		camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(x) ;
	}
	
	void OnPreRender () {
		GL.SetRevertBackfacing (true);
	}
	
	void OnPostRender () {
		GL.SetRevertBackfacing (false);
	}


	public static void OpenConnectionBelt(){
		if (_SerialPortBelt != null){
			if (_SerialPortBelt.IsOpen){
				_SerialPortBelt.Close();
				print("Closing port, because it was already open!");
			}
			else{
				//very important!, this opens the connection
				_SerialPortBelt.Open();
				//sets the timeout value (how long it takes before timeout error occurs)
				//zet de timeout niet te hoog, dit vertraagd Unity enorm en kan zelfs voor crashes zorgen.
				//_SerialPortBelt.ReadTimeout = 1000;
				_SerialPortBelt.WriteTimeout = 1000;
				portopenBelt = true;
			}
		}
		else{
			if (_SerialPortBelt.IsOpen){
				print("Port is already open");
			}else{
				print("Port == null");
			}
		}
	}

	void OnApplicationQuit() {
		//Debug.Log("Ultimo Tacton: " + Tacton[0] + Tacton[1] + Tacton[2] + Tacton[3] + Tacton[4] + Tacton[5] + Tacton[6] + Tacton[7]);
		
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
		
		_SerialPortBelt.Close();
		portopenBelt = false;
		
		//print ("saiu");
	}


}
