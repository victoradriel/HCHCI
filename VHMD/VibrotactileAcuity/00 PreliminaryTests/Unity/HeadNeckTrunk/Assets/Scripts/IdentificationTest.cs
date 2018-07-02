using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Threading;

public class IdentificationTest : MonoBehaviour {

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
	public static int nmbrSt = 20; // How many per site 20
	
	//GUI
	public GameObject BodySite;
	public GameObject StartButton;
	public GameObject StartButtonTxt;
	public static GameObject QuestionScreen;

	public static AudioSource audio;
	public float audioVolume = 0.0F;

	//Logs
	public string LOGlogs = "";
	public string LOGTotals = "";
	public DateTime start;
	public DateTime end;
	public string CorrAnswer; 
	public string TimeReactionMl = "";
	public static int[,] AcertosAmpF = new int[3,12] {{0,0,0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0,0,0}};
	public static int[,] AcertosAmpS = new int[3,12] {{0,0,0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0,0,0}};
	public static int[,] ReacTimAmpF = new int[3,12] {{0,0,0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0,0,0}};
	public static int[,] ReacTimAmpS = new int[3,12] {{0,0,0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0,0,0}};
	public static string[] Forms = new string[] {"https://docs.google.com/forms/d/10JDEOWmr0Yhrf7q4zMLDDi6FaCwYRuBySjffYW-Z5Ww/viewform",
												"https://docs.google.com/forms/d/1jQ60hpgaKeki-Z_OMe7_4PsxXq-nxd5iApKgJKxrRmk/viewform",
												"https://docs.google.com/forms/d/1KQRnPtCnaeH7BQj1RioijU130XblhkPjk-EG_ecBQfE/viewform",
												"https://docs.google.com/forms/d/18-mtrsGFJ_l4onqnXescGXsP7DzuBZMnG0NMrfUwaJ0/viewform",
												"https://docs.google.com/forms/d/1CD0IA2GNQut8Mp1jIJyIPTjerYrIXgyis6IUpm5PqDg/viewform",
												"https://docs.google.com/forms/d/1UdGfM9taUH3ooW1NeKljCA5P6fLuyuQOsIvO4dg_RHY/viewform",
												"https://docs.google.com/forms/d/1bz5ZpThzgGiCdAYAGtaSlJUQjk6RQBQ38gQYOVkfMEQ/viewform",
												"https://docs.google.com/forms/d/1ONfd8uiXgHKVqN08QNX_Du0sCOwQXVPdq2eoK3LLfXc/viewform",
												"https://docs.google.com/forms/d/1ep0e9ohtp-9PhBsN5KQJJOfit74T5QpQCVUXjocjvEQ/viewform",
												"https://docs.google.com/forms/d/1Q4e4yuiVoG9LThlkqlE7Q9YR5ZTPKkiFOnbKJXQI8fg/viewform",
												"https://docs.google.com/forms/d/1_veJcrtWZcbVvRSUTa9cFU61hxXGjMOVN_zraRlAQ1w/viewform",
												"https://docs.google.com/forms/d/1fkcYTzcbRW_eU9c6IRZXiR_BJSgh4ZGn_e1btUVQ9nY/viewform",
												"https://docs.google.com/forms/d/15RxDAzJumK3WdGFQN3t4plbsiXW8h-WKGrVfjp4bheg/viewform",
												"https://docs.google.com/forms/d/1T7un4eMhvb310qbZaOOtcYcYuVsYQbYScB6Eeq3NKJA/viewform",
												"https://docs.google.com/forms/d/1wxc_tkQQ1q6V0lhs6GpMXmT021je83B7pWPGrW3gIj8/viewform",
												"https://docs.google.com/forms/d/1uXL3kQDC66xcYreKyDvuyGwh_Ts5xi7PqkEzRsZQixk/viewform",
												"https://docs.google.com/forms/d/1hEwtaDGaGVJ5lSWuU3xheni4zlW-ZcD91HZxIP_0euM/viewform",
												"https://docs.google.com/forms/d/1oGtI6cbHLDfIgA-aW8JVT827l3_I_iTcm7YQWilar7U/viewform",
												"https://docs.google.com/forms/d/1inQHv0q383IKWtDakPxggCk-sTU8G5mXBaEZ04DE5YA/viewform"};

