using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Threading;

public class BeforeTrials : MonoBehaviour {
	
	// Serial - Belt
	public static string COMPortBelt = "COM8";
	public static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBelt = false; //if port is open or not
	
	// Timer - Print tactons;
	public int enableDraw = 1;
	public int patternKey = 0;
	public System.Timers.Timer timerDrawWaveLess = new System.Timers.Timer(500);
	public int contCicle = 0;
	
	// Stimuli 
	//public static int[,] LatinSquare = new int[6, 6] {{1, 2, 3, 4, 5, 6}, {3, 4, 5, 6, 2, 1}, {6, 1, 2, 5, 4, 3}, {4, 5, 1, 3, 6, 2}, {2, 6, 4, 1, 3, 5}, {5, 3, 6, 2, 1, 4}};
	public static int[,] LatinSquare = new  int[6, 6] {{1, 2, 3, 4, 5, 6}, {2, 6, 4, 5, 3, 1}, {3, 4, 2, 6, 1, 5}, {4, 3, 5, 1, 6, 2}, {5, 1, 6, 2, 4, 3}, {6, 5, 1, 3, 2, 4}};
	public static int[,] LSquareFreq = new int[3, 3] {{1, 2, 3}, {2, 3, 1}, {3, 1, 2}};
	public static int[] StimuliOrder;
	public static int[] StimuliHead = new int[] {0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3};
	public static int[] StimuliNeck = new int[] {4,5,6,7,4,5,6,7,4,5,6,7,4,5,6,7,4,5,6,7};
	public static int[] StimuliTrunk = new int[] {8,9,10,11,8,9,10,11,8,9,10,11,8,9,10,11,8,9,10,11};
	public static int[] Stimuli = new int[] {0,1,2,3,4,5,6,7,8,9,10,11};
	public static int[] Direction = new int[] {0,2,1,3,0,2,1,3,0,2,1,3}; // all south for now
	public static string[] Answers = new string[] {"SphereN","SphereS","SphereE","SphereO"};
	public static string[] BodyLoci = new string[] {"HEAD","NECK","TRUNK","HEAD","NECK","TRUNK"};
	public static string bufferAnswer = "";
	public static string ComeFromArduino = "";
	public static char SignalFreqAmpl = '1';
	public static int UserId = 0;
	public static int UserIdLS = 0;
	public static int UserIdLSF = 0;
	public static int countform = 0;
	public static int Itr = 0;
	public static int ItrFq = 0;
	public static int GoTrial = 0;
	public static int nmbrSt = 20; // How many per site
	
	//GUI
	public GameObject BodySite;
	public GameObject StartButton;
	public GameObject StartButtonTxt;
	public static GameObject QuestionScreen;
	
	public static AudioSource audio;
	
	//Logs
	public DateTime start;
	public DateTime end;
	public string CorrAnswer; 
	public string TimeReactionMl = "";
	
	void Start(){
		audio = GetComponent<AudioSource>();
		start = DateTime.Now;
		Debug.Log("<logIdTask> Start demo/practice: " + start);
		UserId = PlayerPrefs.GetInt("Id");
		
		//LatinSquare BodySiteXAmplitude (6x6)
		if (UserId%6 == 0) UserIdLS = 5;
		else UserIdLS = (UserId%6) - 1;
		//LatinSquare Frequency (3x3)
		if (UserId%3 == 0) UserIdLSF = 2;
		else UserIdLSF = (UserId%3) - 1;
		
		Debug.Log("<logIdTask> User ID: " + UserId);
		
		BodySite = GameObject.Find("BodySite");
		BodySite.guiText.text = "DEMO";
		StimuliOrder = Stimuli;
		
		StartButton = GameObject.Find("Button");
		StartButtonTxt = GameObject.Find("ButtonTxt");
		QuestionScreen = GameObject.Find("QuestionScreen");
		
		//QuestionScreen.SetActive (false);
		
		OpenConnectionBelt();
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
				_SerialPortBelt.ReadTimeout = 1200;
				_SerialPortBelt.WriteTimeout = 1200;
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
	
	void TurnAllOptionsOff(){
		GameObject Sphere;
		for(int i=0;i<4;i++){
			Sphere = GameObject.Find(Answers[i]);
			Sphere.renderer.material.color = Color.white;
		}
	}

	void OnMouseDown() {
		if(GoTrial == 0){
			StartButton.renderer.material.color = Color.black;
			StartButtonTxt.guiText.text = "";
			GoTrial = 1;
		}else{
			Application.LoadLevel(3);
		}
	}
	
	void Update(){
		if(Input.GetKey(KeyCode.Escape))
			Application.Quit();

		if(Input.GetKeyUp(KeyCode.J)){
			OnApplicationQuit();
			Application.LoadLevel(3);
		}

		if(Input.GetKeyUp(KeyCode.R)){
			OnApplicationQuit();
			Application.LoadLevel(2);
		}
		
		if(portopenBelt && _SerialPortBelt.IsOpen){
			if(GoTrial == 1) Demo();
		}
	}
	
	void EnableDrawLess(object source, ElapsedEventArgs e){
		// Código a ser executado quando o intervalo for atingido
		enableDraw = 1;
	}
	
	void Demo(){	
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {10, 12, 14, 8, 18, 16, 6, 4, 20, 22, 2, 0};
		GameObject Sphere;
		
		if(patternKey < 12){
			SendToArduino[SendToArduinoPosition[StimuliOrder[patternKey]]] = '5';
		}else{
			GoTrial = 2;
			enableDraw = 0;
			patternKey = 0;
			StartButtonTxt.guiText.text = "NEXT";
			StartButton.renderer.material.color = Color.gray;
			OnApplicationQuit();
		}
		
		if(enableDraw == 1){			
			enableDraw = 0;
			contCicle++;
			switch(contCicle){
			case 1:	
				try{;
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					
				}catch{ Debug.Log ("<logIdTask> ERROR: _SerialPortBelt.Write()"); }
				break;
			case 2:
				Sphere = GameObject.Find(Answers[Direction[patternKey]]);
				Sphere.renderer.material.color = Color.red;
				break;
			case 4:	
				TurnAllOptionsOff();
				patternKey++;
				contCicle = 0;
				break;				
			}
			timerDrawWaveLess.Elapsed += new ElapsedEventHandler(EnableDrawLess);
			timerDrawWaveLess.Start();
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
