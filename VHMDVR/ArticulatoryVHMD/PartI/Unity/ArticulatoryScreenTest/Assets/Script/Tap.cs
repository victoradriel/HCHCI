using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tap : MonoBehaviour {
    GameObject[] conditions = new GameObject[5];
    int[] condcount = new int[] { 12, 12, 12, 12, 12 };
    string[] trials = new string[] { "CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25","CP05", "CP10", "CP15", "CP20", "CP25" };
    string[] tCP05 = new string[] { "P01","P02","P03","P04","P05","P02","P03","P04","P05","P01","P02","P04" };
    string[] tCP10 = new string[] { "P01","P02","P03","P04","P05","P06","P07","P08","P09","P10","P02","P09" };
    string[] tCP15 = new string[] { "P01","P02","P03","P04","P05","P06","P07","P08","P09","P10","P11","P12","P13","P14","P15" };
    string[] tCP20 = new string[] { "P01","P02","P03","P04","P05","P06","P07","P08","P09","P10","P11","P12","P13","P14","P15","P16","P17","P18","P19","P20" };
    string[] tCP25 = new string[] { "P01","P02","P03","P04","P05","P06","P07","P08","P09","P10","P11","P12","P13","P14","P15","P16","P17","P18","P19","P20","P21","P22","P23","P24","P25" };    
    public int trialID = 0;
    bool trialState = false;
    GameObject target;
    public string targetnow = "";
    int res = 2560; // GS6
    public float xps = 0;
    int answerID = 0;
    float delta = 0;
    public int balls = 0; 
    public float answering = 0;  
    StreamWriter logFile;
    bool treinoon = true;
    Text feedback;
    Text display;

	// Use this for initialization
	void Start () {
        logFile = File.CreateText(Application.persistentDataPath + "/logAFTap-" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv");
        UnityEngine.Debug.Log("<logAFTap> <sys> StartTime;" + System.DateTime.Now.ToString("h:mm:ss tt"));
        logFile.WriteLine("<logAFTap> <sys> StartTime;" + System.DateTime.Now.ToString("h:mm:ss tt"));

        feedback = GameObject.Find("feedback").GetComponent<Text>();
        display = GameObject.Find("display").GetComponent<Text>();

        for (int i = 0; i < conditions.Length; i++) {
            conditions[i] = GameObject.Find(trials[i]);
            conditions[i].SetActive(false);
        }

        trials = MyFisherYatesShffl(trials);        
        tCP05 = MyFisherYatesShffl(tCP05); 
        tCP10 = MyFisherYatesShffl(tCP10); 
        tCP15 = MyFisherYatesShffl(tCP15); 
        tCP20 = MyFisherYatesShffl(tCP20); 
        tCP25 = MyFisherYatesShffl(tCP25);
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
                SetTrial(trials [trialID], pos);
                DisplayStm(pos);
                UnityEngine.Debug.Log("<logAFTap> <sys> <display>;" + trialID + ";" + trials [trialID] + ";" + pos);
                logFile.WriteLine("<logAFTap> <sys> <display>;" + trialID + ";" + trials [trialID] + ";" + pos);
                display.text = "Stm #" + (trialID+1);
                trialID++;
                if (treinoon) { feedback.text = ""; }
            }
            else {
                UnityEngine.Debug.Log("<logAFTap> <sys> EndSession;" + System.DateTime.Now.ToString("h:mm:ss tt"));
                logFile.WriteLine("<logAFTap> <sys> EndSession;" + System.DateTime.Now.ToString("h:mm:ss tt"));
                display.text = "End of Session";
            }
        }

        if (MainController.control.sentSmthng && trialState) {
            trialState = false;
            MainController.control.sentSmthng = false;
            display.text = " ";

            xps = MainController.control.xstrt;
            GetAnswer(xps);    
                   
            target.GetComponent<Renderer>().material.color = Color.white;   
            for (int i = 0; i < conditions.Length; i++) {
                conditions[i].SetActive(false);
            }         
        }

        if (Input.GetKey(KeyCode.Escape)) {
            Leave();
            SceneManager.LoadScene(0);
        } 
	}

    public void GetAnswer(float xanswer) {
        string thistapfinal = "";
        if (xanswer % delta > 0) answerID = (int) (xanswer / delta + 1); 
        else answerID = (int) (xanswer / delta);

        if (answerID < 1) answerID = 1;
        if (answerID > balls) answerID = balls;

        string [] trgtST = targetnow.Split('P');
        int thistrgtID = System.Int32.Parse(trgtST[1]);

        if (thistrgtID == answerID) thistapfinal = "1;" + answerID; //print("RIGHT");
        else thistapfinal = "0;" + answerID; //print("WRONG");

        // get answer Stm;(0/1;StmAnswer);Raw
        UnityEngine.Debug.Log("<logAFTap> <answer>;" + trials [trialID-1] + ";" + thistrgtID + ";" + thistapfinal + ";" + (xanswer / delta) );
        logFile.WriteLine("<logAFTap> <answer>;" + trials [trialID-1] + ";" + thistrgtID + ";" + thistapfinal + ";" + (xanswer / delta) );

        if (treinoon) { feedback.text = "Stm: " + thistrgtID + " Ans: " + (xanswer / delta); }
    }

    public void SetTrial(string thistrial, string thispos) {
        targetnow = thispos;

        string [] ballsp1 = thistrial.Split('C');
        string [] ballsp2 = ballsp1[1].Split('P');
        balls = System.Int32.Parse(ballsp2[1]);
        delta = res / balls;

    }

    public string GetPosition(string thistrial) {
        switch (thistrial) {
            case "CP05":
                conditions[0].SetActive(true);
                if (condcount[0] != 0) {
                    condcount[0] = condcount[0] - 1;
                    return (tCP05[condcount[0]]);
                }
                break;
            case "CP10":
                conditions[1].SetActive(true);
                if (condcount[1] != 0) {
                    condcount[1] = condcount[1] - 1;
                    return (tCP10[condcount[1]]);
                }
                break;
            case "CP15":
                conditions[2].SetActive(true);
                if (condcount[2] != 0) {
                    condcount[2] = condcount[2] - 1;
                    return (tCP15[condcount[2]]);
                }
                break;
            case "CP20":
                conditions[3].SetActive(true);
                if (condcount[3] != 0) {
                    condcount[3] = condcount[3] - 1;
                    return (tCP20[condcount[3]]);
                }
                break;
            case "CP25":
                conditions[4].SetActive(true);
                if (condcount[4] != 0) {
                    condcount[4] = condcount[4] - 1;
                    return (tCP25[condcount[4]]);
                }
                break;
        }
        return ("ERR");
    }

    public void DisplayStm(string thistgt) {
        target = GameObject.Find(thistgt);
        target.GetComponent<Renderer>().material.color = Color.red;
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
            
        logFile.Close();
    }

    public void OnApplicationQuit() {
        Leave();        
	}
}