	void Start(){
		audio = GetComponent<AudioSource>();
		audio.Play();
		audio.volume = 0.0F;

		start = DateTime.Now;
		Debug.Log("<logIdTask> Start: " + start);
		UserId = PlayerPrefs.GetInt("Id");

		//LatinSquare BodySiteXAmplitude (6x6)
		if (UserId%6 == 0) UserIdLS = 5;
		else UserIdLS = (UserId%6) - 1;
		//LatinSquare Frequency (3x3)
		if (UserId%3 == 0) UserIdLSF = 2;
		else UserIdLSF = (UserId%3) - 1;

		Debug.Log("<logIdTask> User ID: " + UserId);
		Debug.Log("<logIdTask> LatinSquare (6x6): " + UserIdLS);
		Debug.Log("<logIdTask> LatinSquare (3x3): " + UserIdLSF);

		BodySite = GameObject.Find("BodySite");
		BodySite.guiText.text = BodyLoci[(LatinSquare[UserIdLS,Itr])-1];
		SetStimuliOrder(LatinSquare[UserIdLS,Itr]);

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
			if(SignalFreqAmpl == '1' || SignalFreqAmpl == '2' || SignalFreqAmpl == '3') AcertosAmpF[(LSquareFreq[UserIdLSF,ItrFq])-1,patternKey]++;
			if(SignalFreqAmpl == '4' || SignalFreqAmpl == '5' || SignalFreqAmpl == '6') AcertosAmpS[(LSquareFreq[UserIdLSF,ItrFq])-1,patternKey]++;
		}else{
			Debug.Log("<logIdTask> Wrong answer: " + bufferAnswer + ". It should be " + Answers[Direction[patternKey]]);
			CorrAnswer = "0";
		}
		LOGlogs = LOGlogs + CorrAnswer + ";" + TimeReactionMl + ";";

