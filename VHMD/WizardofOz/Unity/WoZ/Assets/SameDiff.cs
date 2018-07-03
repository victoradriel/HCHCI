using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Threading;

public class SameDiff : MonoBehaviour {
	// Serial - Belt
	public static string COMPortBelt = "COM8";
	public static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBelt = false; //if port is open or not
	public int cont = 0;
	public static int[] motor = new int[] {8,10,12,14,0,2,4,6,16,18,20,22};
	public static string[] answer = new string[]{"left","right"};
	public int answerPos = 0;
	public int stmref = 5;
	public int position = 0;
	public int randomdelay = 0;
	public int CanSend = 0;
	public int letra = 33;
	public int lastStim = 33;
	public int canread = 0;
	public int canwrite = 0;
	public int clear = 0;
	public int flush = 0;
	public GameObject txtdisplay;
	public static string ComeFromArduino = "";

	public static AudioSource PinkNoise;
	public float audioVolume = 0.0F;
	public int audioStop = 0;

	char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
	string[] PredefStimuli = new string[] {"F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F1 Z", "F1 X", "F1 C", "F11 T", "F11 R", "F11 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F1 Z", "F1 X", "F1 C", "F11 T", "F11 R", "F11 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F1 Z", "F1 X", "F1 C", "F11 T", "F11 R", "F11 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F1 Z", "F1 X", "F1 C", "F11 T", "F11 R", "F11 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F1 Z", "F1 X", "F1 C", "F11 T", "F11 R", "F11 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F1 Z", "F1 X", "F1 C", "F11 T", "F11 R", "F11 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F1 Z", "F1 X", "F1 C", "F11 T", "F11 R", "F11 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F1 Z", "F1 X", "F1 C", "F11 T", "F11 R", "F11 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F1 Z", "F1 X", "F1 C", "F11 T", "F11 R", "F11 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F1 Z", "F1 X", "F1 C", "F11 T", "F11 R", "F11 E"};
	string[] PredefStimuli45 = new string[] {"F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F2 Z", "F2 X", "F2 C", "F10 T", "F10 R", "F10 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F2 Z", "F2 X", "F2 C", "F10 T", "F10 R", "F10 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F2 Z", "F2 X", "F2 C", "F10 T", "F10 R", "F10 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F2 Z", "F2 X", "F2 C", "F10 T", "F10 R", "F10 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F2 Z", "F2 X", "F2 C", "F10 T", "F10 R", "F10 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F2 Z", "F2 X", "F2 C", "F10 T", "F10 R", "F10 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F2 Z", "F2 X", "F2 C", "F10 T", "F10 R", "F10 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F2 Z", "F2 X", "F2 C", "F10 T", "F10 R", "F10 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F2 Z", "F2 X", "F2 C", "F10 T", "F10 R", "F10 E", "F6 E", "F6 R", "F6 T", "F6 Z", "F6 X", "F6 C", "F2 Z", "F2 X", "F2 C", "F10 T", "F10 R", "F10 E"};
	//string[] PredefStimuli = new string[] {"F6 Q","F11 W","F6 E"};
	public int globalIndex = -1;
	public int automaticMode = 0;

	// Use this for initialization
	void Start () {
		OpenConnectionBelt();

		PinkNoise = GetComponent<AudioSource>();
		PinkNoise.Play();
		PinkNoise.volume = 0.0F;

		txtdisplay = GameObject.Find ("Display");
	}

