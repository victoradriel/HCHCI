using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Threading;

public class CalibrationTask : MonoBehaviour {
	
	// Serial - Belt
	public static string COMPortBelt = "COM8";
	public static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBelt = false; //if port is open or not
	
	// Timer - Print tactons;
	public int enableDraw = 1;
	public int patternKey = 0;
	public System.Timers.Timer timerDrawWave = new System.Timers.Timer(1000);
	public int contCicle = 0;
	
	// Stimuli 
	public static int[] StimuliOrder = new int[48];
	public static int[] RandomDelay = new int[] {1000,1300,1600,1900,1000,1300,1600,1900,1000,1300,1600,1900,1000,1300,1600,1900,1000,1300,1600,1900,1000,1300,1600,1900,1000,1300,1600,1900,1000,1300,1600,1900,1000,1300,1600,1900,1000,1300,1600,1900,1000,1300,1600,1900,1000,1300,1600,1900,1000};
	public static int[] StimuliExtra = new int[] {0,1,2,3,0,1,2,3};
	public static int[] StimuliHead = new int[] {0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3};
	public static int[] Stimuli = new int[] {0,1,2,3,4,5,6,7,8,9,10,11};
	public static int[] Direction = new int[] {0,2,1,3,0,2,1,3,0,2,1,3}; // all south for now
	public static string[] Answers = new string[] {"SphereN","SphereS","SphereE","SphereO"};
	public static string[] BodyLoci = new string[] {"HEAD","NECK","TRUNK","HEAD","NECK","TRUNK"};
	public static string bufferAnswer = "";
	public static string TurnLedOn = "";
	public static string ComeFromArduino = "";
	public static char Amplitude = '3';
	public static int UserId = 0;
	public static int UserIdLS = 0;
	public static int Itr = 0;
	public static int GoTrial = 0;
	public static int nmbrSt = 48; // 48 
	
	//GUI
	public GameObject BodySite;
	public GameObject StartButton;
	public GameObject StartButtonTxt;
	public GameObject Button;
	
	//Logs
	public string LOGlogs = "";
	public string LOGTotals = "";
	public DateTime start;
	public DateTime end;
	public string CorrAnswer; 
	public string TimeReactionMl = "";
	public static int[] Acertos = new int[] {0,0,0,0,0,0,0,0,0,0,0,0};
	public static int[] ReacTim = new int[] {0,0,0,0,0,0,0,0,0,0,0,0};

	public static AudioSource audio;
	public float audioVolume = 0.0F;
	