		if(TimeReactionMl != ""){
			if(SignalFreqAmpl == '1' || SignalFreqAmpl == '2' || SignalFreqAmpl == '3') ReacTimAmpF[(LSquareFreq[UserIdLSF,ItrFq])-1,patternKey]+= int.Parse(TimeReactionMl);
			if(SignalFreqAmpl == '4' || SignalFreqAmpl == '5' || SignalFreqAmpl == '6') ReacTimAmpS[(LSquareFreq[UserIdLSF,ItrFq])-1,patternKey]+= int.Parse(TimeReactionMl);
		}
	}

	
	void OnMouseDown() {
		if(ItrFq < 3){
			StartButton.renderer.material.color = Color.black;
			StartButtonTxt.guiText.text = "";
			GoTrial = 1;
		}else{
			Application.LoadLevel(5);
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
			GoTrial = 0;
			enableDraw = 0;
			patternKey = 0;
			contCicle = 0;
			Itr++;
			if(Itr < 6){
				SetStimuliOrder(LatinSquare[UserIdLS,Itr]);
				BodySite.guiText.text = BodyLoci[(LatinSquare[UserIdLS,Itr])-1];
				StartButtonTxt.guiText.text = "NEXT";
			}else{
				ItrFq++;
				if(ItrFq < 3){
					// again with new frequency
					Itr = 0;
					SetStimuliOrder(LatinSquare[UserIdLS,Itr]);
					BodySite.guiText.text = BodyLoci[(LatinSquare[UserIdLS,Itr])-1];
					StartButtonTxt.guiText.text = "BREAK";
				}else{
					BodySite.guiText.text = "";
					StartButtonTxt.guiText.text = "END";
				}
			}
			StartButton.renderer.material.color = Color.gray;


			//OnApplicationQuit();
			//Application.LoadLevel(5);
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

	void ResetTimer(object source, ElapsedEventArgs e){
		// Código a ser executado quando o intervalo for atingido
		contCicle = 0;
	}
	
	void Trial(){
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		//char[] SendToArduinoZero = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {10, 12, 14, 8, 18, 16, 6, 4, 20, 22, 2, 0};
		int controle;
		
		if(patternKey < nmbrSt){
			SendToArduino[SendToArduinoPosition[StimuliOrder[patternKey]]] = SignalFreqAmpl;
			controle = patternKey%2;
			SendToArduino[24] = char.Parse(controle.ToString());
		}else{
			GoTrial = 0;
			audio.volume = 0.0F;
			enableDraw = 0;
			patternKey = 0;
			Itr++;
			if(Itr < 6){
				SetStimuliOrder(LatinSquare[UserIdLS,Itr]);
				BodySite.guiText.text = BodyLoci[(LatinSquare[UserIdLS,Itr])-1];
				StartButtonTxt.guiText.text = "NEXT";
				Application.OpenURL(Forms[countform]);
				countform++;
			}else{
				ItrFq++;
				if(ItrFq < 3){
					// again with new frequency
					Itr = 0;
					SetStimuliOrder(LatinSquare[UserIdLS,Itr]);
					BodySite.guiText.text = BodyLoci[(LatinSquare[UserIdLS,Itr])-1];
					StartButtonTxt.guiText.text = "BREAK";
					Application.OpenURL(Forms[countform]);
					countform++;
				}else{
					end = DateTime.Now;
					BodySite.guiText.text = "";
					StartButtonTxt.guiText.text = "END";
					Application.OpenURL(Forms[countform]);
					LOGlogs = UserId + ";" + start + ";" + end + ";" + LOGlogs;
					Debug.Log ("<logIdTask> LOGlogs: " + LOGlogs);

					LOGTotals = UserId + ";";
					for(int i=0; i < 3; i++){
						for (int j = 0; j < 12; j++) LOGTotals = LOGTotals + AcertosAmpF[i,j] + ";" + ReacTimAmpF[i,j]/5 + ";";
						for (int j = 0; j < 12; j++) LOGTotals = LOGTotals + AcertosAmpS[i,j] + ";" + ReacTimAmpS[i,j]/5 + ";";
					}
					Debug.Log ("<logIdTask> LOGTotals: " + LOGTotals);
					
					OnApplicationQuit();
				}
			}
			StartButton.renderer.material.color = Color.gray;
			//_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
		}
		
		if(enableDraw == 1){			
			enableDraw = 0;
			contCicle++;
			switch(contCicle){
			case 1:	
				try{
					//clear
					//_SerialPortBelt.DiscardInBuffer();
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
				}catch{
					Debug.Log ("<logIdTask> ERROR: _SerialPortBelt.Write()");
				}
				LOGlogs = LOGlogs + BodyLoci[(LatinSquare[UserIdLS,Itr])-1] + "-" + Answers[Direction[StimuliOrder[patternKey]]] + "-" + SignalFreqAmpl + ";";
				Debug.Log ("<logIdTask> StimulusCode: " + BodyLoci[(LatinSquare[UserIdLS,Itr])-1] + "-" + Answers[Direction[StimuliOrder[patternKey]]] + "-" + SignalFreqAmpl);
				Debug.Log ("<logIdTask> StimulusArray: " + SendToArduino[0] + "," +SendToArduino[2] + "," +SendToArduino[4] + "," +SendToArduino[6] + "," +SendToArduino[8] + "," +SendToArduino[10] + "," +SendToArduino[12] + "," +SendToArduino[14] + "," +SendToArduino[16] + "," +SendToArduino[18] + "," +SendToArduino[20] + "," +SendToArduino[22]);
				QuestionScreen.SetActive (true);
				break;
			case 2:	
				if(patternKey == nmbrSt-1){ 
					audio.volume = 0.6F; 
					audioVolume = 0.6F; 
				}
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
				if(patternKey == nmbrSt-1){ 
					audio.volume = 0.3F; 
					audioVolume = 0.3F; 
				}
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
			audioVolume -= 0.5F * Time.deltaTime;
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
