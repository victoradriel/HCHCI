using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Threading;

public class Treino : MonoBehaviour {

	// Serial - Belt
	public static string COMPortBelt = "COM8";
	public static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBelt = false; //if port is open or not
	
	// Timer - Print tactons;
	public int enableDraw = 1;
	public int patternKey = 0;
	public System.Timers.Timer timerDrawWave = new System.Timers.Timer(1200);
	public int contCicle = 0;
	
	// Stimuli 
	//public static int[,] LatinSquare = new int[6, 6] {{1, 2, 3, 4, 5, 6}, {3, 4, 5, 6, 2, 1}, {6, 1, 2, 5, 4, 3}, {4, 5, 1, 3, 6, 2}, {2, 6, 4, 1, 3, 5}, {5, 3, 6, 2, 1, 4}};
	public static int[,] LatinSquare = new  int[6, 6] {{1, 2, 3, 4, 5, 6}, {2, 6, 4, 5, 3, 1}, {3, 4, 2, 6, 1, 5}, {4, 3, 5, 1, 6, 2}, {5, 1, 6, 2, 4, 3}, {6, 5, 1, 3, 2, 4}};
	public static int[,] LSquareFreq = new int[3, 3] {{1, 2, 3}, {2, 3, 1}, {3, 1, 2}};
	public static int[] StimuliOrder;
	public static int[] RandomDelay = new int[] {1200,1500,1800,2100,1200,1500,1800,2100,1200,1500,1800,2100,1200,1500,1800,2100,1200,1500,1800,2100,1200};
	public static int[] StimuliHead = new int[] {0,1,2,3,0,1,2,3};
	public static int[] StimuliNeck = new int[] {4,5,6,7,4,5,6,7};
	public static int[] StimuliTrunk = new int[] {8,9,10,11,8,9,10,11};
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
	public float audioVolume = 0.0F;
	
	//Logs
	public DateTime start;
	public DateTime end;
	public string CorrAnswer; 
	public string TimeReactionMl = "";
	
	void Start(){
		audio = GetComponent<AudioSource>();
		audio.Play();
		audio.volume = 0.0F;

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

		ItrFq = 2;
		Itr = 5;

		SetStimuliOrder(LatinSquare[UserIdLS,Itr]);
		BodySite = GameObject.Find("BodySite");
		BodySite.guiText.text = BodyLoci[(LatinSquare[UserIdLS,Itr])-1];
		
		StartButton = GameObject.Find("Button");
		StartButtonTxt = GameObject.Find("ButtonTxt");
		QuestionScreen = GameObject.Find("QuestionScreen");
		
		QuestionScreen.SetActive (false);

		RandomDelay = MyFisherYatesShfflInt(RandomDelay);

		OpenConnectionBelt();
	}
	
	public void SetStimuliOrder(int LtnSqr){
		int SignalInt = LSquareFreq[UserIdLSF,ItrFq];
		switch(LtnSqr){
		case 1:
			StimuliOrder = MyFisherYatesShfflInt(StimuliHead);
			SignalFreqAmpl = char.Parse(SignalInt.ToString()); // Signals 1, 2, 3 for Ampl1
			break;
		case 2:
			StimuliOrder = MyFisherYatesShfflInt(StimuliNeck);
			SignalFreqAmpl = char.Parse(SignalInt.ToString());
			break;
		case 3:
			StimuliOrder = MyFisherYatesShfflInt(StimuliTrunk);
			SignalFreqAmpl = char.Parse(SignalInt.ToString());
			break;
		case 4:
			StimuliOrder = MyFisherYatesShfflInt(StimuliHead);
			SignalInt += 3;
			SignalFreqAmpl = char.Parse(SignalInt.ToString()); // Signals 4, 5, 6 for Ampl2
			break;
		case 5:
			StimuliOrder = MyFisherYatesShfflInt(StimuliNeck);
			SignalInt += 3;
			SignalFreqAmpl = char.Parse(SignalInt.ToString());
			break;
		case 6:
			StimuliOrder = MyFisherYatesShfflInt(StimuliTrunk);
			SignalInt += 3;
			SignalFreqAmpl = char.Parse(SignalInt.ToString());
			break;
		}
	}
	
