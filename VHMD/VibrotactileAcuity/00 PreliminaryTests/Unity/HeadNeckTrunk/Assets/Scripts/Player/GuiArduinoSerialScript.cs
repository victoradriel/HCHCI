using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Threading;

public class GuiArduinoSerialScript : MonoBehaviour{
	// Serial - Belt
	public static string COMPortBelt = "COM3";
	public static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBelt = false; //if port is open or not

	// Serial - Button
	public static string COMPortBtt = "COM6";
	public static SerialPort _SerialPortBtt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBtt = false; //if port is open or not

	// Timer - Print tactons;
	public int enableDraw = 1;
	public int patternKey = 0;
	public System.Timers.Timer timerDrawWave = new System.Timers.Timer(1000);
	public System.Timers.Timer timerDrawWaveInt = new System.Timers.Timer(200);
	public int contCicle = 0;
	public static int logFlag = 0;
	public static int regf = 0;

	public static char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};

	//GUI
	public GameObject StartButton;
	public GameObject StartButtonTxt;
	public static GameObject FirstSession;
	public static GameObject SecondSession;
	public static GameObject ThirdSession;

	public int fsession = 0;
	public int ssession = 0;
	public int tsession = 0;

	public static string[] FQd = {"PlaneF1", "PlaneF2", "PlaneF3", "PlaneF4", "PlaneF5", "PlaneF6", "PlaneF7", "PlaneF8", "PlaneF9", "PlaneF10", "PlaneF10"};
	public static string[] SQd = {"PlaneS1", "PlaneS2", "PlaneS3", "PlaneS4", "PlaneS5", "PlaneS6", "PlaneS7", "PlaneS8", "PlaneS9", "PlaneS10", "PlaneS10"};
	public static string[] TQd = {"PlaneT1", "PlaneT2", "PlaneT3", "PlaneT4", "PlaneT5", "PlaneT6", "PlaneT7", "PlaneT8", "PlaneT9", "PlaneT10", "PlaneT10"};

	/*int headRight = 0;
	int headLeft = 0;
	int neckRight = 0;
	int neckLeft = 0;
	int trunkRight = 0;
	int trunkLeft = 0;*/

	void Start(){
		StartButton = GameObject.Find("Button");
		StartButtonTxt = GameObject.Find("ButtonTxt");
		FirstSession = GameObject.Find("FirstSession");
		SecondSession = GameObject.Find("SecondSession");
		ThirdSession = GameObject.Find("ThirdSession");

		OpenConnectionBelt();
		//OpenConnectionBtt();
		EsconderOpcoes();
	}

	public static void EsconderOpcoes(){
		ThirdSession.SetActive (false);
		SecondSession.SetActive (false);
		FirstSession.SetActive (false);
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

	public static void OpenConnectionBtt(){
		if (_SerialPortBtt != null){
			if (_SerialPortBtt.IsOpen){
				_SerialPortBtt.Close();
				print("Closing port, because it was already open!");
			}
			else{
				//very important!, this opens the connection
				_SerialPortBtt.Open();
				//sets the timeout value (how long it takes before timeout error occurs)
				//zet de timeout niet te hoog, dit vertraagd Unity enorm en kan zelfs voor crashes zorgen.
				//_SerialPortBelt.ReadTimeout = 1000;
				_SerialPortBtt.WriteTimeout = 1000;
				portopenBtt = true;
			}
		}
		else{
			if (_SerialPortBtt.IsOpen){
				print("Port is already open");
			}else{
				print("Port == null");
			}
		}
	}

	void OnMouseDown() {
		// start:logtime
		if(fsession == 0){
			fsession = 1;
			StartButtonTxt.guiText.text = "NEXT";
		}else if(fsession == 2 && ssession == 0){
			ssession = 1;
			patternKey = 0;
		}else if(ssession == 2 && tsession == 0){
			tsession = 1;
			patternKey = 0;
		}
	}

	void Update(){
		if(Input.GetKey(KeyCode.Escape))
			Application.Quit();

		if(portopenBelt && _SerialPortBelt.IsOpen){
			if(fsession == 1){
				FirstSession.SetActive (true);
				FirstSessionToBelt();
				//if option=verde: ssession = 1
			}else if(ssession == 1){
				SecondSession.SetActive (true);
				SecondSessionToBelt();
				//if option=verde: ssession = 1
			} else if(tsession == 1){
				ThirdSession.SetActive (true);
				ThirdSessionToBelt();
				//if option=verde: ssession = 1
			}
		}
	}

	void EnableDraw(object source, ElapsedEventArgs e){
		// Código a ser executado quando o intervalo for atingido
		enableDraw = 1;
	}

	void FirstSessionToBelt(){
		char[] SendToArduinoZero = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		GameObject status = GameObject.Find(FQd[patternKey]);
		GameObject TitleF = GameObject.Find("TrialOneFirst");
		GameObject TitleS = GameObject.Find("TrialOneSecond");

		switch(patternKey){
		case 0:
			TitleS.guiText.color = Color.gray;
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='1';
			break;
		case 1:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='1';
			break;
		case 2:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='1';
			break;
		case 3:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='1';
			break;
		case 4:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='1';
			break;
		case 5:
			TitleS.guiText.color = Color.white;
			TitleF.guiText.color = Color.gray;
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='2';
			break;
		case 6:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='2';
			break;
		case 7:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='2';
			break;
		case 8:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='2';
			break;
		case 9:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='2';
			break;
		case 10:
			_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
			enableDraw = 0; 
			status.renderer.material.color = Color.white;
			TitleF.guiText.color = Color.white;
			fsession = 2;
			_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
			break;
		}

		if(enableDraw == 1){			
			enableDraw = 0;
			contCicle++;
			switch(contCicle){
			case 1:		
				status.renderer.material.color = Color.gray;
				_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
				print ("1-> " + SendToArduino[0] + "," +SendToArduino[2] + "," +SendToArduino[4] + "," +SendToArduino[6] + "," +SendToArduino[8] + "," +SendToArduino[10] + "," +SendToArduino[12] + "," +SendToArduino[14] + "," +SendToArduino[16]);
				break;
			case 2:		
				SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
				_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);	
				print ("2-> " + SendToArduino[0] + "," +SendToArduino[2] + "," +SendToArduino[4] + "," +SendToArduino[6] + "," +SendToArduino[8] + "," +SendToArduino[10] + "," +SendToArduino[12] + "," +SendToArduino[14] + "," +SendToArduino[16]);
				break;
			case 3:
				status.renderer.material.color = Color.white;
				patternKey++;
				contCicle = 0;
				break;				
			}
			timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
			timerDrawWave.Start();
		}
	}

	void SecondSessionToBelt(){		
		char[] SendToArduinoZero = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		GameObject status = GameObject.Find(SQd[patternKey]);
		GameObject TitleF = GameObject.Find("TrialTwoFirst");
		GameObject TitleS = GameObject.Find("TrialTwoSecond");
		
		switch(patternKey){
		case 0:
			TitleS.guiText.color = Color.gray;
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='1';
			break;
		case 1:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='1';
			break;
		case 2:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='1';
			break;
		case 3:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='1';
			break;
		case 4:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='1';
			break;
		case 5:
			TitleS.guiText.color = Color.white;
			TitleF.guiText.color = Color.gray;
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='2';
			break;
		case 6:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='2';
			break;
		case 7:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='2';
			break;
		case 8:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='2';
			break;
		case 9:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='2';
			break;
		case 10:
			_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
			enableDraw = 0; 
			status.renderer.material.color = Color.white;
			TitleF.guiText.color = Color.white;
			ssession = 2;
			_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
			break;
		}

		if(enableDraw == 1){			
			enableDraw = 0;
			contCicle++;
			switch(contCicle){
			case 1:		
				status.renderer.material.color = Color.gray;
				_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
				print ("1-> " + SendToArduino[0] + "," +SendToArduino[2] + "," +SendToArduino[4] + "," +SendToArduino[6] + "," +SendToArduino[8] + "," +SendToArduino[10] + "," +SendToArduino[12] + "," +SendToArduino[14] + "," +SendToArduino[16]);
				break;
			case 2:		
				SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
				_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);	
				print ("2-> " + SendToArduino[0] + "," +SendToArduino[2] + "," +SendToArduino[4] + "," +SendToArduino[6] + "," +SendToArduino[8] + "," +SendToArduino[10] + "," +SendToArduino[12] + "," +SendToArduino[14] + "," +SendToArduino[16]);
				break;
			case 3:
				status.renderer.material.color = Color.white;
				patternKey++;
				contCicle = 0;
				break;				
			}
			timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
			timerDrawWave.Start();
		}

	}

	void ThirdSessionToBelt(){		
		char[] SendToArduinoZero = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		GameObject status = GameObject.Find(TQd[patternKey]);
		GameObject TitleF = GameObject.Find("TrialThreeFirst");
		GameObject TitleS = GameObject.Find("TrialThreeSecond");
		
		switch(patternKey){
		case 0:
			TitleS.guiText.color = Color.gray;
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='1';
			break;
		case 1:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='1';
			break;
		case 2:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='1';
			break;
		case 3:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='1';
			break;
		case 4:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='1';
			break;
		case 5:
			TitleS.guiText.color = Color.white;
			TitleF.guiText.color = Color.gray;
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='2';
			break;
		case 6:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='2';
			break;
		case 7:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[1]]='2';
			break;
		case 8:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='2';
			break;
		case 9:
			SendToArduino = SendToArduinoZero;
			SendToArduino[SendToArduinoPosition[0]]='2';
			break;
		case 10:
			enableDraw = 0; 
			status.renderer.material.color = Color.white;
			TitleF.guiText.color = Color.white;
			tsession = 2;
			//_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
			break;
		}
		
		if(enableDraw == 1){			
			enableDraw = 0;
			contCicle++;
			switch(contCicle){
			case 1:		
				status.renderer.material.color = Color.gray;
				_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
				print ("1-> " + SendToArduino[0] + "," +SendToArduino[2] + "," +SendToArduino[4] + "," +SendToArduino[6] + "," +SendToArduino[8] + "," +SendToArduino[10] + "," +SendToArduino[12] + "," +SendToArduino[14] + "," +SendToArduino[16]);
				break;
			case 2:		
				SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
				_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);	
				print ("2-> " + SendToArduino[0] + "," +SendToArduino[2] + "," +SendToArduino[4] + "," +SendToArduino[6] + "," +SendToArduino[8] + "," +SendToArduino[10] + "," +SendToArduino[12] + "," +SendToArduino[14] + "," +SendToArduino[16]);
				break;
			case 3:
				status.renderer.material.color = Color.white;
				patternKey++;
				contCicle = 0;
				break;				
			}
			timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
			timerDrawWave.Start();
		}
	}

	void OnApplicationQuit() {
		//Debug.Log("Ultimo Tacton: " + Tacton[0] + Tacton[1] + Tacton[2] + Tacton[3] + Tacton[4] + Tacton[5] + Tacton[6] + Tacton[7]);
		
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
		
		_SerialPortBelt.Close();
		portopenBelt = false;

		_SerialPortBtt.Close();
		portopenBtt = false;
		
		//print ("saiu");
	}
	
}