	public static string[] MyFisherYatesShfflSt(string[] StArray){
		//Fisher–Yates Shuffle variation
		int ArrayLength = StArray.Length;
		for (int i = 0; i < ArrayLength; i++) {
			string temp = StArray[i];
			int randomIndex = UnityEngine.Random.Range(i, ArrayLength-1);
			if((i > 0)&&(i < ArrayLength-2)){
				if(StArray[i-1] == StArray[randomIndex])
					randomIndex++;
			}
			StArray[i] = StArray[randomIndex];
			StArray[randomIndex] = temp;
		}
		return StArray;
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
				_SerialPortBelt.ReadTimeout = 3000;
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
	void PinkNoiseControl(){
		if (audioStop == 0) fadeIn ();
		else fadeOut ();

		if(Input.GetKey(KeyCode.DownArrow)){
			audioStop = 1;
		}

		if(Input.GetKey(KeyCode.UpArrow)){
			audioStop = 0;
		}

	}
	
	// Update is called once per frame
	void Update () {
		UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;

		// Pink noise starts on. Press Up to fade it in. Press Down to fade it out. 
		PinkNoiseControl();

		if (automaticMode == 1) {
			// Press Left and Right Arrows to trigger predefined stimuli
			if(lastStim == 33){ GetStimuli (); }
		}else{
			// Press QWERT A ZXCVB to provide deltas from -5 to 5. Only possible if the answer was processed
			if(lastStim == 33){ GetDelta(); }
			// Press F1 to F12 to change refference
			GetReff();
		}

		// If a delta was provided, prepare the string to send to Arduino
		if (CanSend == 1){ 
			MakeInput (); 
			CanSend = 0; 
		}

		cont++;
		if(cont > 150){
			cont = 0;
			// After a while, if I already did a flush on the Serial port, I can do it again
			if(flush == 1){ flush = 0; }

			// If there is something to write
			if(canwrite == 1 && portopenBelt && _SerialPortBelt.IsOpen){
				try{
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					zeraString();
					print ("<headJNDlog> STATUS Enviado ao Arduino");
					invertIfDeltaisNegative();
					print ("<headJNDlog> StmRefPos: " + (stmref+1) + ", Delta: " + (letra - 5) + ", Position: " + (position - 48) + ", RightAnswer: " + answer[answerPos] + ", DelaySwitch: " + (randomdelay - 48) + ", Time: " + DateTime.Now.ToString("hh:mm:ss"));
					txtdisplay.GetComponent<GUIText>().text = " ";
					canwrite = 0;

					if(clear == 0){ canread = 1; }
					else{ clear = 0; lastStim = 33; }
				}catch{
					print ("<headJNDlog> STATUS Erro ao tentar enviar ao Arduino");
					txtdisplay.GetComponent<GUIText>().text = "Without Communication";
					canwrite = 0;
				}
			
			// If there is something to read
			}else if(canread == 1 && portopenBelt && _SerialPortBelt.IsOpen){
				try{
					ComeFromArduino = _SerialPortBelt.ReadLine();
					InputGetButt(ComeFromArduino);
					canread = 0;
				}catch{
					LogAndDisplay ("Timeout: Answer time is over.");
					print ("<headJNDlog> Answer: NONE, ReactionTime: NONE, Time: NONE");
					print ("<headJNDlog> GotRight: NONE");
					if(automaticMode == 1){ globalIndex--; }
					lastStim = 33;
					canread = 0;
				}
			}
		}

		// To press space bar before the stimuli desables the reading
		if(Input.GetKey(KeyCode.Space)){
			//if(clear == 0){
				//clear = 1;
				LogAndDisplay ("Invalidate Answer");
			//}
		}

		// To press F reads the Serial port to flush any unwanted answer
		if(Input.GetKey(KeyCode.F)){
			if(flush == 0){
				flush = 1;
				FlushSerial();
			}
		}

		// To press S enables automatic mode
		if(Input.GetKey(KeyCode.S)){
			if(automaticMode == 0){
				automaticMode = 1;
				globalIndex = -1;
				LogAndDisplay ("AutomaticMode: On");
				PredefStimuli = MyFisherYatesShfflSt(PredefStimuli);
			}
		}

		// To press D enables automatic mode for 45º
		if(Input.GetKey(KeyCode.D)){
			if(automaticMode == 0){
				automaticMode = 1;
				globalIndex = -1;
				LogAndDisplay ("AutomaticMode: On - 45 degrees");
				PredefStimuli = MyFisherYatesShfflSt(PredefStimuli45);
			}
		}

		// To press S enables manual mode
		if(Input.GetKey(KeyCode.M)){
			if(automaticMode == 1){
				automaticMode = 0;
				LogAndDisplay ("AutomaticMode: Off");
			}
		}
	}

	void FlushSerial (){
		try{
			ComeFromArduino = _SerialPortBelt.ReadLine();
			LogAndDisplay("Flushed. " + ComeFromArduino);
		}catch{
			//LogAndDisplay ("Timeout1" + ex);
			LogAndDisplay ("Timeout: Nothing to flush.");
		}
	}

	void InputGetButt(string entrada){
		string[] ComeFromArduinoCheck = entrada.Split(' ');
		if(ComeFromArduinoCheck.Length > 1){
			AnswerArduino(ComeFromArduinoCheck[0], int.Parse(ComeFromArduinoCheck[1]));
		}
	}

	void invertIfDeltaisNegative() {
		if((letra - 5) < 0){
			if(position == 49){ answerPos = 1; }
			else if(position == 50){ answerPos = 0; }
		}else{
			answerPos = position - 49;
		}
	}

	void zeraString() {
		int i = 0;
		for(i = 0; i < 12; i++){
			SendToArduino[motor[i]] = '0';
		}
	}

	void AnswerArduino (string answerArd, int reactionTime) {
		int delta = letra - 5;
		LogAndDisplay ("Answer " + answerArd);
		if(answerArd == "left"){
			if(lastStim != 33){
				print ("<headJNDlog> Answer: " + answer[0] + ", ReactionTime: " + reactionTime + ", Time: " + DateTime.Now.ToString("hh:mm:ss"));
				if((lastStim == 0 && delta > 0) || (lastStim == 1 && delta < 0)){
					LogAndDisplay("GotRight: 1");
					print ("<headJNDlog> GotRight: 1");
				}else{
					LogAndDisplay("GotRight: 0");
					print ("<headJNDlog> GotRight: 0");
				}
				lastStim = 33;
			}
		}else if(answerArd == "right"){
			if(lastStim != 33){
				print ("<headJNDlog> Answer: " + answer[1] + ", ReactionTime: " + reactionTime + ", Time: " + DateTime.Now.ToString("hh:mm:ss"));
				if((lastStim == 0 && delta < 0) || (lastStim == 1 && delta > 0)){
					LogAndDisplay("GotRight: 1");
					print ("<headJNDlog> GotRight: 1");
				}else{
					LogAndDisplay("GotRight: 0");
					print ("<headJNDlog> GotRight: 0");
				}
				lastStim = 33;
			}
		}else{
			LogAndDisplay("Something wrong with answer coming from Arduino");
		}
	}

	void MakeInput () {
		// Set delta
		int delta = letra - 5;
		position = UnityEngine.Random.Range(49, 51);
		if(delta == 0 || Mathf.Abs(delta) == 5) position = 49; // fixa a posiçao da referencia para auxiliar na correçao
		SendToArduino[motor[stmref + delta]] = (char)position;
		
		// random interval
		randomdelay = UnityEngine.Random.Range(49, 52);
		SendToArduino[24] = (char)randomdelay;
		
		lastStim = position-49; // auxiliar para conferir resposta
		
		// Set reff
		SendToArduino[motor[stmref]] = '5';
		

		canwrite = 1;
	}

	void GetStimuli () {

		if(Input.GetKey(KeyCode.LeftArrow)){
			if(globalIndex > 0){
				//globalIndex--;
				GetDeltaAutomatic ();
			}
		}

		if(Input.GetKey(KeyCode.RightArrow)){
			if(globalIndex < (PredefStimuli.Length)-1){ 
				globalIndex++;
				GetDeltaAutomatic ();
			}
		}
	}

	void GetDelta () {
		if(Input.GetKey(KeyCode.Q)){
			if(stmref > 4){
				letra = 0;
				CanSend = 1;
			}
		}
		if(Input.GetKey(KeyCode.W)){
			if(stmref > 3){
				letra = 1;
				CanSend = 1;
			}
		}
		if(Input.GetKey(KeyCode.E)){
			if(stmref > 2){
				letra = 2;
				CanSend = 1;
			}
		}
		if(Input.GetKey(KeyCode.R)){
			if(stmref > 1){
				letra = 3;
				CanSend = 1;
			}
		}
		if(Input.GetKey(KeyCode.T)){
			if(stmref > 0){
				letra = 4;
				CanSend = 1;
			}
		}
		if(Input.GetKey(KeyCode.A)){
			letra = 5;
			CanSend = 1;
		}
		if(Input.GetKey(KeyCode.Z)){
			if(stmref < 11){
				letra = 6;
				CanSend = 1;
			}
		}
		if(Input.GetKey(KeyCode.X)){
			if(stmref < 10){
				letra = 7;
				CanSend = 1;
			}
		}
		if(Input.GetKey(KeyCode.C)){
			if(stmref < 9){
				letra = 8;
				CanSend = 1;
			}
		}
		if(Input.GetKey(KeyCode.V)){
			if(stmref < 8){
				letra = 9;
				CanSend = 1;
			}
		}
		if(Input.GetKey(KeyCode.B)){
			if(stmref < 7){
				letra = 10;
				CanSend = 1;
			}
		}
	}

	void LogAndDisplay(string text){
		print ("<headJNDlog> STATUS " + text);
		txtdisplay.GetComponent<GUIText>().text = text;

	}

	void GetReff () {
		if(Input.GetKey(KeyCode.F1)){
			stmref = 0;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F2)){
			stmref = 1;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F3)){
			stmref = 2;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F4)){
			stmref = 3;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F5)){
			stmref = 4;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F6)){
			stmref = 5;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F7)){
			stmref = 6;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F8)){
			stmref = 7;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F9)){
			stmref = 8;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F10)){
			stmref = 9;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F11)){
			stmref = 10;
			LogAndDisplay("StmReff setted as " + stmref);
		}
		if(Input.GetKey(KeyCode.F12)){
			stmref = 11;
			LogAndDisplay("StmReff setted as " + stmref);
		}
	}

	void fadeIn() {
		if (audioVolume < 1.0F) {
			audioVolume += 0.2F * Time.deltaTime;
			PinkNoise.volume = audioVolume;
		}
	}
	
	void fadeOut() {
		if(audioVolume > 0.0F){
			audioVolume -= 0.5F * Time.deltaTime;
			PinkNoise.volume = audioVolume;
		}
	}

	void GetDeltaAutomatic () {
		string[] quebraStm = PredefStimuli[globalIndex].Split(' '); //make it random with N
		LogAndDisplay ("Stimuli: " + (globalIndex+1));

		switch(quebraStm[0]) {
			case "F1":
				stmref = 0;
				break;
			case "F2":
				stmref = 1;
				break;
			case "F3":
				stmref = 2;
				break;
			case "F4":
				stmref = 3;
				break;
			case "F5":
				stmref = 4;
				break;
			case "F6":
				stmref = 5;
				break;
			case "F7":
				stmref = 6;
				break;
			case "F8":
				stmref = 7;
				break;
			case "F9":
				stmref = 8;
				break;
			case "F10":
				stmref = 9;
				break;
			case "F11":
				stmref = 10;
				break;
			case "F12":
				stmref = 11;
				break;
		}
		
		switch(quebraStm[1]){
			case "Q":
				letra = 0;
				CanSend = 1;
				break;
			case "W":
				letra = 1;
				CanSend = 1;
				break;
			case "E":
				letra = 2;
				CanSend = 1;
				break;
			case "R":
				letra = 3;
				CanSend = 1;
				break;
			case "T":
				letra = 4;
				CanSend = 1;
				break;
			case "A":
				letra = 5;
				CanSend = 1;
				break;
			case "Z":	
				letra = 6;
				CanSend = 1;
				break;
			case "X":
				letra = 7;
				CanSend = 1;
				break;
			case "C":
				letra = 8;
				CanSend = 1;
				break;
			case "V":
				letra = 9;
				CanSend = 1;
				break;
			case "B":
				letra = 10;
				CanSend = 1;
				break;
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
