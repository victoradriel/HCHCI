using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Collections.Generic;

public class BeltSerialScript : MonoBehaviour{
	// Serial - Belt
	public static string COMPortBelt = "COM4";
	public static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBelt = false; //if port is open or not
	// Serial - Input
	public static string COMPort = "COM5";
	public static SerialPort _SerialPort = new SerialPort(COMPort, 9600); //COM port and baudrate	
	public static bool portopen = false; //if port is open or not
	// Update control
	public static int updateslower = 0;
	public static int updateinterval = 2; //50 seems interesting

	// SendToArduinoGlobal nao esta sendo usado
	char[] SendToArduinoGlobal = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};

	int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
	public char[] Tacton = new char[] {'0','0','0','0','0','0','0','0'}; // Tacton
	public char[] TactonHeard = new char[] {'0','0','0','0','0','0','0','0'}; // Tacton
	public int PHeard = 666;
	public string[] motorsClient = {"MotorClient01", "MotorClient02", "MotorClient03", "MotorClient04", "MotorClient05", "MotorClient06", "MotorClient07", "MotorClient08"};
	public string[] motorsServer = {"MotorServer01", "MotorServer02", "MotorServer03", "MotorServer04", "MotorServer05", "MotorServer06", "MotorServer07", "MotorServer08"};
	public int heIsNotTalking = 1;
	public int StillHearHim = 0;
	public int IamNotTalking = 1;
	int need2Clean = 0;
	//GameObject DebugText;
	
	void Start (){
		OpenConnection();
		OpenConnectionBelt();
		//DebugText = GameObject.Find("DebugNetwork");
	}

	public static void OpenConnection(){
		if (_SerialPort != null){
			if (_SerialPort.IsOpen){
				/*_SerialPort.Close();
				print("Closing port, because it was already open!");*/
			} else{
				try{
					_SerialPort.Open();
					_SerialPort.ReadTimeout = 100;
					portopen = true;
					print("TactileBelt >> Openning port: " + COMPort);
				}catch(Exception error){
					print ("Running without the Tactile Belt. >> " + error);
				}
			}
		}
		else{
			_SerialPort = new SerialPort(COMPort, 57600);
			if (_SerialPort.IsOpen){
				print("Port is already open");
			} else{
				_SerialPort.Open();
			}
		}
	}

	public static void OpenConnectionBelt(){
		if (_SerialPortBelt != null){
			if (_SerialPortBelt.IsOpen){
				/*_SerialPort.Close();
				print("Closing port, because it was already open!");*/
			} else{
				try{
					_SerialPortBelt.Open();
					_SerialPortBelt.ReadTimeout = 100;
					portopenBelt = true;
					print("TactileBelt >> Openning port: " + COMPortBelt);
				}catch(Exception error){
					print ("Running without the Tactile Belt. >> " + error);
				}
			}
		}
		else{
			_SerialPortBelt = new SerialPort(COMPortBelt, 57600);
			if (_SerialPortBelt.IsOpen){
				print("Port is already open");
			} else{
				_SerialPortBelt.Open();
			}
		}
	}

	public static void AskAboutCOnnection(){
		loadTest.Log ("<####> BELT-INFO-ERROR portopenBelt: " + portopenBelt);

		if (_SerialPortBelt != null)
			loadTest.Log ("<####> BELT-INFO-ERROR _SerialPortBelt value: not null");
		else
			loadTest.Log ("<####> BELT-INFO-ERROR _SerialPortBelt value: NULL");

		if (_SerialPortBelt.IsOpen)
			loadTest.Log ("<####> BELT-INFO-ERROR _SerialPortBelt status: OPEN");
		else
			loadTest.Log ("<####> BELT-INFO-ERROR _SerialPortBelt status: Closed");
	}

	public static void TryNewConnection(){
		loadTest.Log ("<####> BELT-NOTE TryNewConnection() ");
		try{
			_SerialPortBelt.Close();
			loadTest.Log ("<####> BELT-NOTE TryNewConnection() - Closing port: " + COMPortBelt);
		}catch{
			loadTest.Log ("<####> BELT-NOTE TryNewConnection() - ERROR in Closing" + COMPortBelt);
		}

		try{
			OpenConnectionBelt();
		}catch{
			loadTest.Log ("<####> BELT-NOTE TryNewConnection() - ERROR in Opening" + COMPortBelt);
		}

	}
	
	public static void CloseConnection(){
		if (portopen == true){
			try{
				_SerialPort.Close();
				print("TactileBelt >> Closing port: " + COMPort);
			}catch{
				print("TactileBelt >> ERROR in closing" + COMPort);
			}
		}
		if (portopenBelt == true) {
			try{
				_SerialPortBelt.Close();
				print("TactileBelt >> Closing port: " + COMPortBelt);
			}catch{
				print("TactileBelt >> ERROR in closing" + COMPortBelt);
			}
		}
	}
	
	[RPC]
	public void ChangePositionClient(int id){
		GameObject motor = GameObject.Find(motorsClient[id]);
		motor.transform.position = new Vector3 (0f, 0f, 0.1f);
	}
	
	[RPC]
	public void ReturnPositionClient(int id){
		GameObject motor = GameObject.Find(motorsClient[id]);
		motor.transform.position = new Vector3 (0f, 0f, 0f);
	}
	
	[RPC]
	public void ChangePositionServer(int id){
		GameObject motor = GameObject.Find(motorsServer[id]);
		motor.transform.position = new Vector3 (0.1f, 0f, 0.1f);
	}
	
	[RPC]
	public void ReturnPositionServer(int id){
		GameObject motor = GameObject.Find(motorsServer[id]);
		motor.transform.position = new Vector3 (0.1f, 0f, 0f); 
	}
	
	void Update (){
		if (Input.GetKeyDown(KeyCode.W)){
			TryNewConnection();
		}

		updateslower++;		
		if (_SerialPort.IsOpen && _SerialPortBelt.IsOpen && updateslower>=updateinterval) {
			updateslower = 0;
			Listen();
			if(heIsNotTalking == '1') 
				AskForInput ();
			else{ 
				WhatIsHeTalking();
			}
		}
	}
	
	void AskForInput(){
		try{
			int fromArduino = int.Parse(_SerialPort.ReadLine());
			IamNotTalking = 0;
			if(Tacton[fromArduino] == '0'){
				Tacton[fromArduino] = '1';
				loadTest.Log ("<####> BELT-TOUCH Ativa-motor, " + fromArduino + "," + Time.fixedTime);	
				AtivaMotor(fromArduino);
				AtivaMotorAlheio(fromArduino);
			}
		}catch{
			// Desativa motores virtuais:
			for (int i=0; i<8; i++) {
				if (Tacton [i] == '1') {
					Tacton [i] = '0';
					//DesativaMotor(i);
					DesativaMotorAlheio (i);
					loadTest.Log ("<####> BELT-TOUCH Desativa-motor, " + i + "," + Time.fixedTime);
				}
				if (TactonHeard [i] == '1') {
					TactonHeard [i] = '0';
					//DesativaMotor(i);
					loadTest.Log ("<####> BELT-LISTENER Desativa-motor, " + i + "," + Time.fixedTime);
				}
			}

			// Desativa motores reais:
			if (IamNotTalking == 0){
				DesativaMotores ();
				IamNotTalking = 1;
			}

			if (StillHearHim == 1){
				DesativaMotores ();
				StillHearHim = 0;
			}
		}
	}
	
	public void Listen() {
		if (TodosDesativados ()) 
			heIsNotTalking = '1';
		else
			heIsNotTalking = '0';
	}
	
	public void WhatIsHeTalking() {
		if (Network.isClient) {
			if(TactonHeard[PHeard] == '0'){
				TactonHeard[PHeard] = '1';
				StillHearHim = 1;
				print ("He is talking: ativa " + PHeard);
				AtivaMotor(PHeard);
				loadTest.Log ("<####> BELT-LISTENER Ativa-motor, " + PHeard + "," + Time.fixedTime);
			}
		}else if(Network.isServer){
			if(TactonHeard[PHeard] == '0'){
				TactonHeard[PHeard] = '1';
				StillHearHim = 1;
				print ("He is talking: ativa " + PHeard);
				AtivaMotor(PHeard); 
				loadTest.Log ("<####> BELT-LISTENER Ativa-motor, " + PHeard + "," + Time.fixedTime);
			}
		}
	}
	
	bool TodosDesativados(){
		if (Network.isClient) {
			for(int i=0;i<8;i++){
				GameObject motor = GameObject.Find(motorsClient[i]);
				if (motor.transform.position.z == 0.1f){
					PHeard = i;
					return false;
				}
			}
		}else if(Network.isServer){
			for(int i=0;i<8;i++){
				GameObject motor = GameObject.Find(motorsServer[i]);
				if (motor.transform.position.z == 0.1f){
					PHeard = i;
					return false;
				}
			}
		}
		PHeard = 666;
		return true;
	}
	
	public void AtivaMotorAlheio(int id) {
		if (Network.isClient) {
			GetComponent<NetworkView>().RPC ("ChangePositionServer", RPCMode.All, id);
		}else if (Network.isServer){
			GetComponent<NetworkView>().RPC ("ChangePositionClient", RPCMode.All, id);
		}
	}
	
	public void DesativaMotorAlheio(int id) {
		if (Network.isClient) {
			GetComponent<NetworkView>().RPC ("ReturnPositionServer", RPCMode.All, id);
		}else if (Network.isServer){
			GetComponent<NetworkView>().RPC ("ReturnPositionClient", RPCMode.All, id);
		} 
	}
	
	public void AtivaMotor(int id) {
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		SendToArduino[SendToArduinoPosition[id]] = '1';
		//DebugText.guiText.text = "Imprimindo: " + SendToArduino [0] + "." + SendToArduino [2] + "." + SendToArduino [4] + "." + SendToArduino [6] + "." + SendToArduino [8] + "." + SendToArduino [10] + "." + SendToArduino [12] + "." + SendToArduino [14]; 
		try{
			_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
			loadTest.Log ("<####> BELT-NOTE AtivaMotor() Sending...," + Time.fixedTime + " Imprimindo: " + SendToArduino [0] + "." + SendToArduino [2] + "." + SendToArduino [4] + "." + SendToArduino [6] + "." + SendToArduino [8] + "." + SendToArduino [10] + "." + SendToArduino [12] + "." + SendToArduino [14]); 
		}catch{
			loadTest.Log ("<####> BELT-ERROR AtivaMotor() Not sending...," + Time.fixedTime);
			AskAboutCOnnection();
			TryNewConnection();
		}
	}
	
	public void DesativaMotor(int id) {
		SendToArduinoGlobal[SendToArduinoPosition[id]] = '0';
		//DebugText.guiText.text = "Imprimindo: " + SendToArduino [0] + "." + SendToArduino [2] + "." + SendToArduino [4] + "." + SendToArduino [6] + "." + SendToArduino [8] + "." + SendToArduino [10] + "." + SendToArduino [12] + "." + SendToArduino [14]; 
		try{
			_SerialPortBelt.Write(SendToArduinoGlobal, 0, SendToArduinoGlobal.Length);
			loadTest.Log ("<####> BELT-NOTE DesativaMotor() Sending...," + Time.fixedTime + " Imprimindo: " + SendToArduinoGlobal [0] + "." + SendToArduinoGlobal [2] + "." + SendToArduinoGlobal [4] + "." + SendToArduinoGlobal [6] + "." + SendToArduinoGlobal [8] + "." + SendToArduinoGlobal [10] + "." + SendToArduinoGlobal [12] + "." + SendToArduinoGlobal [14]); 
		}catch{
			loadTest.Log ("<####> BELT-ERROR DesativaMotor() Not sending...," + Time.fixedTime);
			AskAboutCOnnection();
			TryNewConnection();
		}
	}

	public void DesativaMotores() {
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};

		try{
			_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
			loadTest.Log ("<####> BELT-NOTE DesativaMotores() Sending...," + Time.fixedTime + " Imprimindo: " + SendToArduino [0] + "." + SendToArduino [2] + "." + SendToArduino [4] + "." + SendToArduino [6] + "." + SendToArduino [8] + "." + SendToArduino [10] + "." + SendToArduino [12] + "." + SendToArduino [14]); 
		}catch{
			loadTest.Log ("<####> BELT-ERROR DesativaMotores() Not sending...," + Time.fixedTime);
			AskAboutCOnnection();
			TryNewConnection();
		}
	}
	
	void GravaLog(){
		//loadTest.Log("TactileBelt_Info: ");
	}		
	
	void OnApplicationQuit() {
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		if (_SerialPortBelt.IsOpen){
			_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
		}
		GravaLog();
		
		CloseConnection();
		portopen = false;
	}
}