	public static int[] MyFisherYatesShfflInt(int[] IntArray){
		//Fisher–Yates Shuffle variation
		int ArrayLength = IntArray.Length;
		for (int i = 0; i < ArrayLength; i++) {
			int temp = IntArray[i];
			int randomIndex = UnityEngine.Random.Range(i, ArrayLength-1);
			if((i > 0)&&(i < ArrayLength-2)){
				if(IntArray[i-1] == IntArray[randomIndex])
					randomIndex++;
			}
			IntArray[i] = IntArray[randomIndex];
			IntArray[randomIndex] = temp;
		}
		return IntArray;
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
	
	void TurnOptionOn(string dirAnswer){
		GameObject Sphere;
		switch (dirAnswer){
		case "North":
			Sphere = GameObject.Find(Answers[0]);
			Sphere.renderer.material.color = Color.black;
			bufferAnswer = Answers[0];
			break;
		case "South":
			Sphere = GameObject.Find(Answers[1]);
			Sphere.renderer.material.color = Color.black;
			bufferAnswer = Answers[1];
			break;
		case "East":
			Sphere = GameObject.Find(Answers[2]);
			Sphere.renderer.material.color = Color.black;
			bufferAnswer = Answers[2];
			break;
		case "West":
			Sphere = GameObject.Find(Answers[3]);
			Sphere.renderer.material.color = Color.black;
			bufferAnswer = Answers[3];
			break;
		}
		contCicle = 9;
	}
	
	void CheckAnswer(int patternKey){
		if(bufferAnswer == Answers[Direction[patternKey]]){
			Debug.Log("<logIdTask> Right answer: " + bufferAnswer);
			CorrAnswer = "1";
		}else{
			Debug.Log("<logIdTask> Wrong answer: " + bufferAnswer + ". It should be " + Answers[Direction[patternKey]]);
			CorrAnswer = "0";
		}
	}
	
	
	void OnMouseDown() {
		if(GoTrial == 0){
			StartButton.renderer.material.color = Color.black;
			StartButtonTxt.guiText.text = "";
			GoTrial = 1;
		}else{
			Application.LoadLevel(4);
		}
	}
	
	void InputGetButt(string entrada){
		string[] answerBtt = entrada.Split(' ');
		TurnAllOptionsOff();
		Debug.Log ("<logIdTask> Answer: " + answerBtt[0] + " - TimeReaction: " + answerBtt[1]);
		TurnOptionOn(answerBtt[0]);
		
		TimeReactionMl = answerBtt[1];
	}
	
	void Update(){
		if(Input.GetKey(KeyCode.Escape))
			Application.Quit();

		if(Input.GetKeyUp(KeyCode.J)){
			OnApplicationQuit();
			Application.LoadLevel(4);
		}

		if(Input.GetKeyUp(KeyCode.R)){
			OnApplicationQuit();
			Application.LoadLevel(3);
		}
		
		if(portopenBelt && _SerialPortBelt.IsOpen){
			if(GoTrial == 1) Trial();
		}

		if (GoTrial == 1)
			fadeIn();
	}
	
	void EnableDraw(object source, ElapsedEventArgs e){
		// Código a ser executado quando o intervalo for atingido
		enableDraw = 1;
	}

	void Trial(){
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		//char[] SendToArduinoZero = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {10, 12, 14, 8, 18, 16, 6, 4, 20, 22, 2, 0};
		int controle;
		
		if(patternKey < 8){
			SendToArduino[SendToArduinoPosition[StimuliOrder[patternKey]]] = SignalFreqAmpl;
			controle = patternKey%2;
			SendToArduino[24] = char.Parse(controle.ToString());
		}else{
			GoTrial = 2;
			audio.volume = 0.0F;
			enableDraw = 0;
			patternKey = 0;
			end = DateTime.Now;
			Debug.Log ("<logIdTask> End of demo/practice: " + end);
			BodySite.guiText.text = "Ready?";
			StartButtonTxt.guiText.text = "NEXT";
			OnApplicationQuit();

			StartButton.renderer.material.color = Color.gray;
		}
		
		if(enableDraw == 1){			
			enableDraw = 0;
			contCicle++;
			switch(contCicle){
			case 1:	
				try{
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
				}catch{
					Debug.Log ("<logIdTask> ERROR: _SerialPortBelt.Write()");
				}
				QuestionScreen.SetActive (true);
				break;
			case 2:	
				if(patternKey == nmbrSt-1){ audio.volume = 0.6F; }
				try{
					ComeFromArduino = _SerialPortBelt.ReadLine();
					string[] ComeFromArduinoCheck = ComeFromArduino.Split(':');
					if(int.Parse(ComeFromArduinoCheck[0]) == patternKey%2){
						InputGetButt(ComeFromArduinoCheck[1]);
					}
				}catch(TimeoutException ex){
					Debug.Log ("<logIdTask> Timeout1" + ex);
				}
				break;
			case 3:		
				try{
					ComeFromArduino = _SerialPortBelt.ReadLine();
					string[] ComeFromArduinoCheck = ComeFromArduino.Split(':');
					if(int.Parse(ComeFromArduinoCheck[0]) == patternKey%2){
						InputGetButt(ComeFromArduinoCheck[1]);
					}
				}catch(TimeoutException ex){
					Debug.Log ("<logIdTask> Timeout2" + ex);
					contCicle = 9;
				}
				break;
			case 10:
				if(patternKey == nmbrSt-1){ audio.volume = 0.3F; }
				CheckAnswer(StimuliOrder[patternKey]);
				TurnAllOptionsOff();
				patternKey++;
				QuestionScreen.SetActive (false);
				contCicle = 0;
				break;				
			}
			if(contCicle == 0){
				timerDrawWave.Interval = RandomDelay[patternKey];
				timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
				timerDrawWave.Start();
			}else{
				timerDrawWave.Interval = 1200;
				timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
				timerDrawWave.Start();
			}
		}
	}

	void fadeIn() {
		if (audioVolume < 1.0F) {
			audioVolume += 0.2F * Time.deltaTime;
			audio.volume = audioVolume;
		}
	}
	
	void fadeOut() {
		if(audioVolume > 0.0F){
			audioVolume -= 0.2F * Time.deltaTime;
			audio.volume = audioVolume;
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
