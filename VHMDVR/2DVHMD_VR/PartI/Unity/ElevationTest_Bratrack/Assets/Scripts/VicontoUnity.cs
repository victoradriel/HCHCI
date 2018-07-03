using UnityEngine;
using System.IO.Ports;
using System.Diagnostics;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VicontoUnity : MonoBehaviour {
    int userID = 1;
    int[,] hapticmodality = new int[4, 4] { { 0, 1, 3, 2 }, { 1, 2, 0, 3 }, { 2, 3, 1, 0 }, { 3, 0, 2, 1 } };
    
    int NumberTrials = 84;  
    int[] targetdir = new int[] { 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6}; // *12 and shuffle
    float[] TargetY = new float[] { 1, 0.66f, 0.33f, 0, -0.33f, -0.66f, -1 };
    float[] TargetZ = new float[] { 1, 1, 1, 1, 1, 1, 1 };
    Text display;
    GameObject displayobj;

    public AudioClip beep;
    private AudioSource player;

    // Vicon to Unity
    Vector3 vectorNrth = new Vector3(0, 0, 1);
    Vector3 vecLkngAZM; // Vector with head direction in the azimuthal plane
    Vector3 vecTrgtAZM; // Vector with target direction in the azimuthal plane
    Vector3 vecLkngALT; // Vector with head direction in the elevation plane
    Vector3 vecTrgtALT; // Vector with target direction in the elevation plane
    public float TrgtX = 0;
    public float TrgtY = 0;
    public float TrgtZ = 1000;
    float AX = 0;
    float AY = 0;
    float AZ = 0;
    float BX = 0;
    float BY = 0;
    float BZ = 0;
    float azmTarget;
    float azmHead;
    float altTarget;
    float proxHead;
    float altHead;
    public int trialNumber = 0;
    public int sectionNumber = 0;
    Stopwatch stopwatch;
    long elapsed_time = 0;
    public GameObject HeadA;
    public GameObject HeadB;
    public GameObject Trgt;
    bool firstArduinoCall = true;

    /* Aux variables */
    int FOVindex = 0;
    int headdir = 0;
    public int altindex = 0;
    int fovnow = 0;
    int fovbefore = 0;
    int iddle = 1;
    bool trialState = true;
    char frequency = '0';
    char freqbefore = '0';
    int maxfreq = 9;
    public bool lockKey = false;

    /* Serial Port communication with the vibrotactile device */
    static string COMPortBelt = "COM3";
    static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
    static bool portopenBelt = false; //if port is open or not
    char[] SendToArduino = new char[] { '0', ',', '0', ',', '0', ',', '1', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0' };

    string[] DirectionsArray = new string[] { "45deg", "30deg", "15deg", "0deg", "-15deg", "-30deg", "-45deg" };
    string[] haptcmdrry = new string[] { "Linear Growth", "Quadratic Growth (POS)", "Quadratic Growth (NEG)", "Stair Growth" };
    public static int[] Intensity = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };

    // Use this for initialization
    void Start() {
        UnityEngine.Debug.Log("<logVicon> UserID:" + userID + ":StartTime:" + System.DateTime.Now.ToString("h:mm:ss tt"));

        stopwatch = new Stopwatch();
        HeadA = GameObject.Find("HeadA");
        HeadB = GameObject.Find("HeadB");
        //Trgt = GameObject.Find("Trgt");
        displayobj = GameObject.Find("Display");
        display = displayobj.GetComponent<Text>();
        display.text = "Session #" + sectionNumber + ": " + haptcmdrry[sectionNumber];
        UnityEngine.Debug.Log("<logVicon> Session #" + sectionNumber + ": " + haptcmdrry[sectionNumber]);

        player = GetComponent<AudioSource>();

        targetdir = MyFisherYatesShffl(targetdir);
        OpenConnectionBelt();
    }

    // Update is called once per frame
    void Update() {

        var newRotation = Input.GetAxis("Vertical") * (60);
        transform.Rotate(newRotation * Time.deltaTime,0 , 0);

       /* var newwRotation = Input.GetAxis("Horizontal") * (60);
        transform.Rotate(0, newwRotation * Time.deltaTime, 0);*/


        if (Input.GetKey(KeyCode.O)) {
            player.Stop();
            display.text = "Pink Noise is off";
        }

        if (Input.GetKey(KeyCode.P) && !lockKey) {
            player.Play();
            lockKey = true;
            display.text = "Pink Noise is on";
        }

        // Start Trial
        if (Input.GetKey(KeyCode.Space) && !lockKey) {
            lockKey = true;
            // Trigger trial
            if (!trialState && trialNumber < NumberTrials) {
                player.PlayOneShot(beep, 1); // 3000 HZ, 500ms
                display.text = "Triggering stimulus #" + trialNumber;
                trialState = true;
                stopwatch.Reset();
            }
            else {
                player.Stop();
                display.text = "Noise is off. No more stimuli for now";
            }
        }

        if (Input.GetKey(KeyCode.KeypadEnter)) {
            lockKey = false;
            // Answer
            if (trialState) {
                stopwatch.Stop();
                elapsed_time = stopwatch.ElapsedMilliseconds;
                UnityEngine.Debug.Log("<logVicon> <answer> ReactionTime: " + elapsed_time);
                //display.text = "Time: " + elapsed_time;

                verifyAnswer();
                setIddleState();
                trialState = false;

                trialNumber++;
            }
        }

        if (Input.GetKey(KeyCode.Z) && trialNumber > 0 && !lockKey) {
            lockKey = true;
            // Trial Anterior
            trialNumber--;
            display.text = "Ready to trigger stimulus #" + trialNumber;
            UnityEngine.Debug.Log("<logVicon> Return to stimulus #" + trialNumber);

            if (trialState) {
                stopwatch.Stop();
                DisplayDirectionFOV(0);
                trialState = false;
                firstArduinoCall = true;
            }
        }

        if (Input.GetKey(KeyCode.X) && trialNumber < (targetdir.Length) - 1 && !lockKey) {
            lockKey = true;
            // Trial seguinte
            trialNumber++;
            display.text = "Ready to trigger stimulus #" + trialNumber;
            UnityEngine.Debug.Log("<logVicon> Jump to stimulus #" + trialNumber);

            if (trialState) {
                stopwatch.Stop();
                DisplayDirectionFOV(0);
                trialState = false;
                firstArduinoCall = true;
            }
        }

        // SESSION INPUT
        if (Input.GetKey(KeyCode.S)) {
            // Start session
            SetStartSession();
            display.text = "Play Session #" + sectionNumber + ": " + haptcmdrry[sectionNumber];
            UnityEngine.Debug.Log("<logVicon> RestartingSession #" + sectionNumber + ": " + haptcmdrry[sectionNumber]);

            player.Play();
        }

        if (Input.GetKey(KeyCode.Q) && sectionNumber > 0 && !lockKey) {
            // Sessao Anterior
            sectionNumber--;
            lockKey = true;

            SetStartSession();
            display.text = "Restarting Session #" + sectionNumber + ": " + haptcmdrry[sectionNumber];
            UnityEngine.Debug.Log("<logVicon> RestartingSession #" + sectionNumber + ": " + haptcmdrry[sectionNumber]);
        }

        if (Input.GetKey(KeyCode.W) && sectionNumber < 3 && !lockKey) {
            sectionNumber++;
            lockKey = true;

            SetStartSession();
            display.text = "Session #" + sectionNumber + ": " + haptcmdrry[sectionNumber];
            UnityEngine.Debug.Log("<logVicon> Session #" + sectionNumber + ": " + haptcmdrry[sectionNumber]);
        }

        if (Input.GetKey(KeyCode.K)) {
            lockKey = false;
        }

        if (trialState == true) {
            GetAB();
            GetTrgt(); // precisar passar a pegar os taregts predefinidos

            GetAzmVectors();
            GetAltVectors();

            proxHead = Altitude(vecLkngALT, vecTrgtALT);
            altHead = Altitude(vecLkngALT, vectorNrth);

            switch (hapticmodality[userID - 1, sectionNumber]) {
                case 0:
                    GetFrequency("Linear", proxHead);
                    GetPointDirFOV(AskHeadDirection("Default"));
                    break;
                case 1:
                    GetFrequency("QuadPOS", proxHead);
                    GetPointDirFOV(AskHeadDirection("Default"));
                    break;
                case 2:
                    GetFrequency("QuadNEG", proxHead);
                    GetPointDirFOV(AskHeadDirection("Default"));
                    break;
                case 3:
                    GetFrequency("Stair", proxHead);
                    GetPointDirFOV(AskHeadDirection("Default"));
                    break;
            }
            
            ProximityToTarget();
        }
    }

    public static int[] MyFisherYatesShffl(int[] IntArray) {
        //Fisher–Yates Shuffle variation
        int ArrayLength = IntArray.Length;

        for (int i = 0; i < ArrayLength; i++) {
            int temp = IntArray[i];
            int randomIndex = UnityEngine.Random.Range(i, ArrayLength - 1);
            if ((i > 0) && (i < ArrayLength - 2)) {
                if (IntArray[i - 1] == IntArray[randomIndex])
                    randomIndex++;
            }
            IntArray[i] = IntArray[randomIndex];
            IntArray[randomIndex] = temp;
        }
        return IntArray;
    }

    void SetStartSession() {
        trialNumber = 0;
        targetdir = MyFisherYatesShffl(targetdir);
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

        firstArduinoCall = true;
    }

    void verifyAnswer() {
        altindex = AltitudetoIndex(altHead);

        if (targetdir[trialNumber] == altindex) {
            UnityEngine.Debug.Log("<logVicon> <answer> Right: " + DirectionsArray[altindex]);
            UnityEngine.Debug.Log("<logVicon> <answer> Precision: " + proxHead);
            display.text = "Right Answer: " + DirectionsArray[altindex] + " Precision:" + System.Math.Round(proxHead, 2);
        }
        else {
            UnityEngine.Debug.Log("<logVicon> <answer> Wrong: " + DirectionsArray[altindex] + ". It should be: " + DirectionsArray[targetdir[trialNumber]]);
            UnityEngine.Debug.Log("<logVicon> <answer> Precision: " + proxHead);
            display.text = "Wrong Answer: " + DirectionsArray[altindex] + ". It should be: " + DirectionsArray[targetdir[trialNumber]] + " Error:" + System.Math.Round(proxHead, 2);
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
    }

    /* Get virtual target */
    void GetTrgt() {
        /*TrgtX = Trgt.transform.position.x;
        TrgtY = Trgt.transform.position.y;
        TrgtZ = Trgt.transform.position.z;*/

        TrgtX = 0;
        TrgtY = TargetY[targetdir[trialNumber]];
        TrgtZ = TargetZ[targetdir[trialNumber]];
    }

    /* Get target and headDirection vectors in azimuthal plane */
    private void GetAzmVectors() {
        vecTrgtAZM = new Vector3(TrgtX - AX, 0, TrgtZ - AZ);
        vecLkngAZM = new Vector3(BX - AX, 0, BZ - AZ);
    }

    /* Get target and headDirection vectors in elevation plane */
    private void GetAltVectors() {
        vecTrgtALT = new Vector3(0, TrgtY - AY, TrgtZ - AZ);
        vecLkngALT = new Vector3(0, BY - AY, BZ - AZ);
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
            if (motorIndx == 1) { fovnow = 1; if (fovbefore != fovnow || freqbefore != frequency) { DisplayDirectionFOV(7); fovbefore = fovnow; freqbefore = frequency; headdir = inputdir; iddle = 0;}}     // West
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

        if (angle > 37.5) {                         // 45deg
            return 0;
        }
        else if (angle <= 37.5 && angle > 22.5) {   // 30deg
            return 1;
        }
        else if (angle <= 22.5 && angle > 7.5) {    // 15deg
            return 2;
        }
        else if (angle <= 7.5 && angle > -7.5) {    // 0deg
            return 3;
        }
        else if (angle <= -7.5 && angle > -22.5) {  // -15deg
            return 4;
        }
        else if (angle <= -22.5 && angle > -37.5) { // -30deg
            return 5;
        }
        else if (angle <= -37.5) {                  // -45deg
            return 6;
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
        else if (angle >= -2 && angle < 2) {      // NORTH
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
            UnityEngine.Debug.Log("<logVicon> <sys> ERROR Could not write in Serial Port");
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
            }
            else {
                _SerialPortBelt.Open();
                portopenBelt = true;
            }
        }
        else {
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
        UnityEngine.Debug.Log("<logVicon> ExitTime:" + System.DateTime.Now.ToString("h:mm:ss tt"));
        //clean...
        DisplayDirectionFOV(0);
        //and close
        if (portopenBelt) {
            CloseConnection();
            portopenBelt = false;
        }
    }
}