	void Start(){
		audio = GetComponent<AudioSource>();
		audio.Play();
		audio.volume = 0.0F;

		start = DateTime.Now;
		Debug.Log("<logIdTask> Start: " + start);
		UserId = PlayerPrefs.GetInt("Id");
		Debug.Log("<logIdTask> User ID: " + UserId);

		//StimuliOrder = MyFisherYatesShfflInt(StimuliHead);
		StimuliHead = MyFisherYatesShfflInt(StimuliHead);
		StimuliExtra = MyFisherYatesShfflInt(StimuliExtra);
		StimuliExtra.CopyTo (StimuliOrder, 0);
		StimuliHead.CopyTo (StimuliOrder, StimuliExtra.Length);
		
		StartButton = GameObject.Find("Button");
		StartButtonTxt = GameObject.Find("ButtonTxt");

		RandomDelay = MyFisherYatesShfflInt(RandomDelay);

		OpenConnectionBelt();
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
				_SerialPortBelt.ReadTimeout = 1000;
				_SerialPortBelt.WriteTimeout = 500;
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
			Sphere.renderer.material.color = Color.magenta;
			bufferAnswer = Answers[0];
			break;
		case "South":
			Sphere = GameObject.Find(Answers[1]);
			Sphere.renderer.material.color = Color.magenta;
			bufferAnswer = Answers[1];
			break;
		case "East":
			Sphere = GameObject.Find(Answers[2]);
			Sphere.renderer.material.color = Color.magenta;
			bufferAnswer = Answers[2];
			break;
		case "West":
			Sphere = GameObject.Find(Answers[3]);
			Sphere.renderer.material.color = Color.magenta;
			bufferAnswer = Answers[3];
			break;
		case " ":
			bufferAnswer = " ";
			break;
		}
		contCicle = 9;
	}
	
	void CheckAnswer(int patternKey){
		if(bufferAnswer == Answers[Direction[patternKey]]){
			Debug.Log("<logIdTask> Right answer: " + bufferAnswer);
			CorrAnswer = "1";
			Acertos[patternKey]++;
		}else{
			Debug.Log("<logIdTask> Wrong answer: " + bufferAnswer + ". It should be " + Answers[Direction[patternKey]]);
			CorrAnswer = "0";
		}
		LOGlogs = LOGlogs + CorrAnswer + ";" + TimeReactionMl + ";";
		
		if(TimeReactionMl != ""){
			ReacTim[patternKey]+= int.Parse(TimeReactionMl);
		}
	}
	
	
	void OnMouseDown() {
		if(Itr < 1){
			StartButton.renderer.material.color = Color.black;
			StartButtonTxt.guiText.text = "";
			GoTrial = 1;
		}else{
			Application.LoadLevel(2);
		}
	}
	
	void InputGetButt(string entrada){
		string[] answerBtt = entrada.Split(' ');
		TurnAllOptionsOff();
		if (answerBtt.Length > 1){
			Debug.Log ("<logIdTask> Answer: " + answerBtt[0] + " - TimeReaction: " + answerBtt[1]);
			TurnOptionOn(answerBtt[0]);
			TimeReactionMl = answerBtt[1];
		}else{
			Debug.Log ("<logIdTask> Answer: Out of Time ");
			TurnOptionOn(" ");
			TimeReactionMl = "0";
		}
	}
	
	void Update(){
		if(Input.GetKey(KeyCode.Escape))
			Application.Quit();
		
		if(Input.GetKey(KeyCode.Space))
			OnMouseDown();
		
		if(Input.GetKeyUp(KeyCode.J)){
			OnApplicationQuit();
			Application.LoadLevel(2);
		}

		if(Input.GetKeyUp(KeyCode.R)){
			OnApplicationQuit();
			Application.LoadLevel(1);
		}
		
		if(portopenBelt && _SerialPortBelt.IsOpen){
			if(GoTrial == 1) Trial();
		}

		if (GoTrial == 0) { 
			audio.volume = 0.0F;
			audioVolume = 0.0F;	
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
		int controle;
		
		if(patternKey < nmbrSt){
			SendToArduino[0] = '7';
			controle = patternKey%2;
			SendToArduino[24] = char.Parse(controle.ToString());
		}else{
			Itr++;
			end = DateTime.Now;
			StartButtonTxt.guiText.text = "END";
			audio.Stop();
			LOGlogs = UserId + ";" + start + ";" + end + ";" + LOGlogs;
			Debug.Log ("<logIdTask> LOGlogsButt: " + LOGlogs);
			
			LOGTotals = UserId + ";";
			for (int i = 0; i < 4; i++) {
				LOGTotals = LOGTotals + Acertos[i] + ";" + ReacTim[i]/12 + ";";
			}
			Debug.Log ("<logIdTask> LOGTotalsButt: " + LOGTotals);
			
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
					TurnLedOn = _SerialPortBelt.ReadLine();
					if(TurnLedOn == "lighton"){
						Button = GameObject.Find(Answers[Direction[StimuliOrder[patternKey]]]);
						Button.renderer.material.color = Color.red;
					}
				}catch(TimeoutException ex){
					Debug.Log ("<logIdTask> ERROR: serial write" + ex);
				}
				LOGlogs = LOGlogs + Answers[Direction[StimuliOrder[patternKey]]] + ";";
				Debug.Log ("<logIdTask> Stimulus: " + Answers[Direction[StimuliOrder[patternKey]]]);
				break;
			case 2:	
				if(patternKey == nmbrSt-1){ 
					audio.volume = 0.6F;
					audioVolume = 0.6F;		
				}
				Button.renderer.material.color = Color.white;
				try{
					ComeFromArduino = _SerialPortBelt.ReadLine();
					string[] ComeFromArduinoCheck = ComeFromArduino.Split(':');
					if(ComeFromArduinoCheck.Length > 1){
						InputGetButt(ComeFromArduinoCheck[1]);
					}
				}catch(TimeoutException ex){
					Debug.Log ("<logIdTask> Timeout1" + ex);
					//contCicle = 19;
				}
				break;
			case 3:	
				try{
					ComeFromArduino = _SerialPortBelt.ReadLine();
					string[] ComeFromArduinoChecks = ComeFromArduino.Split(':');
					if(ComeFromArduinoChecks.Length > 1){
						InputGetButt(ComeFromArduinoChecks[1]);
					}
				}catch(TimeoutException ex){
					Debug.Log ("<logIdTask> Timeout2" + ex);
					contCicle = 19;
				}
				break;
			case 10:
				if(patternKey == nmbrSt-1){ 
					audio.volume = 0.3F; 
					audioVolume = 0.3F;		
				}
				CheckAnswer(StimuliOrder[patternKey]);
				TurnAllOptionsOff();
				TurnLedOn = "";
				patternKey++;
				contCicle = 0;

				break;				
			}

			if(contCicle == 0){
				timerDrawWave.Interval = RandomDelay[patternKey];
				timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
				timerDrawWave.Start();
			}else{
				timerDrawWave.Interval = 1000;
				timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
				timerDrawWave.Start();
			}
			//timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
			//timerDrawWave.Start();
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
		
		_SerialPortBelt.Close();
		portopenBelt = false;
		
		//print ("saiu");
	}
}