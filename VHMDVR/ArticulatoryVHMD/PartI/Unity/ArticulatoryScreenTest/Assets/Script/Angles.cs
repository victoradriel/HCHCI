using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Angles : MonoBehaviour {
    public int enableDraw = 0;
    public System.Timers.Timer timerDrawWave;

    GameObject [] target = new GameObject [7];
    string [] balls = new string [] { "P01", "P02", "P03", "P04", "P05", "P06", "P07" };
    int [] condcount = new int [] { 12, 12, 12, 12, 12, 12 };
    string [] conditions = new string [] { "15", "30", "45", "60", "75", "90" };
    string [] trials = new string [] { "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90", "15", "30", "45", "60", "75", "90" };
    string [] t15 = new string [] { "P01-P02", "P02-P03", "P03-P04", "P04-P05", "P05-P06", "P06-P07", "P02-P01", "P03-P02", "P04-P03", "P05-P04", "P06-P05", "P07-P06" };
    string [] t30 = new string [] { "P01-P03", "P02-P04", "P03-P05", "P04-P06", "P05-P07", "P03-P01", "P04-P02", "P05-P03", "P06-P04", "P07-P05", "P04-P06", "P02-P04" };
    string [] t45 = new string [] { "P01-P04", "P02-P05", "P03-P06", "P04-P07", "P04-P01", "P05-P02", "P06-P03", "P07-P04", "P01-P04", "P04-P01", "P04-P07", "P07-P04" };
    string [] t60 = new string [] { "P01-P05", "P02-P06", "P03-P07", "P05-P01", "P06-P02", "P07-P03", "P01-P05", "P02-P06", "P03-P07", "P05-P01", "P06-P02", "P07-P03" };
    string [] t75 = new string [] { "P01-P06", "P06-P01", "P01-P06", "P06-P01", "P01-P06", "P06-P01", "P01-P06", "P06-P01", "P01-P06", "P06-P01", "P01-P06", "P06-P01" };
    string [] t90 = new string [] { "P01-P07", "P07-P01", "P01-P07", "P07-P01", "P01-P07", "P07-P01", "P01-P07", "P07-P01", "P01-P07", "P07-P01", "P01-P07", "P07-P01" };

    public int trialID = 0;
    bool trialState = false;
    public string direction = "";
    public string targetfrom = "";
    public string targetto = "";
    public int targetfromid = 0;
    public int targettoid = 0;
    int res = 2560; // GS6
    public float xstr = 0;
    public float xend = 0;
    int answerIDstr = 0;
    int answerIDend = 0;
    int answerangle = 0;
    float delta = 0;
    public float answering = 0;
    int globali = 0;
    StreamWriter logFile;
    bool treinoon = true;
    Text feedback;
    Text display;

    // Use this for initialization
    void Start( ) {
        logFile = File.CreateText(Application.persistentDataPath + "/logAFAngles-" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv");
        UnityEngine.Debug.Log("<logAFAngles> <sys> StartTime;" + System.DateTime.Now.ToString("h:mm:ss tt"));
        logFile.WriteLine("<logAFAngles> <sys> StartTime;" + System.DateTime.Now.ToString("h:mm:ss tt"));

        feedback = GameObject.Find("feedback").GetComponent<Text>();
        display = GameObject.Find("display").GetComponent<Text>();

        for (int i = 0; i < balls.Length; i++) { target [i] = GameObject.Find(balls [i]); }
        delta = res / balls.Length;

        trials = MyFisherYatesShffl(trials);
        t15 = MyFisherYatesShffl(t15);
        t30 = MyFisherYatesShffl(t30);
        t45 = MyFisherYatesShffl(t45);
        t60 = MyFisherYatesShffl(t60);
        t75 = MyFisherYatesShffl(t75);
        t90 = MyFisherYatesShffl(t90);

        timerDrawWave = new System.Timers.Timer(300);
        timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
		timerDrawWave.Start();
    }

    void FixedUpdate( ) {
        if (trialState && enableDraw == 1 && globali != 0) {
            enableDraw = 0;

            if (direction == "right" && globali <= targettoid) { target [globali - 1].GetComponent<Renderer>().material.color = Color.red; globali++; }
            else if (direction == "left" && globali >= targettoid) { target [globali - 1].GetComponent<Renderer>().material.color = Color.red; globali--; }              	
        }
    }

    // Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.S) && treinoon) {
            treinoon = false;
            feedback.text = "";
        }        

        if (Input.GetKey(KeyCode.Space) && !trialState) {
            if (trialID < trials.Length) {
                trialState = true;
                string pos = GetPosition(trials [trialID]);
                DisplayStm(pos);
                UnityEngine.Debug.Log("<logAFAngles> <sys> <display>;" + trialID + ";" + trials [trialID] + ";" + pos);
                logFile.WriteLine("<logAFAngles> <sys> <display>;" + trialID + ";" + trials [trialID] + ";" + pos);
                display.text = "Stm #" + (trialID+1);
                trialID++;
                if (treinoon) { feedback.text = ""; }
            }
            else {
                UnityEngine.Debug.Log("<logAFAngles> <sys> EndSession;" + System.DateTime.Now.ToString("h:mm:ss tt"));
                logFile.WriteLine("<logAFAngles> <sys> EndSession;" + System.DateTime.Now.ToString("h:mm:ss tt"));
                display.text = "End of Session";
            }
        }

        if (MainController.control.sentSmthng && trialState) {
            trialState = false;
            MainController.control.sentSmthng = false;
            display.text = " ";

            xstr = MainController.control.xstrt;
            xend = MainController.control.xend;
            GetAnswer(xstr,xend);    
                   
            ClearAll();     
        }

        if (Input.GetKey(KeyCode.Escape)) {
            Leave();
            SceneManager.LoadScene(0);
        } 
	}

    public void GetAnswer(float thisxstr, float thisxend) {
        string answeranglefinal = "";
        string answerpressfinal = "";
        string answerreleasefinal = "";
        float thisangle = thisxend - thisxstr;
        if (thisangle < 0) thisangle = thisangle * (-1);
        thisangle = (90 * thisangle) / res;

        if (thisxstr % delta > 0) answerIDstr = (int) (thisxstr / delta + 1); 
        else answerIDstr = (int) (thisxstr / delta);
        if (answerIDstr < 1) answerIDstr = 1;
        if (answerIDstr > balls.Length) answerIDstr = balls.Length;
        if (targetfromid == answerIDstr) answerpressfinal = "1;" + answerIDstr; //print("RIGHT " + answerIDstr);
        else answerpressfinal = "0;" + answerIDstr; //print("WRONG " + answerIDstr);

        if (thisxend % delta > 0) answerIDend = (int) (thisxend / delta + 1); 
        else answerIDend = (int) (thisxend / delta);
        if (answerIDend < 1) answerIDend = 1;
        if (answerIDend > balls.Length) answerIDend = balls.Length;
        if (targettoid == answerIDend) answerreleasefinal = "1;" + answerIDend; //print("RIGHT " + answerIDend);
        else answerreleasefinal = "0;" + answerIDend; //print("WRONG " + answerIDend);

        answerangle = (int) (answerIDend - answerIDstr);
        if (answerangle < 0) answerangle = answerangle * (-1);
        if (answerangle < 1) answerangle = 1;
        if (answerangle > conditions.Length) answerangle = conditions.Length;
        if(conditions[answerangle-1] == trials[trialID-1]) answeranglefinal = "1;" + conditions[answerangle-1]; //print("RIGHT " + (answerangle-1));
        else answeranglefinal = "0;" + conditions[answerangle-1]; //print("WRONG " + (answerangle-1));

        // get answer Stm;direction;(0/1;StmAnswer);Raw;(0/1;StmAnswer);Raw;(0/1;StmAnswer);Raw
        UnityEngine.Debug.Log("<logAFAngles> <answer>;"+ trials[trialID-1] + ";" + direction + ";" + answerpressfinal + ";" + (thisxstr / delta) + ";" + answerreleasefinal + ";" + (thisxend / delta) + ";" + answeranglefinal + ";" + thisangle );
        logFile.WriteLine("<logAFAngles> <answer>;"+ trials[trialID-1] + ";" + direction + ";" + answerpressfinal + ";" + (thisxstr / delta) + ";" + answerreleasefinal + ";" + (thisxend / delta) + ";" + answeranglefinal + ";" + thisangle );

        if (treinoon) { feedback.text = "Stm: " + trials[trialID-1] + " Ans: " + thisangle; }
    }

    void ClearAll( ) {
        for (int i = 0; i < balls.Length; i++) {
            target[i].GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void SetTrial(string thispos) {
        string [] thisposarray = thispos.Split('-');
        targetfrom = thisposarray[0];
        targetto = thisposarray [1];

        string [] targetfromarray = targetfrom.Split('P');
        targetfromid = System.Int32.Parse(targetfromarray[1]);
        string [] targettoarray = targetto.Split('P');
        targettoid = System.Int32.Parse(targettoarray[1]);

        if (targetfromid < targettoid) { direction = "right"; }
        else { direction = "left"; }
    }

    public void DisplayStm(string thistgt) {
        SetTrial(thistgt);
        globali = targetfromid;

        
    }

    public string GetPosition(string thistrial) {
        switch (thistrial) {
            case "15":
                if (condcount[0] != 0) {
                    condcount[0] = condcount[0] - 1;
                    return (t15[condcount[0]]);
                }
                break;
            case "30":
                if (condcount[1] != 0) {
                    condcount[1] = condcount[1] - 1;
                    return (t30[condcount[1]]);
                }
                break;
            case "45":
                if (condcount[2] != 0) {
                    condcount[2] = condcount[2] - 1;
                    return (t45[condcount[2]]);
                }
                break;
            case "60":
                if (condcount[3] != 0) {
                    condcount[3] = condcount[3] - 1;
                    return (t60[condcount[3]]);
                }
                break;
            case "75":
                if (condcount[4] != 0) {
                    condcount[4] = condcount[4] - 1;
                    return (t75[condcount[4]]);
                }
                break;
            case "90":
                if (condcount[5] != 0) {
                    condcount[5] = condcount[5] - 1;
                    return (t90[condcount[5]]);
                }
                break;
        }
        return ("ERR");
    }

    void EnableDraw(object source, ElapsedEventArgs e){
		enableDraw = 1;
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

    void Leave( ) {
        UnityEngine.Debug.Log("<logAFTap> <sys> EndSession;" + System.DateTime.Now.ToString("h:mm:ss tt"));
        logFile.WriteLine("<logAFTap> <sys> EndSession;" + System.DateTime.Now.ToString("h:mm:ss tt"));

        timerDrawWave.Stop();
        logFile.Close();
    }

    public void OnApplicationQuit() {
        Leave();        
	}
}
