using UnityEngine;
using System.IO.Ports;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VicontoUnity : MonoBehaviour {
    public EnemyHealth enemyHealth;
    Text timetext;
    public GameObject score;
    Text scoretext;
    public GameObject timerplay;
    Vector3 vectorNrth = new Vector3(0, 0, 1);
    Vector3 vectorLkng;
    Vector3 vectorTrgt;
    float TrgtX = 0;
    float TrgtY = 1000;
    float AX = 0;
    float AY = 0;
    float BX = 0;
    float BY = 0;
    float angleTarget;
    float angleHead;
    public GameObject HeadA;
    public GameObject HeadB;
    public GameObject Trgt;
    public GameObject enemysoul;
    public Renderer rend;
    bool enemyishere = false;

    /* Aux variables */
    int FOVindex = 0;
    int headdir = 0;
    int fovnow = 0;
    int fovbefore = 0;
    int iddle = 1;
    bool trialState = true;

    /* Serial Port communication with the vibrotactile device */
    static string COMPortBelt = "COM3";
    static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
    static bool portopenBelt = false; //if port is open or not
    char[] SendToArduino = new char[] { '0', ',', '0', ',', '0', ',', '1', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0' };
    //static string[] SpMotors = new string[] { "Motor1", "Motor2", "Motor3", "Motor4", "Motor5", "Motor6", "Motor7", "Motor8" };
    static string[] SpMotors = new string[] { "Motor8", "Motor7", "Motor6", "Motor5", "Motor4", "Motor3", "Motor2", "Motor1" };
    public static int[] MotorPosition = new int[] { 0, 2, 4, 6, 8, 10, 12, 14 };

    // Use this for initialization
    void Start () {
        HeadA = GameObject.Find("HeadA");
        HeadB = GameObject.Find("HeadB");

        OpenConnectionBelt();

        timerplay = GameObject.Find("TimerText");
        timetext = timerplay.GetComponent<Text>();

        score = GameObject.Find("ScoreText");
        scoretext = score.GetComponent<Text>();
    }

    void FindEnemy() {
        if (GameObject.Find("ZomBunny(Clone)")) {
            Trgt = GameObject.Find("ZomBunny(Clone)");
            TrgtX = Trgt.transform.position.x;
            TrgtY = Trgt.transform.position.z;
            enemysoul = GameObject.Find("Zombunny");
            rend = enemysoul.GetComponent<Renderer>();
            enemyishere = true;
        } else if (GameObject.Find("ZomBear(Clone)")) {
            Trgt = GameObject.Find("ZomBear(Clone)");
            TrgtX = Trgt.transform.position.x;
            TrgtY = Trgt.transform.position.z;
            enemysoul = GameObject.Find("ZomBear");
            rend = enemysoul.GetComponent<Renderer>();
            enemyishere = true;
        } else if (GameObject.Find("Hellephant(Clone)")) {
            Trgt = GameObject.Find("Hellephant(Clone)");
            TrgtX = Trgt.transform.position.x;
            TrgtY = Trgt.transform.position.z;
            enemysoul = GameObject.Find("Hellephant");
            rend = enemysoul.GetComponent<Renderer>();
            enemyishere = true;
        }   
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.R)){
            DisplayDirectionFOV(0);
            SceneManager.LoadScene(0);
        }

        if (timetext.color == Color.red) {
            print("< logVicon > Exit time" + System.DateTime.Now.ToString("h: mm:ss tt"));
            //clean...
            DisplayDirectionFOV(0);
            //and close
            if (portopenBelt)
            {
                CloseConnection();
                portopenBelt = false;
            }

            PlayerPrefs.SetString("FinalScore", scoretext.text);
            SceneManager.LoadScene(2);
        }

        /* if(transform.position.z < -10)
             transform.Translate(Vector3.forward * Time.deltaTime);*/

        GetAB();
        FindEnemy();

        if (rend != null){
            if (rend.enabled){ trialState = false; DisplayDirectionFOV(0); }
            else { trialState = true; }
                
        }

        if (trialState == true){
            GetVectors();
            GetPointDirFOV(AskHeadDirection("Default"));
            ProximityToTarget();
        }
        
    }

    /* Get virtual markers */
    void GetAB() {
        AX = HeadA.transform.position.x;
        AY = HeadA.transform.position.z;
        BX = HeadB.transform.position.x;
        BY = HeadB.transform.position.z;
    }

    /* Get target and headDirection vectors */
    private void GetVectors() {
        vectorTrgt = new Vector3(TrgtX - AX, 0, TrgtY - AY);
        vectorLkng = new Vector3(BX - AX, 0, BY - AY);
    }

    float SignedAngle(Vector3 a, Vector3 b){
        float angle = Vector3.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.y > 0) angle = -angle;
        return angle;
    }

    /* Angle between head direction and target */
    private double ProximityToTarget() {
        //angleTarget = Vector3.Angle(vectorLkng, vectorTrgt);
        angleTarget = SignedAngle(vectorLkng, vectorTrgt);
        return angleTarget;
    }

    /* Return cardinal drection of the head related to the target */
    private int AskHeadDirection(string mode)
    {
        // angleHead = Vector3.Angle(vectorLkng, vectorNrth);
        angleHead = SignedAngle(vectorLkng, vectorNrth);
        return AngletoIndex(angleHead);
    }

    /* Get an cardinal point and display it related to the head - with FOV */
    private void GetPointDirFOV(int inputdir) {
        double angle = ProximityToTarget();
        int motorIndx = AngletoIndex(angle);
        //print(angle);
        if (inputdir >= 0 && inputdir <= 4) {
            motorIndx++;
            if (motorIndx == 1) { fovnow = 1; if (fovbefore != fovnow) { DisplayDirectionFOV(7); fovbefore = fovnow; headdir = inputdir; iddle = 0; } }     // West
            if (motorIndx == 2) { fovnow = 2; if (fovbefore != fovnow) { DisplayDirectionFOV(6); fovbefore = fovnow; headdir = inputdir; iddle = 0; } }     // NW
            if (motorIndx == 3) {
                if (FOVindex == -1) { fovnow = 3; if (fovbefore != fovnow) { DisplayDirectionFOV(5); fovbefore = fovnow; headdir = inputdir; iddle = 0; } } // Nleft
                if (FOVindex == 0) { fovnow = 4; if (fovbefore != fovnow) { DisplayDirectionFOV(4); fovbefore = fovnow; headdir = inputdir; iddle = 0; } }  // NORTH
                if (FOVindex == 1) { fovnow = 5; if (fovbefore != fovnow) { DisplayDirectionFOV(3); fovbefore = fovnow; headdir = inputdir; iddle = 0; } }  // Nright
            }
            if (motorIndx == 4) { fovnow = 6; if (fovbefore != fovnow) { DisplayDirectionFOV(2); fovbefore = fovnow; headdir = inputdir; iddle = 0; } }     // NE
            if (motorIndx == 5) { fovnow = 7; if (fovbefore != fovnow) { DisplayDirectionFOV(1); fovbefore = fovnow; headdir = inputdir; iddle = 0; } }     // East
        }
        else {
            fovnow = 0;
            if (fovbefore != fovnow) {
                DisplayDirectionFOV(0);
                fovbefore = fovnow;
            }
        }
    }

    /* Detection of cardinal point - Wide angle */
    private int AngletoIndex(double angle) {

        /*if (angle >= -112.5 && angle < -67.5) {          // WEST
            return 0;
        }*/
        if (angle < -67.5) {          // WEST
            return 0;
        } else if (angle >= -67.5 && angle < -22.5) {    // NW
            return 1;
        } else if (angle >= -22.5 && angle < -7.5) {    // NORTH (left)
            FOVindex = -1;
            return 2;
        } else if (angle >= -2 && angle < 2) {      // NORTH
            FOVindex = 0;
            return 2;
        } else if (angle >= 7.5 && angle < 22.5) {      // NORTH (right)
            FOVindex = 1;
            return 2;
        } else if (angle >= 22.5 && angle < 67.5) {     // NE
            return 3;
        } else if (angle >= 67.5) {    // EAST;
            return 4;
        } else {
            // error
            return 333;
        }
    }

    /* Trigger tactor with direction (1-8) or set all tactors off (0) */
    private void DisplayDirectionFOV(int type) {
        SendToArduino[0] = '2'; // ON 		0-2
        SendToArduino[4] = '0'; // FREQ 	0-2

        switch (type) {
            case 0:
                SendToArduino[0] = '0'; // ON 		0-2
                SendToArduino[2] = '0'; // MOTOR 	1-8
                break;
            case 1:
                SendToArduino[2] = '1'; // MOTOR 	1-8
                iddle = 0;
                break;
            case 2:
                SendToArduino[2] = '2'; // MOTOR 	1-8
                iddle = 0;
                break;
            case 3:
                SendToArduino[2] = '3'; // MOTOR 	1-8
                iddle = 0;
                break;
            case 4:
                SendToArduino[2] = '4';
                iddle = 0;
                break;
            case 5:
                SendToArduino[2] = '5'; // MOTOR 	1-8
                iddle = 0;
                break;
            case 6:
                SendToArduino[2] = '6'; // MOTOR 	1-8
                iddle = 0;
                break;
            case 7:
                SendToArduino[2] = '7'; // MOTOR 	1-8
                iddle = 0;
                break;
            case 8:
                SendToArduino[2] = '8'; // MOTOR 	1-8
                iddle = 0;
                break;
        }

        Send();
    }

    /* Send string to vibrotactile device **/
    private void Send() {
        try {
            _SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
            //LogRecord(logPath, "<logVicon> <sys> Submitted to Arduino");
        }
        catch {
            print("<logVicon> <sys> ERROR Could not write in Serial Port");
        }
    }

    /*void Send() {
        GameObject motor;

        if (SendToArduino[0] == '2') {
            zeraArray();

            motor = GameObject.Find(SpMotors[(int)char.GetNumericValue(SendToArduino[2])]);
            motor.GetComponent<Renderer>().material.color = Color.red;
        }
    }*/

    void zeraArray() {
        GameObject motor;
        for (int i = 0; i < 8; i++) {
            motor = GameObject.Find(SpMotors[i]);
            motor.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    /* Set Serial Port communication with the vibrotactile device */
    public static void OpenConnectionBelt() {
        if (_SerialPortBelt != null) {
            if (_SerialPortBelt.IsOpen) {
                _SerialPortBelt.Close();
                print("<logVicon> <sys> Closing port, because it was already open!");

                _SerialPortBelt.Open();
                portopenBelt = true;
            } else {
                _SerialPortBelt.Open();
                portopenBelt = true;
            }
        } else {
            if (_SerialPortBelt.IsOpen) { print("<logVicon> <sys> Port is already open"); }
            else { print("<logVicon> <sys> Port == null"); }
        }
    }

    /* Ends Serial Port communication with the vibrotactile device */
    public static void CloseConnection() {
        if (portopenBelt == true) {
            try {
                _SerialPortBelt.Close();
                print("<logVicon> <sys> Closing port: " + COMPortBelt);
            }
            catch {
                print("<logVicon> <sys> ERROR in closing " + COMPortBelt);
            }
        }
    }

    /* Quit/Exit/Close */
    void OnApplicationQuit() {
        print("< logVicon > Exit time" + System.DateTime.Now.ToString("h: mm:ss tt"));
        //clean...
        DisplayDirectionFOV(0);
        //and close
        if (portopenBelt) {
            CloseConnection();
            portopenBelt = false;
        } 
    }
}
