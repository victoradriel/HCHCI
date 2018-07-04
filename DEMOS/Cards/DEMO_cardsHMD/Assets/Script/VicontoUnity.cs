using UnityEngine;
using System.IO.Ports;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VicontoUnity : MonoBehaviour {
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

    /* Aux variables */
    int FOVindex = 0;
    int headdir = 0;
    int fovnow = 0;
    int fovbefore = 0;
    int iddle = 1;
    bool trialState = true;
    Text statusText;
    Text countdownText;

    float timeLeft = 3f;

    /* Serial Port communication with the vibrotactile device */
    static string COMPortBelt = "COM3";
    static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
    static bool portopenBelt = false; //if port is open or not
    char[] SendToArduino = new char[] { '0', ',', '0', ',', '0', ',', '1', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0' };

    // Use this for initialization
    void Start () {
        PlayerPrefs.SetInt("Selected", 0);

        HeadA = GameObject.Find("HeadA");
        HeadB = GameObject.Find("HeadB");
        Trgt = GameObject.Find("CardJoker");

        TrgtX = Trgt.transform.position.x;
        TrgtY = Trgt.transform.position.z;

        OpenConnectionBelt();
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Jump")) {
            DisplayDirectionFOV(0);
            CloseConnection();
            SceneManager.LoadScene(0);
        }

        /* if (Input.GetKey(KeyCode.V))
             trialState = true;*/

        if (PlayerPrefs.GetInt("Selected") == 1) {
            trialState = false;
            DisplayDirectionFOV(0);
            statusText = GameObject.Find("Intro").GetComponent<Text>();
            countdownText = GameObject.Find("Timer").GetComponent<Text>();

            if (PlayerPrefs.GetInt("Answer") == 1) {
                statusText.text = "GREAT!!";
            }
            else {
                statusText.text = "Whoops. Try again.";      
            }

            timeLeft -= Time.deltaTime;
            countdownText.text = "" + System.Math.Round(timeLeft, 0);

            if (timeLeft <= 0) {
                DisplayDirectionFOV(0);
                CloseConnection();
                SceneManager.LoadScene(Random.Range(1, 5));
            }
        }

        /*var newRotation = Input.GetAxis("Horizontal") * (60);
        transform.Rotate(0, newRotation * Time.deltaTime, 0);*/

        GetAB();

        if (trialState == true) {
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

        if (angle >= -112.5 && angle < -67.5) {          // WEST
            return 0;
        }
        else if (angle >= -67.5 && angle < -22.5) {    // NW
            return 1;
        }
        else if (angle >= -22.5 && angle < -7.5) {    // NORTH (left)
            FOVindex = -1;
            return 2;
        }
        else if (angle >= -7.5 && angle < 7.5) {      // NORTH
            FOVindex = 0;
            return 2;
        }
        else if (angle >= 7.5 && angle < 22.5) {      // NORTH (right)
            FOVindex = 1;
            return 2;
        }
        else if (angle >= 22.5 && angle < 67.5) {     // NE
            return 3;
        }
        else if (angle >= 67.5 && angle < 112.5) {    // EAST;
            return 4;
        }
        else {
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

    /* Send string to vibrotactile device */
    private void Send() {
        try {
            _SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
            //LogRecord(logPath, "<logVicon> <sys> Submitted to Arduino");
        }
        catch {
            print("<logVicon> <sys> ERROR Could not write in Serial Port");
        }
    }

    /* Set Serial Port communication with the vibrotactile device */
    public static void OpenConnectionBelt() {
        if (_SerialPortBelt != null) {
            if (_SerialPortBelt.IsOpen) {
                _SerialPortBelt.Close();
                print("<logVicon> <sys> Closing port, because it was already open!");
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
