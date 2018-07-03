using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Diagnostics;

public class secRhythm : MonoBehaviour {
    public bool[] enableDraw = new bool [5];
    public System.Timers.Timer[] timerDrawWave = new System.Timers.Timer[5];
    bool trialState = false;

    Stopwatch stopwatch;
    long elapsed_time = 0;

    //string [] conditions = new string [] { "250", "500", "750", "1000", "1250" };
    string [] conditions = new string [] { "200", "400", "600", "800", "1000" };
    string [] trials = new string [] { "200", "400", "600", "800", "1000" };
    int [] conditionsint = new int [] { 200, 400, 600, 800, 1000 };
    int trialID = 0;
    string thistrial = "";
    GameObject ball;
    bool redf = false;
    int motorstatus = 0;

	// Use this for initialization
	void Start () {
        ball = GameObject.Find("P01");

        for (int i = 0; i < conditions.Length; i++) { timerDrawWave [i] = new System.Timers.Timer(conditionsint[i]); }
        timerDrawWave[0].Elapsed += new ElapsedEventHandler(EnableDraw1);
        timerDrawWave[1].Elapsed += new ElapsedEventHandler(EnableDraw2);
        timerDrawWave[2].Elapsed += new ElapsedEventHandler(EnableDraw3);
        timerDrawWave[3].Elapsed += new ElapsedEventHandler(EnableDraw4);
        timerDrawWave[4].Elapsed += new ElapsedEventHandler(EnableDraw5);
        for (int i = 0; i < conditions.Length; i++) { timerDrawWave[i].Start();  }

		stopwatch = new Stopwatch();
        stopwatch.Start(); 
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
        if (Input.GetKey(KeyCode.Space) && trialID < trials.Length && !trialState) {
            trialState = true;
            motorstatus = 0;
            thistrial = trials [trialID];
            trialID++;
        }

        if (Input.GetKey(KeyCode.KeypadEnter) && trialState) {
            trialState = false;
            //MainController.control.sentSmthng = false;

            /*xstr = MainController.control.xstrt;
            xend = MainController.control.xend;
            GetAnswer(xstr,xend);    
                   
            ClearAll();    */ 
        }
    }

    // Callback functions for each rhythm condition
    void EnableDraw1(object source, ElapsedEventArgs e){ enableDraw[0] = true;  }
    void EnableDraw2(object source, ElapsedEventArgs e){ enableDraw[1] = true;  }
    void EnableDraw3(object source, ElapsedEventArgs e){ enableDraw[2] = true;  }
    void EnableDraw4(object source, ElapsedEventArgs e){ enableDraw[3] = true;  }
    void EnableDraw5(object source, ElapsedEventArgs e){ enableDraw[4] = true;  }

    public void OnApplicationQuit() {
        for (int i = 0; i < conditions.Length; i++) { timerDrawWave[i].Stop();  }
	}
}
