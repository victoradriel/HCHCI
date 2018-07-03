using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Diagnostics;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Rhythm : MonoBehaviour {
    public bool[] enableDraw = new bool [5];
    public System.Timers.Timer[] timerDrawWave = new System.Timers.Timer[5];
    bool trialState = false;
    string [] conditions = new string [] { "200", "400", "600", "800", "1000" };
    string [] trials = new string [] { "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000", "200", "400", "600", "800", "1000"};
    int [] conditionsint = new int [] { 200, 400, 600, 800, 1000 };
    int trialID = 0;
    public string thistrial = "";
    GameObject ball;
    bool redf = false;
    int motorstatus = 0;
    int touchcount = 0;
    public float frstxstr = 0;
    public float frstxend = 0;
    public long frsttime = 0;
    public long frsttimedur = 0;
    public float secxstr = 0;
    public float secxend = 0;
    public long sectime = 0;
    public long sectimedur = 0;
    int answerIDinterburst = 0;
    int answerIDburst1 = 0;
    int answerIDburst2 = 0;
    StreamWriter logFile;
    bool treinoon = true;
    Text feedback;
    Text display;

	// Use this for initialization
	void Start () {
        logFile = File.CreateText(Application.persistentDataPath + "/logAFRhythm-" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv");
        UnityEngine.Debug.Log("<logAFRhythm> <sys> StartTime;" + System.DateTime.Now.ToString("h:mm:ss tt"));
        logFile.WriteLine("<logAFRhythm> <sys> StartTime;" + System.DateTime.Now.ToString("h:mm:ss tt"));

        feedback = GameObject.Find("feedback").GetComponent<Text>();
        display = GameObject.Find("display").GetComponent<Text>();

        ball = GameObject.Find("P01");

        for (int i = 0; i < conditions.Length; i++) { timerDrawWave [i] = new System.Timers.Timer(conditionsint[i]); }
        timerDrawWave[0].Elapsed += new ElapsedEventHandler(EnableDraw1);
        timerDrawWave[1].Elapsed += new ElapsedEventHandler(EnableDraw2);
        timerDrawWave[2].Elapsed += new ElapsedEventHandler(EnableDraw3);
        timerDrawWave[3].Elapsed += new ElapsedEventHandler(EnableDraw4);
        timerDrawWave[4].Elapsed += new ElapsedEventHandler(EnableDraw5);
        for (int i = 0; i < conditions.Length; i++) { timerDrawWave[i].Start();  }

        trials = MyFisherYatesShffl(trials);
	}
	
	void FixedUpdate () {
        // If trialstate is true search for the actual condition and check if it is enabled to display the signal
        if (trialState) {
            for (int i = 0; i < enableDraw.Length; i++) {
                if (thistrial == conditions [i] && enableDraw [i]) {
                    enableDraw [i] = false;

                    if (motorstatus == 0) { motorstatus++; }
                    else if (motorstatus == 1 || motorstatus == 3) { ball.GetComponent<Renderer>().material.color = Color.red; motorstatus++; }
                    else if (motorstatus == 2 || motorstatus == 4) { ball.GetComponent<Renderer>().material.color = Color.white; motorstatus++; }
                }
            }
        }        
	}

    void Update( ) {

        if (Input.GetKey(KeyCode.S) && treinoon) {
            treinoon = false;
            feedback.text = "";
        }

        if (Input.GetKey(KeyCode.Space) && !trialState) {
            if (trialID < trials.Length) {
                trialState = true;
                touchcount = 0;
                motorstatus = 0;
                thistrial = trials [trialID];
                UnityEngine.Debug.Log("<logAFRhythm> <sys> <display>;" + trialID +";"+ thistrial);
                logFile.WriteLine("<logAFRhythm> <sys> <display>;" + trialID +";"+ thistrial);
                display.text = "Stm #" + (trialID+1);
                trialID++;
                if (treinoon) { feedback.text = ""; }
            } else {
                UnityEngine.Debug.Log("<logAFRhythm> <sys> EndSession;" + System.DateTime.Now.ToString("h:mm:ss tt"));
                logFile.WriteLine("<logAFRhythm> <sys> EndSession;" + System.DateTime.Now.ToString("h:mm:ss tt"));
                display.text = "End of Session";
            }
        }

        if (MainController.control.sentSmthng && trialState && touchcount == 0) {
            touchcount++;
            MainController.control.sentSmthng = false;
            display.text = " ";

            frstxstr = MainController.control.xstrt;
            frstxend = MainController.control.xend;
            frsttime = MainController.control.rtbtDouble;
            frsttimedur = MainController.control.rtbtDur;
        }else if (MainController.control.sentSmthng && trialState && touchcount == 1) {
            trialState = false;
            touchcount++;
            MainController.control.sentSmthng = false;

            secxstr = MainController.control.xstrt;
            secxend = MainController.control.xend;
            sectime = MainController.control.rtbtDouble;
            sectimedur = MainController.control.rtbtDur;
            GetAnswer(frsttimedur,sectime,sectimedur);    
        }

        if (Input.GetKey(KeyCode.Escape)) {
            Leave();
            SceneManager.LoadScene(0);
        } 
    }

    public void GetAnswer(long thisburst1, long thisinterburst, long thisburst2) {
        // get index
        if (thisburst1 >= 100 && thisburst1 < 300) answerIDburst1 = 1;
        else if (thisburst1 >= 300 && thisburst1 < 500) answerIDburst1 = 2;
        else if (thisburst1 >= 500 && thisburst1 < 700) answerIDburst1 = 3;
        else if (thisburst1 >= 700 && thisburst1 < 900) answerIDburst1 = 4;
        else if (thisburst1 >= 900 && thisburst1 < 1100) answerIDburst1 = 5;
        else answerIDburst1 = 33;

        if (thisinterburst >= 100 && thisinterburst < 300) answerIDinterburst = 1;
        else if (thisinterburst >= 300 && thisinterburst < 500) answerIDinterburst = 2;
        else if (thisinterburst >= 500 && thisinterburst < 700) answerIDinterburst = 3;
        else if (thisinterburst >= 700 && thisinterburst < 900) answerIDinterburst = 4;
        else if (thisinterburst >= 900 && thisinterburst < 1100) answerIDinterburst = 5;
        else answerIDinterburst = 33;

        if (thisburst2 >= 100 && thisburst2 < 300) answerIDburst2 = 1;
        else if (thisburst2 >= 300 && thisburst2 < 500) answerIDburst2 = 2;
        else if (thisburst2 >= 500 && thisburst2 < 700) answerIDburst2 = 3;
        else if (thisburst2 >= 700 && thisburst2 < 900) answerIDburst2 = 4;
        else if (thisburst2 >= 900 && thisburst2 < 1100) answerIDburst2 = 5;
        else answerIDburst2 = 33;

        string thisburst1final = "";
        string thisinterburstfinal = "";
        string thisburst2final = "";
        
        if (answerIDburst1 == 33) { thisburst1final = "0;outrange"; }
        else if (thistrial == conditions [answerIDburst1 - 1]) { thisburst1final = "1;"+conditions [answerIDburst1 - 1]; }
        else { thisburst1final = "0;"+conditions [answerIDburst1 - 1]; }

        if (answerIDinterburst == 33) { thisinterburstfinal = "0;outrange"; }
        else if (thistrial == conditions [answerIDinterburst - 1]) { thisinterburstfinal = "1;"+conditions [answerIDinterburst - 1]; }
        else { thisinterburstfinal = "0;"+conditions [answerIDinterburst - 1]; }

        if (answerIDburst2 == 33) { thisburst2final = "0;outrange"; }
        else if (thistrial == conditions [answerIDburst2 - 1]) { thisburst2final = "1;"+conditions [answerIDburst2 - 1]; }
        else { thisburst2final = "0;"+conditions [answerIDburst2 - 1]; }

        // get answer Stm;(0/1;StmAnswer);Raw;(0/1;StmAnswer);Raw;(0/1;StmAnswer);Raw
        UnityEngine.Debug.Log("<logAFRhythm> <answer>;" + thistrial + ";" + thisburst1final + ";" + thisburst1 + ";" + thisinterburstfinal + ";" + thisinterburst + ";" + thisburst2final + ";" + thisburst2 );
        logFile.WriteLine("<logAFRhythm> <answer>;" + thistrial + ";" + thisburst1final + ";" + thisburst1 + ";" + thisinterburstfinal + ";" + thisinterburst + ";" + thisburst2final + ";" + thisburst2 );

        if (treinoon) { feedback.text = "Stm: " + thistrial + " Ans: " + thisburst1 + "-" + thisinterburst + "-" + thisburst2; }
    }

    // Callback functions for each rhythm condition
    void EnableDraw1(object source, ElapsedEventArgs e){ enableDraw[0] = true;  }
    void EnableDraw2(object source, ElapsedEventArgs e){ enableDraw[1] = true;  }
    void EnableDraw3(object source, ElapsedEventArgs e){ enableDraw[2] = true;  }
    void EnableDraw4(object source, ElapsedEventArgs e){ enableDraw[3] = true;  }
    void EnableDraw5(object source, ElapsedEventArgs e){ enableDraw[4] = true;  }

    void Leave( ) {
        UnityEngine.Debug.Log("<logAFTap> <sys> EndSession;" + System.DateTime.Now.ToString("h:mm:ss tt"));
        logFile.WriteLine("<logAFTap> <sys> EndSession;" + System.DateTime.Now.ToString("h:mm:ss tt"));
        for (int i = 0; i < conditions.Length; i++) { timerDrawWave[i].Stop();  }        
        logFile.Close();
    }

    public void OnApplicationQuit() {
        Leave();        
	}

    public static string[] MyFisherYatesShffl(string[] StrArray) {
        //Fisher–Yates Shuffle variation
        int ArrayLength = StrArray.Length;

        for (int i = 0; i < ArrayLength; i++) {
            string temp = StrArray[i];
            int randomIndex = UnityEngine.Random.Range(i, ArrayLength - 1);
            if ((i > 0) && (i < ArrayLength - 2)) {
                if (StrArray[i - 1] == StrArray[randomIndex])
                    randomIndex++;
            }
            StrArray[i] = StrArray[randomIndex];
            StrArray[randomIndex] = temp;
        }
        return StrArray;
    }
}
