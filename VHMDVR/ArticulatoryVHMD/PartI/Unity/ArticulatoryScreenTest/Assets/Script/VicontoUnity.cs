using UnityEngine;
using System.IO.Ports;
using System.Diagnostics;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class VicontoUnity : MonoBehaviour {
    int userID = 1;

    GameObject[] Target = new GameObject[15];
    string[] targetnames = new string[] { "C1L4", "C2L1", "C3L3", "C4L2", "C5L1", "C6L4", "C7L1", "C8L3", "C9L2", "C10L4", "C1L2", "C4L4", "C5L3", "C6L2", "C10L1" };
    //string[] targetnames = new string[] { "C1L4", "C2L1", "C3L3", "C4L2", "C5L1", "C6L4", "C7L1", "C8L3", "C9L2", "C10L4" };

    string[] trgtinfo = new string[] { "", "", "" };
    bool somethingslctd = false;
    RaycastHit hit;
    Vector3 pointingnorth = new Vector3(0, 0, 10);
    public GameObject pointnow = null;
    GameObject previouspoint = null;

    int NumberTrials = 0;

    // Vicon to Unity
    Vector3 vectorNrth = new Vector3(0, 0, 1);
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

    float AlX = 0;
    float AlY = 0;
    float AlZ = 0;
    float BlX = 0;
    float BlY = 0;
    float BlZ = 0;

    float AaX = 0;
    float AaY = 0;
    float AaZ = 0;
    float BaX = 0;
    float BaY = 0;
    float BaZ = 0;

    float azmTarget;
    float azmHead;
    float altTarget;
    public float proxHeadALT = 0;
    public double proxHeadAZM = 0;
    public double angPrecision = 0;
    float altHead;
    public int trialNumber = 0;
    public int sectionNumber = 0;
    Stopwatch stopwatch;
    long elapsed_time = 0;
    public GameObject HeadA;
    public GameObject HeadB;
    public GameObject Trgt;

    GameObject HeadElevation;
    Quaternion QtHeadElevation;
    GameObject HeadAzimuth;
    Quaternion QtHeadAzimuth;
    GameObject HeadlA;
    GameObject HeadlB;
    GameObject HeadaA;
    GameObject HeadaB;
    GameObject Crumb;

    Light mLight; 

    bool firstArduinoCall = true;

    /* Aux variables */
    int FOVindex = 0;
    int headdir = 0;
    public int altindex = 0;
    int fovnow = 0;
    int fovbefore = 0;
    int iddle = 1;
    bool trialState = false;
    char frequency = '0';
    char freqbefore = '0';
    int maxfreq = 9;
    public bool lockKey = false;
    int countTrial = 0;
    int countFrames = 0;

    //logs
    float logproxHeadALT = 0;
    double logproxHeadAZM = 0;
    double logangPrecision = 0;
    GameObject logpointnow = null;
    GameObject logtarget = null;
    string logtrajectory = "";
    long logRT = 0;
    bool lockVib = true;
    int score = 0;

    /* Serial Port communication with the vibrotactile device */
    static string COMPortBelt = "COM3";
    static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
    static bool portopenBelt = false; //if port is open or not
    char[] SendToArduino = new char[] { '0', ',', '0', ',', '0', ',', '1', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0' };
    public static int[] Intensity = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };


    StreamWriter logFile;
    // Use this for initialization
    void Start() {
        logFile = File.CreateText(Application.persistentDataPath + "/logApplication-" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv");

        UnityEngine.Debug.Log("<logApplication> <sys> StartTime:" + System.DateTime.Now.ToString("h:mm:ss tt"));
        logFile.WriteLine("<logApplication> <sys> StartTime:" + System.DateTime.Now.ToString("h:mm:ss tt"));

        stopwatch = new Stopwatch();
        NumberTrials = targetnames.Length;

        HeadA = GameObject.Find("HeadA");
        HeadB = GameObject.Find("HeadB");

        HeadElevation = GameObject.Find("HeadElevation");
        HeadlB = GameObject.Find("HeadlB");
        HeadlA = GameObject.Find("HeadlA");

        HeadAzimuth = GameObject.Find("HeadAzimuth");
        HeadaB = GameObject.Find("HeadaB");
        HeadaA = GameObject.Find("HeadaA");

        Crumb = GameObject.Find("Crumb");
        GameObject PLight = GameObject.Find("Pointlight");
        mLight = PLight.GetComponent<Light>();
        mLight.color = Color.black;

        targetnames = MyFisherYatesShffl(targetnames);
        for (int i = 0; i < Target.Length; i++) {
            Target[i] = GameObject.Find(targetnames[i]);
        }

        print(Application.persistentDataPath);
        //Trgt = Target[0];
        //getTrgtInfo(Target[0]);

        OpenConnectionBelt();
    }

    void GetHEadElevation() {
        QtHeadElevation = Camera.main.transform.rotation;
        QtHeadElevation.y = 0f;
        QtHeadElevation.z = 0f;
        HeadElevation.transform.rotation = QtHeadElevation;
    }

    void GetHEadAzimuth() {
        QtHeadAzimuth = Camera.main.transform.rotation;
        QtHeadAzimuth.x = 0f;
        QtHeadAzimuth.z = 0f;
        HeadAzimuth.transform.rotation = QtHeadAzimuth;
    }

    // Update is called once per frame
    void Update() {

        GetHEadElevation();
        GetHEadAzimuth();

        if (Input.GetButton("Fire3")) {
            lockVib = false;
            mLight.color = Color.white;
        }

        if (Input.GetKey(KeyCode.Space)) {
            Scene scene = SceneManager.GetActiveScene();
            UnityEngine.Debug.Log("<logApplication> <scene> End of " + scene.name);
            logFile.WriteLine("<logApplication> <scene> End of " + scene.name);
            CloseAll();
            PlayerPrefs.SetInt("Score" + scene.buildIndex, score);
            SceneManager.LoadScene(scene.buildIndex+1);
        }

        if (Input.GetButton("Fire1") && !lockKey && !lockVib) {
            Select(pointnow);
        }

        if (!somethingslctd)
            LookingAt();

        if (trialState == false)
            countTrial++;

        if (Input.GetKey(KeyCode.K)) 
            lockKey = false;
        

        // Start Trial
        if (countTrial > 150) {
            // Trigger trial
            if (!trialState && trialNumber < NumberTrials) {
                if (previouspoint != null) {
                    MovingOn();
                    somethingslctd = false;
                }
                UnityEngine.Debug.Log("<logApplication> <sys> Triggering stimulus #" + trialNumber);
                logFile.WriteLine("<logApplication> <sys> Triggering stimulus #" + trialNumber);
                trialState = true;
                countTrial = 0;
                lockKey = false;
                stopwatch.Reset();
            }
            else {
                if (previouspoint != null) {
                    MovingOn();
                    somethingslctd = false;
                }
                UnityEngine.Debug.Log("<logApplication> <sys> End of Session" + System.DateTime.Now.ToString("h:mm:ss tt"));
                logFile.WriteLine("<logApplication> <sys> End of Session" + System.DateTime.Now.ToString("h:mm:ss tt"));
                mLight.color = Color.black;
            }
        }

        if (trialState == true) {
            // Get Head piece and Target
            GetAB();
            GetTrgt(); 

            // Get vectors
            GetAzmVectors();
            GetAltVectors();
            GetVectors();

            // Get angles
            proxHeadALT = Altitude(vecLkngALT, vecTrgtALT);
            altHead = Altitude(vecLkngALT, vectorNrth);
            proxHeadAZM = ProximityToTarget();
            angPrecision = Precision();
            //UnityEngine.Debug.DrawLine(Vector3.zero, vecLkngALT, Color.blue);
            //UnityEngine.Debug.DrawLine(Vector3.zero, vecTrgtALT, Color.red);

            if (!lockVib) {
                // Display tactile feedback
                GetFrequency("QuadPOS", proxHeadALT);
                GetPointDirFOV(AskHeadDirection("Default"));
            }

            countFrames++;
            if (countFrames % 5 == 0) {
                logtrajectory = logtrajectory + Crumb.transform.position + ";";
                countFrames = 0;
            }
        }
    }

    void LookingAt() {
        if (Physics.Raycast(transform.position, transform.TransformDirection(pointingnorth), out hit)) {
            pointnow = GameObject.Find(hit.collider.name);

            if (pointnow.tag == "Selectable") {
                pointnow.GetComponent<Renderer>().material.color = Color.yellow;
                foreach (Renderer r in pointnow.GetComponentsInChildren<Renderer>()) {
                    r.material.color = Color.yellow;
                }
                if (previouspoint != pointnow && previouspoint != null) MovingOn();
                previouspoint = pointnow;
            }
            else {
                if (previouspoint != null) MovingOn();
            }
        }
    }

    void MovingOn() {
        previouspoint.GetComponent<Renderer>().material.color = Color.white;
        foreach (Renderer r in previouspoint.GetComponentsInChildren<Renderer>()) {
            r.material.color = Color.white;
        }
    }

    void Select(GameObject point) {
        if (trialState && point.tag == "Selectable") {
            stopwatch.Stop();
            elapsed_time = stopwatch.ElapsedMilliseconds;

            lockKey = true;
            somethingslctd = true;

            logproxHeadALT = proxHeadALT;
            logproxHeadAZM = proxHeadAZM;
            logangPrecision = angPrecision;
            logpointnow = pointnow;
            logtarget = Target[trialNumber];
            logRT = elapsed_time;

            verifyAnswer(point);
            setIddleState();
            trialState = false;

            trialNumber++;
        }
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

    void setIddleState() {
        DisplayDirectionFOV(0);

        FOVindex = 0;
        headdir = 0;
        fovnow = 0;
        fovbefore = 0;
        iddle = 1;
        frequency = '0';
        freqbefore = '0';

        logproxHeadALT = 0;
        logproxHeadAZM = 0;
        logangPrecision = 0;
        logpointnow = null;
        logtarget = null;
        logRT = 0;
        logtrajectory = "";

        firstArduinoCall = true;
    }

    void verifyAnswer(GameObject point) {

        if (point.name == Trgt.name) {
            point.GetComponent<Renderer>().material.color = Color.green;
            foreach (Renderer r in point.GetComponentsInChildren<Renderer>()) {
                r.material.color = Color.green;
            }

            UnityEngine.Debug.Log("<logApplication> <answer> "+ logRT +";1;;"+ Trgt.name +";"+ logangPrecision);
            UnityEngine.Debug.Log("<logApplication> <precisions> alt;"+ logproxHeadALT + ";azm;" + logproxHeadAZM + ";gen;" + logangPrecision);
            UnityEngine.Debug.Log("<logApplication> <trajectory> " + logtrajectory);
            score++;

            // Log To File
            logFile.WriteLine("<logApplication> <answer> " + logRT + ";1;;" + Trgt.name + ";" + logangPrecision);
            logFile.WriteLine("<logApplication> <precisions> alt;" + logproxHeadALT + ";azm;" + logproxHeadAZM + ";gen;" + logangPrecision);
            logFile.WriteLine("<logApplication> <trajectory> " + logtrajectory);
        }
        else {
            point.GetComponent<Renderer>().material.color = Color.red;
            foreach (Renderer r in point.GetComponentsInChildren<Renderer>()) {
                r.material.color = Color.red;
            }

            UnityEngine.Debug.Log("<logApplication> <answer> " + logRT + ";0;"+ point.name + ";" + Trgt.name + ";" + logangPrecision);
            UnityEngine.Debug.Log("<logApplication> <precisions> alt;" + logproxHeadALT + ";azm;" + logproxHeadAZM + ";gen;" + logangPrecision);
            UnityEngine.Debug.Log("<logApplication> <trajectory> " + logtrajectory);

            // Log To File
            logFile.WriteLine("<logApplication> <answer> " + logRT + ";0;" + point.name + ";" + Trgt.name + ";" + logangPrecision);
            logFile.WriteLine("<logApplication> <precisions> alt;" + logproxHeadALT + ";azm;" + logproxHeadAZM + ";gen;" + logangPrecision);
            logFile.WriteLine("<logApplication> <trajectory> " + logtrajectory);
        }
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

    /* Get virtual markers */
    void GetAB() {
        AX = HeadA.transform.position.x;
        AY = HeadA.transform.position.y;
        AZ = HeadA.transform.position.z;
        BX = HeadB.transform.position.x;
        BY = HeadB.transform.position.y;
        BZ = HeadB.transform.position.z;

        AlX = HeadlA.transform.position.x;
        AlY = HeadlA.transform.position.y;
        AlZ = HeadlA.transform.position.z;
        BlX = HeadlB.transform.position.x;
        BlY = HeadlB.transform.position.y;
        BlZ = HeadlB.transform.position.z;

        AaX = HeadaA.transform.position.x;
        AaY = HeadaA.transform.position.y;
        AaZ = HeadaA.transform.position.z;
        BaX = HeadaB.transform.position.x;
        BaY = HeadaB.transform.position.y;
        BaZ = HeadaB.transform.position.z;
    }

    /* Get virtual target */
    void GetTrgt() {
        Trgt = Target[trialNumber];
        getTrgtInfo(Trgt);
        //print("Col: " + trgtinfo[1] + "Row: " + trgtinfo[2]);

        TrgtX = Trgt.transform.position.x;
        TrgtY = Trgt.transform.position.y;
        TrgtZ = Trgt.transform.position.z;
    }

    void getTrgtInfo(GameObject thisTarget) {
        string targetname = thisTarget.name;
        char[] delimiterChars = { 'C', 'L' };
        int i = 0;

        string[] pos = targetname.Split(delimiterChars);
        foreach (string s in pos) {
            trgtinfo[i] = s;
            i++;
        }  
    }

    /* Get target and headDirection vectors in azimuthal plane */
    private void GetVectors() {
        vecTrgt = new Vector3(TrgtX - AX, TrgtY - AY, TrgtZ - AZ);
        vecLkng = new Vector3(BX - AX, BY - AY, BZ - AZ);
    }

    /* Get target and headDirection vectors in azimuthal plane */
    private void GetAzmVectors() {
        vecLkngAZM = new Vector3(BaX - AaX, 0, BaZ - AaZ);

        switch (trgtinfo[1]) {
            case "1":
                vecTrgtAZM = new Vector3(-5.884711f, 0f, -1.170542f);
                break;
            case "2":
                vecTrgtAZM = new Vector3(-5.884712f, 0f, 1.170541f);
                break;
            case "3":
                vecTrgtAZM = new Vector3(-4.988818f, 0f, 3.333421f);
                break;
            case "4":
                vecTrgtAZM = new Vector3(-3.333421f, 0f, 4.988818f);
                break;
            case "5":
                vecTrgtAZM = new Vector3(-1.170542f, 0f, 5.884711f);
                break;
            case "6":
                vecTrgtAZM = new Vector3(1.170542f, 0f, 5.884711f);
                break;
            case "7":
                vecTrgtAZM = new Vector3(3.333421f, 0f, 4.988818f);
                break;
            case "8":
                vecTrgtAZM = new Vector3(4.988818f, 0f, 3.333421f);
                break;
            case "9":
                vecTrgtAZM = new Vector3(5.884712f, 0f, 1.170541f);
                break;
            case "10":
                vecTrgtAZM = new Vector3(5.884711f, 0f, -1.170542f);
                break;
        }
    }

    /* Get target and headDirection vectors in elevation plane */
    private void GetAltVectors() {
        vecLkngALT = new Vector3(0, BlY - AlY, BlZ - AlZ);

        switch (trgtinfo[2]) {
            case "1":
                vecTrgtALT = new Vector3(0f, -2.296101f, 5.543277f);
                break;
            case "2":
                vecTrgtALT = new Vector3(0f, 0f, 6f);
                break;
            case "3":
                vecTrgtALT = new Vector3(0f, 2.296101f, 5.543277f);
                break;
            case "4":
                vecTrgtALT = new Vector3(0, 4.242641f, 4.24264f);
                break;
        }
    }

    float SignedAngle(Vector3 a, Vector3 b) {
        float angle = Vector3.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.y > 0) angle = -angle;
        //print("ang: " + angle);
        return angle;
    }

    float Altitude(Vector3 a, Vector3 b) {
        float angle = Vector3.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.x < 0) angle = -angle;
        //print("ang: " + angle);
        return angle;
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

    /* Detection of points on elevation plane */
    private int AltitudetoIndex(double angle) {

        if (angle <= 56.25 && angle > 33.75) {   // 45deg
            return 0;
        }
        else if (angle <= 33.75 && angle > 11.25) {    // 22.5deg
            return 1;
        }
        else if (angle <= 11.25 && angle > -11.25) {    // 0deg
            return 2;
        }
        else if (angle <= -11.25 && angle > -33.75) {  // -22.5deg
            return 3;
        }
        else if (angle <= -33.75 && angle > -56.25) { // -45deg
            return 4;
        }
        else {
            // error
            return 333;
        }
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

    /* Trigger tactor with direction (1-8) or set all tactors off (0) */
    private void DisplayDirectionFOV(int type) {
        SendToArduino[0] = '2'; // ON 		0-2
        SendToArduino[4] = frequency; // FREQ 	0-2

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
        //print("Freq: "+ SendToArduino[4] + " Pos: " + SendToArduino[2]);

        Send();
    }

    /* Send string to vibrotactile device **/
    private void Send() {
        try {
            if (firstArduinoCall) { // first Arduino call in trial: beep, play and start counting
                _SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
                stopwatch.Start();
                firstArduinoCall = false;
            }
            else {
                _SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
            }
        }
        catch {
            UnityEngine.Debug.Log("<logApplication> <sys> ERROR Could not write in Serial Port");
        }
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
    void CloseAll() {
        UnityEngine.Debug.Log("<logApplication> ExitTime:" + System.DateTime.Now.ToString("h:mm:ss tt"));
        logFile.WriteLine("<logApplication> ExitTime:" + System.DateTime.Now.ToString("h:mm:ss tt"));
        //clean...
        DisplayDirectionFOV(0);
        //and close
        if (portopenBelt) {
            CloseConnection();
            portopenBelt = false;
        }

        logFile.Close();
    }

    /* Quit/Exit/Close */
    void OnApplicationQuit() {
        CloseAll();
    }
}
