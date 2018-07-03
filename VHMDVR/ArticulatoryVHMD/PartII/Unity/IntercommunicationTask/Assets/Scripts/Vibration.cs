using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Vibration : MonoBehaviour {
    public GameObject Target;
    public string targetname = "";
    public int TrgtID;
    int actualpos = 0;

    /* Serial Port communication with the vibrotactile device */
    static string COMPortBelt = "COM4";
    static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
    static bool portopenBelt = false; //if port is open or not
    char[] SendToArduino = new char[] { '0', ',', '0', ',', '0', ',', '1', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0' };
    public static int[] Intensity = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };

    public GameObject HeadA;
    public GameObject HeadB;

    // AZM only
    GameObject HeadAzimuth;
    Quaternion QtHeadAzimuth;
    GameObject HeadaA;
    GameObject HeadaB;
    // Alt only
    GameObject HeadElevation;
    Quaternion QtHeadElevation;
    GameObject HeadlA;
    GameObject HeadlB;

    RaycastHit hit;
    Vector3 pointingnorth = new Vector3(0, 0, 20);
    public GameObject pointnow = null;
    GameObject previouspoint = null;

    /* Aux variables */
    bool firstArduinoCall = true;
    int FOVindex = 0;
    int headdir = 0;
    int altindex = 0;
    int fovnow = 0;
    int fovbefore = 0;
    int iddle = 1;
    public bool trialState = false;
    char frequency = '9';
    char freqbefore = '0';
    int maxfreq = 9;
    bool lockKey = false;
    int countTrial = 0;

    // Vicon to Unity
    Vector3 vectorNrth = new Vector3(0, 0, 20);
    Vector3 vecLkngAZM; // Vector with head direction in the azimuthal plane
    Vector3 vecTrgtAZM; // Vector with target direction in the azimuthal plane
    Vector3 vecLkng; 
    Vector3 vecTrgt; 
    Vector3 vecLkngALT; // Vector with head direction in the elevation plane
    Vector3 vecTrgtALT; // Vector with target direction in the elevation plane
    float TrgtX = 0;
    float TrgtY = 0;
    float TrgtZ = 1000;
    float AX = 0;
    float AY = 0;
    float AZ = 0;
    float BX = 0;
    float BY = 0;
    float BZ = 0;

    float AaX = 0;
    float AaY = 0;
    float AaZ = 0;
    float BaX = 0;
    float BaY = 0;
    float BaZ = 0;

    float AlX = 0;
    float AlY = 0;
    float AlZ = 0;
    float BlX = 0;
    float BlY = 0;
    float BlZ = 0;

    float azmTarget;
    float azmHead;
    float altTarget;
    public float proxHeadALT = 0;
    public double proxHeadAZM = 0;
    public double angPrecision = 0;
    float altHead;
    public int trialNumber = 0;
    public int sectionNumber = 0;
    Scene scene;
    char sqfrom;
    char sqto;
    char sqdirection;
    public int gestStat = 66;
    public bool flushed = true;

    // Use this for initialization
    void Start () {
        OpenConnectionBelt();

        HeadA = GameObject.Find("HeadA");
        HeadB = GameObject.Find("HeadB");

        HeadElevation = GameObject.Find("HeadElevation");
        HeadlB = GameObject.Find("HeadlB");
        HeadlA = GameObject.Find("HeadlA");

        HeadAzimuth = GameObject.Find("HeadAzimuth");
        HeadaB = GameObject.Find("HeadaB");
        HeadaA = GameObject.Find("HeadaA");

        vectorNrth.x = transform.position.x;
        vectorNrth.y = transform.position.y;
        pointingnorth.x = transform.position.x;
        pointingnorth.y = transform.position.y; 
    }
	
	// Update is called once per frame
	void Update () {
        GetHEadElevation();
        GetHEadAzimuth();

        if (trialState) {
            GetTrgt();
            GetAB();
            GetAzmVectors();
            GetAltVectors();
            GetVectors();
            proxHeadALT = Altitude(vecLkngALT, vecTrgtALT);
            altHead = Altitude(vecLkngALT, vectorNrth);
            proxHeadAZM = ProximityToTarget();
            angPrecision = Precision();
            GetFrequency("QuadPOS", proxHeadALT);
            GetPointDirFOV(AskHeadDirection("Default"));   
        }

        switch (gestStat) {
            case 0:
                DisplayDirectionFOV(10); //2tap
                gestStat = 33;
                break;
            case 1:
                DisplayDirectionFOV(9); // tap
                gestStat = 33;
                break;
            case 2:
                gestStat = 15;
                trialState = true;
                flushed = false;
                break;
            case 3:
                sqfrom = '4';
                sqto = '7';
                sqdirection = '3'; 
                DisplayDirectionFOV(11); // 45r
                gestStat = 33;
                break;
            case 4: 
                sqfrom = '4'; sqto = '1'; sqdirection = '4'; 
                DisplayDirectionFOV(11); // 45l
                gestStat = 33;
                break;
            case 5:
                sqfrom = '1'; sqto = '7'; sqdirection = '3'; 
                DisplayDirectionFOV(11); // 90r
                gestStat = 33;
                break;
            case 6:
                sqfrom = '7'; sqto = '1'; sqdirection = '4'; 
                DisplayDirectionFOV(11); // 90l
                gestStat = 33;
                break;
            case 7:
                DisplayDirectionFOV(0);
                gestStat = 33;
                flushed = false;
                break;
            case 77:
                print("77");
                if (!flushed) { print("66 notflushed"); trialState = false; DisplayDirectionFOV(0); flushed = true; } 
                break;
        }
    }

    void GetHEadElevation() {
        QtHeadElevation = Camera.main.transform.rotation;
        QtHeadElevation.y = 0f;
        QtHeadElevation.z = 0f;
        HeadElevation.transform.rotation = QtHeadElevation;
    }

    void GetHEadAzimuth( ){
        QtHeadAzimuth = Camera.main.transform.rotation;
        QtHeadAzimuth.x = 0f;
        QtHeadAzimuth.z = 0f;
        HeadAzimuth.transform.rotation = QtHeadAzimuth;
    }

    /* Angle between head direction and target */
    private double Precision() {
        //angleTarget = Vector3.Angle(vectorLkng, vectorTrgt);
        angPrecision = SignedAngle(vecLkng, vecTrgt);
        return angPrecision;
    }

    /* Angle between head direction and target in azimuthal plane */
    private double ProximityToTarget() {
        //angleTarget = Vector3.Angle(vectorLkng, vectorTrgt);
        azmTarget = SignedAngle(vecLkngAZM, vecTrgtAZM);
        return azmTarget;
    }

    /* Return cardinal drection of the head related to the target */
    private int AskHeadDirection(string mode) {
        // angleHead = Vector3.Angle(vectorLkng, vectorNrth);
        azmHead = SignedAngle(vecLkngAZM, vectorNrth);
        return AngletoIndex(azmHead);
    }

    /* Detection of cardinal point - Wide angle */
    private int AngletoIndex(double angle) {

        /*if (angle >= -112.5 && angle < -67.5) {          // WEST
            return 0;
        }*/
        if (angle < -67.5) {          // WEST
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
        else if (angle >= 67.5) {    // EAST;
            return 4;
        }
        else {
            // error
            return 333;
        }
    }

    float SignedAngle(Vector3 a, Vector3 b) {
        float angle = Vector3.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.y > 0) angle = -angle;
        //print("ang: " + angle);
        return angle;
    }

    /* Get an cardinal point and display it related to the head - with FOV */
    private void GetPointDirFOV(int inputdir) {
        double angle = ProximityToTarget();
        int motorIndx = AngletoIndex(angle);
        if (inputdir >= 0 && inputdir <= 4) {
            motorIndx++;
            if (motorIndx == 1) { fovnow = 1; if (fovbefore != fovnow || freqbefore != frequency) { DisplayDirectionFOV(7); fovbefore = fovnow; freqbefore = frequency; headdir = inputdir; iddle = 0; } }     // West
            if (motorIndx == 2) { fovnow = 2; if (fovbefore != fovnow || freqbefore != frequency) { DisplayDirectionFOV(6); fovbefore = fovnow; freqbefore = frequency; headdir = inputdir; iddle = 0; } }     // NW
            if (motorIndx == 3) {
                if (FOVindex == -1) { fovnow = 3; if (fovbefore != fovnow || freqbefore != frequency) { DisplayDirectionFOV(5); fovbefore = fovnow; freqbefore = frequency; headdir = inputdir; iddle = 0; } } // Nleft
                if (FOVindex == 0) { fovnow = 4; if (fovbefore != fovnow || freqbefore != frequency) { DisplayDirectionFOV(4); fovbefore = fovnow; freqbefore = frequency; headdir = inputdir; iddle = 0; } }  // NORTH
                if (FOVindex == 1) { fovnow = 5; if (fovbefore != fovnow || freqbefore != frequency) { DisplayDirectionFOV(3); fovbefore = fovnow; freqbefore = frequency; headdir = inputdir; iddle = 0; } }  // Nright
            }
            if (motorIndx == 4) { fovnow = 6; if (fovbefore != fovnow || freqbefore != frequency) { DisplayDirectionFOV(2); fovbefore = fovnow; freqbefore = frequency; headdir = inputdir; iddle = 0; } }     // NE
            if (motorIndx == 5) { fovnow = 7; if (fovbefore != fovnow || freqbefore != frequency) { DisplayDirectionFOV(1); fovbefore = fovnow; freqbefore = frequency; headdir = inputdir; iddle = 0; } }     // East
        }
        else {
            fovnow = 0;
            if (fovbefore != fovnow) {
                DisplayDirectionFOV(0);
                fovbefore = fovnow;
                freqbefore = '0';
                frequency = '0';
            }
        }
    }

    /* Trigger tactor with direction (1-8) or set all tactors off (0) */
    public void DisplayDirectionFOV(int type) {
        switch (type) {
            case 0:
                SendToArduino[0] = '0'; // ON 		0-2
                SendToArduino[2] = '0'; // MOTOR 	1-8
                break;
            case 1:
                SendToArduino[0] = '2'; // ON 		0-2
                SendToArduino[2] = '1'; // MOTOR 	1-8
                SendToArduino[4] = '9'; // FREQ 	0-9
                iddle = 0;
                break;
            case 2:
                SendToArduino[0] = '2'; // ON 		0-2
                SendToArduino[2] = '2'; // MOTOR 	1-8
                SendToArduino[4] = '9'; // FREQ 	0-9
                iddle = 0;
                break;
            case 3:
                SendToArduino[0] = '2'; // ON 		0-2
                SendToArduino[2] = '3'; // MOTOR 	1-8
                SendToArduino[4] = '9'; // FREQ 	0-9
                iddle = 0;
                break;
            case 4:
                SendToArduino[0] = '2'; // ON 		0-2
                SendToArduino[2] = '4';
                SendToArduino[4] = '9'; // FREQ 	0-9
                iddle = 0;
                break;
            case 5:
                SendToArduino[0] = '2'; // ON 		0-2
                SendToArduino[2] = '5'; // MOTOR 	1-8
                SendToArduino[4] = '9'; // FREQ 	0-9
                iddle = 0;
                break;
            case 6:
                SendToArduino[0] = '2'; // ON 		0-2
                SendToArduino[2] = '6'; // MOTOR 	1-8
                SendToArduino[4] = '9'; // FREQ 	0-9
                iddle = 0;
                break;
            case 7:
                SendToArduino[0] = '2'; // ON 		0-2
                SendToArduino[2] = '7'; // MOTOR 	1-8
                SendToArduino[4] = '9'; // FREQ 	0-9
                iddle = 0;
                break;
            case 8:
                SendToArduino[0] = '2'; // ON 		0-2
                SendToArduino[2] = '8'; // MOTOR 	1-8
                SendToArduino[4] = '9'; // FREQ 	0-9
                iddle = 0;
                break;
            case 9:
                // tap
                SendToArduino[0] = '1'; // ON 		0-2
                SendToArduino[2] = '4'; // MOTOR 	1-8
                SendToArduino[4] = '9'; // FREQ 	0-9
                SendToArduino[6] = '1'; // TIMES
                SendToArduino[8] = '0'; // BurstDur
                SendToArduino[10] = '1'; // InterburstDur
                iddle = 0;
                break;
            case 10:
                // 2tap
                SendToArduino[0] = '1'; // ON 		0-2
                SendToArduino[2] = '4'; // MOTOR 	1-8
                SendToArduino[4] = '9'; // FREQ 	0-9
                SendToArduino[6] = '2'; // TIMES
                SendToArduino[8] = '0'; // BurstDur
                SendToArduino[10] = '1'; // InterburstDur
                iddle = 0;
                break;
            case 11:
                // sequency 3/4
                SendToArduino[0] = sqdirection; // ON 		0-2
                SendToArduino[2] = sqfrom; // MOTOR 	1-8
                SendToArduino[4] = '9'; // FREQ 	0-9
                SendToArduino[6] = '2'; // TIMES
                SendToArduino[8] = '0'; // BurstDur
                SendToArduino[10] = '0'; // InterburstDur
                SendToArduino[12] = sqto; // finalmotor
                iddle = 0;
                break;
        }
        //print("Freq: "+ SendToArduino[4] + " Pos: " + SendToArduino[2]);

        Send();
    }

    /* Get frequency */
    void GetFrequency(string mode, float altitude) {
        int index = 0;
        int alt = 0;
        int frq = 0;
        float aux = 0f;

        if (altitude < 0)
            altitude = altitude * (-1);

        index = (int)altitude;
        index = index / 10;
        if (index > maxfreq) index = maxfreq;
        alt = Intensity[index];

        switch (mode) {
            case "Linear":
                frq = alt + 48; // to transform into the proper char
                frequency = (char)frq;
                break;
            case "QuadPOS":
                aux = Mathf.Sqrt(maxfreq * maxfreq - alt * alt);
                aux = aux * (-1);
                frq = (int)aux + maxfreq;
                frq = frq + 48;
                frequency = (char)frq;
                break;
            case "QuadNEG":
                aux = (alt * alt) * (-1);
                aux = Mathf.Sqrt(aux + 2 * alt * maxfreq);
                frq = (int)aux;
                frq = frq + 48;
                frequency = (char)frq;
                break;
            case "Stair":
                if (alt == 0) frequency = '0';
                else if (alt >= 1 && alt < 4) frequency = '3';
                else if (alt >= 4 && alt < 9) frequency = '6';
                else if (alt >= 9) frequency = '9';
                break;
        }
    }

    /* Send string to vibrotactile device **/
    private void Send() {
        try {
            if (firstArduinoCall) { // first Arduino call in trial: beep, play and start counting
                _SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
                //print("sent "+ SendToArduino[2]);
                firstArduinoCall = false;
            } else {
                _SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
                //print("sent "+ SendToArduino[2]);
            }
        }
        catch {
            UnityEngine.Debug.Log("<logApplication> <sys> ERROR Could not write in Serial Port");
        }
    }

    /* Get target and headDirection vectors in azimuthal plane */
    private void GetAzmVectors() {
        vecLkngAZM = new Vector3(BaX - AaX, 0, BaZ - AaZ);
        vecTrgtAZM = new Vector3(TrgtX - transform.position.x, 0f, TrgtZ - transform.position.z);

        //print("BaX: "+BaX+ " AaX: " + AaX+ " BaZ: " + BaZ+ " AaZ: " + AaZ);
    }

    /* Get target and headDirection vectors in elevation plane */
    private void GetAltVectors() {
        vecLkngALT = new Vector3(0, BlY - AlY, BlZ - AlZ);
        vecTrgtALT = new Vector3(0f, TrgtY - transform.position.y, TrgtZ - transform.position.z);
    }

    float Altitude(Vector3 a, Vector3 b) {
        float angle = Vector3.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.x < 0) angle = -angle;
        //print("ang: " + angle);
        return angle;
    }

    /* Get virtual markers */
    void GetAB() {
        AX = HeadA.transform.position.x;
        AY = HeadA.transform.position.y;
        AZ = HeadA.transform.position.z;
        BX = HeadB.transform.position.x;
        BY = HeadB.transform.position.y;
        BZ = HeadB.transform.position.z;

        AaX = HeadaA.transform.position.x;
        AaY = HeadaA.transform.position.y;
        AaZ = HeadaA.transform.position.z;
        BaX = HeadaB.transform.position.x;
        BaY = HeadaB.transform.position.y;
        BaZ = HeadaB.transform.position.z;
    }

    /* Get virtual target */
    void GetTrgt() {
        if (targetname != "") {
            Target = GameObject.Find(targetname);
            if (Target != null) {
                TrgtX = Target.transform.position.x;
                TrgtY = Target.transform.position.y;
                TrgtZ = Target.transform.position.z;
            }
            else {
                //print("Target null");
                trialState = false;
                gestStat = 66;
            }            
        }
        else {
            //print("targetname vazio");
            trialState = false;
            gestStat = 66;
        }
    }

    /* Get target and headDirection vectors in azimuthal plane */
    private void GetVectors() {
        vecTrgt = new Vector3(TrgtX - AX, TrgtY - AY, TrgtZ - AZ);
        vecLkng = new Vector3(BX - AX, BY - AY, BZ - AZ);
    }

    /* Set Serial Port communication with the vibrotactile device */
    public static void OpenConnectionBelt() {
        if (_SerialPortBelt != null) {
            if (_SerialPortBelt.IsOpen) {
                _SerialPortBelt.Close();
                print("<logApplication> <sys> Closing port, because it was already open!");

                _SerialPortBelt.Open();
                portopenBelt = true;
            }
            else {
                _SerialPortBelt.Open();
                portopenBelt = true;
            }
        }
        else {
            if (_SerialPortBelt.IsOpen) { print("<logApplication> <sys> Port is already open"); }
            else { print("<logApplication> <sys> Port == null"); }
        }
    }

    /* Ends Serial Port communication with the vibrotactile device */
    public static void CloseConnection() {
        if (portopenBelt == true) {
            try {
                _SerialPortBelt.Close();
                print("<logApplication> <sys> Closing port: " + COMPortBelt);
            }
            catch {
                print("<logApplication> <sys> ERROR in closing " + COMPortBelt);
            }
        }
    }

    /* Quit/Exit/Close */
    public void CloseAll() {
        //clean...
        DisplayDirectionFOV(0);
        //and close
        if (portopenBelt) {
            CloseConnection();
            portopenBelt = false;
        }
    }

    /* Quit/Exit/Close */
    void OnApplicationQuit() {
        CloseAll();
    }
}
