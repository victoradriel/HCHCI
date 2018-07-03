using System;
using System.Windows;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViconDataStreamSDK.DotNET;
using System.Diagnostics;

namespace CSharpClient{
    class Program{

        // TRIAL / Spacebar: Trigger / ZX: Jump | SESSION / S: Start / QW: Jump | SOUND / O: Play / P: Pause | ESC LEAVES
        int userID = 3;
        int[,] hapticmodality = new int[3,3] { { 0, 1, 2 }, { 1, 2, 0 }, { 2, 0, 1 }};

        int NumberTrials = 50;  // 50 or 40
        int[] targetdir = new int[] { 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4 }; // *10 or *8 and shuffle
        double[] TargetX = new double[] { -1, -1, 0, 1, 1 };
        double[] TargetY = new double[] { 0, 1, 1, 1, 0 };
        string logPath = @"C:\Users\victo\Documents\LogFileVicon.txt";
        
        static string COMPortBelt = "COM3";
        static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
        static bool portopenBelt = false; //if port is open or not
        char[] SendToArduino = new char[] { '0', ',', '0', ',', '0', ',', '1', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0' };
        string[] DirectionsArray = new string[] { "W", "NW", "N", "NE", "E" };
        string[] haptcmdrry = new string[] { "Pointing (45)", "Pointing (15)", "Pointing (Tactile Fovea)" };
        
        int trialNumber = 0;
        int sectionNumber = 0;
        int headdir = 0;
        int FOVindex = 0;

        int iddle = 1;
        int fovnow = 0;
        int fovbefore = 0;
        int bitnow = 0;
        int bitbefore = 0;
        int dirnow = 0;
        int dirbefore = 0;
        bool firstArduinoCall = true;

        double AX = 0;
        double AY = 0;
        double BX = 0;
        double BY = 0;
        double angleHead;
        double angleTarget;
        Vector vectorLkng;
        Vector vectorTrgt;
        Vector vectorNrth = new Vector(0, 1);
        Stopwatch stopwatch = new Stopwatch();

        static string Adapt(Direction i_Direction){
            switch (i_Direction){
                case Direction.Forward:
                    return "Forward";
                case Direction.Backward:
                    return "Backward";
                case Direction.Left:
                    return "Left";
                case Direction.Right:
                    return "Right";
                case Direction.Up:
                    return "Up";
                case Direction.Down:
                    return "Down";
                default:
                    return "Unknown";
            }
        }

        static string Adapt(DeviceType i_DeviceType){
            switch (i_DeviceType){
                case DeviceType.ForcePlate:
                    return "ForcePlate";
                case DeviceType.Unknown:
                default:
                    return "Unknown";
            }
        }

        static string Adapt(Unit i_Unit){
            switch (i_Unit){
                case Unit.Meter:
                    return "Meter";
                case Unit.Volt:
                    return "Volt";
                case Unit.NewtonMeter:
                    return "NewtonMeter";
                case Unit.Newton:
                    return "Newton";
                case Unit.Kilogram:
                    return "Kilogram";
                case Unit.Second:
                    return "Second";
                case Unit.Ampere:
                    return "Ampere";
                case Unit.Kelvin:
                    return "Kelvin";
                case Unit.Mole:
                    return "Mole";
                case Unit.Candela:
                    return "Candela";
                case Unit.Radian:
                    return "Radian";
                case Unit.Steradian:
                    return "Steradian";
                case Unit.MeterSquared:
                    return "MeterSquared";
                case Unit.MeterCubed:
                    return "MeterCubed";
                case Unit.MeterPerSecond:
                    return "MeterPerSecond";
                case Unit.MeterPerSecondSquared:
                    return "MeterPerSecondSquared";
                case Unit.RadianPerSecond:
                    return "RadianPerSecond";
                case Unit.RadianPerSecondSquared:
                    return "RadianPerSecondSquared";
                case Unit.Hertz:
                    return "Hertz";
                case Unit.Joule:
                    return "Joule";
                case Unit.Watt:
                    return "Watt";
                case Unit.Pascal:
                    return "Pascal";
                case Unit.Lumen:
                    return "Lumen";
                case Unit.Lux:
                    return "Lux";
                case Unit.Coulomb:
                    return "Coulomb";
                case Unit.Ohm:
                    return "Ohm";
                case Unit.Farad:
                    return "Farad";
                case Unit.Weber:
                    return "Weber";
                case Unit.Tesla:
                    return "Tesla";
                case Unit.Henry:
                    return "Henry";
                case Unit.Siemens:
                    return "Siemens";
                case Unit.Becquerel:
                    return "Becquerel";
                case Unit.Gray:
                    return "Gray";
                case Unit.Sievert:
                    return "Sievert";
                case Unit.Katal:
                    return "Katal";

                case Unit.Unknown:
                default:
                    return "Unknown";
            }
        }

        public static void OpenConnectionBelt() {
            if (_SerialPortBelt != null){
                if (_SerialPortBelt.IsOpen){
                    _SerialPortBelt.Close();
                    Console.WriteLine("<logVicon> <sys> Closing port, because it was already open!");
                } else {
                    //very important!, this opens the connection
                    _SerialPortBelt.Open();
                    //sets the timeout value (how long it takes before timeout error occurs)
                    //zet de timeout niet te hoog, dit vertraagd Unity enorm en kan zelfs voor crashes zorgen.
                    //_SerialPortBelt.ReadTimeout = 1000;
                    //_SerialPortBelt.WriteTimeout = 50;
                    portopenBelt = true;
                }
            } else {
                if (_SerialPortBelt.IsOpen) { Console.WriteLine("<logVicon> <sys> Port is already open"); }
                else { Console.WriteLine("<logVicon> <sys> Port == null"); }
            }
        }

        public static void CloseConnection(){
            if (portopenBelt == true){
                try{
                    _SerialPortBelt.Close();
                    Console.WriteLine("<logVicon> <sys> Closing port: {0}", COMPortBelt);
                } catch{
                    Console.WriteLine("<logVicon> <sys> ERROR in closing {0}", COMPortBelt);
                }
            }
        }

        public static int[] MyFisherYatesShffl(int[] IntArray) {
            //Fisher–Yates Shuffle variation
            int ArrayLength = IntArray.Length;
            Random rnd = new Random();

            for (int i = 0; i < ArrayLength; i++) {
                int temp = IntArray[i];
                int randomIndex = rnd.Next(i);
                //int randomIndex = UnityEngine.Random.Range(i, ArrayLength - 1);
                if ((i > 0) && (i < ArrayLength - 2)) {
                    if (IntArray[i - 1] == IntArray[randomIndex])
                        randomIndex++;
                }
                IntArray[i] = IntArray[randomIndex];
                IntArray[randomIndex] = temp;
            }
            return IntArray;
        }

        private void Send(){
            try{
                if (firstArduinoCall) { // first Arduino call in trial: beep, play and start counting
                    _SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
                    stopwatch.Start();
                    firstArduinoCall = false;
                }
                else {
                    _SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
                }
                
                LogRecord(logPath, "<logVicon> <sys> Submitted to Arduino");
            } catch{
                Console.WriteLine("<logVicon> <sys> ERROR Could not write in Serial Port");
                LogRecord(logPath, "<logVicon> <sys> ERROR Could not write in Serial Port");
            }
        }

        private void DisplayAlert()
        {
            SendToArduino[0] = '1';     // ON               0-1
            SendToArduino[2] = '8';     // MOTOR            1-8
            SendToArduino[4] = '0';     // FREQ             0-2
            SendToArduino[6] = '3';     // QtdBURST         1-n
            SendToArduino[8] = '2';     // BURSTDur         0-2
            SendToArduino[10] = '2';    // INTERBURSTDur    0-2

            Send();
        }

        private void DisplayDirection(int type){

            SendToArduino[0] = '2'; // ON 		0-2
            SendToArduino[4] = '0'; // FREQ 	0-2

            switch (type){
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
                    SendToArduino[2] = '4'; // MOTOR 	1-8
                    iddle = 0;
                    break;
                case 4:
                    SendToArduino[2] = '6'; // MOTOR 	1-8
                    iddle = 0;
                    break;
                case 5:
                    SendToArduino[2] = '7'; // MOTOR 	1-8
                    iddle = 0;
                    break;
                case 6:
                    SendToArduino[2] = '8'; // MOTOR 	1-8
                    iddle = 0;
                    break;
            }

            Send();
        }

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
                    SendToArduino[2] = '4'; // MOTOR 	1-8
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

        private void DisplayIntensity(int type, string mode) {
            char[] intensityArray = new char[] { '3', '2', '1', '0' };

            switch (mode) {
                case "Up":
                    intensityArray = new char[] {'0', '1', '2', '3'};
                    break;
                case "Down":
                    intensityArray = new char[] {'3', '2', '1', '0'};
                    break;
                default:
                    Console.WriteLine("ERROR: There is no mode called " + mode + " in DisplayIntensity");
                    break;
            }

            SendToArduino[0] = '2'; // ON 		0-2
            SendToArduino[4] = '0'; // FREQ 	0-2
            SendToArduino[2] = '4'; // MOTOR 	1-8

            switch (type) {
                case 0:
                    SendToArduino[0] = '0'; // ON 		0-2
                    SendToArduino[2] = '0'; // MOTOR 	1-8
                    break;
                case 1:
                    SendToArduino[4] = intensityArray[0]; // FREQ 	0-2
                    iddle = 0;
                    break;
                case 2:
                    SendToArduino[4] = intensityArray[1]; // FREQ 	0-2
                    iddle = 0;
                    break;
                case 3:
                    SendToArduino[4] = intensityArray[2]; // FREQ 	0-2
                    iddle = 0;
                    break;
                case 4:
                    SendToArduino[4] = intensityArray[3]; // FREQ 	0-2
                    iddle = 0;
                    break;
                case 5:
                    SendToArduino[4] = intensityArray[2]; // FREQ 	0-2
                    iddle = 0;
                    break;
                case 6:
                    SendToArduino[4] = intensityArray[1]; // FREQ 	0-2
                    iddle = 0;
                    break;
                case 7:
                    SendToArduino[4] = intensityArray[0]; // FREQ 	0-2
                    iddle = 0;
                    break;
                case 8:
                    SendToArduino[4] = '0'; // FREQ 	0-2
                    iddle = 0;
                    break;
            }

            Send();
        }

        private void GetPointDir(int inputdir) {
            int motorIndx = targetdir[trialNumber] - inputdir;
            if (motorIndx >= -2 && motorIndx <= 2){
                dirnow = motorIndx + 3;
                if (dirbefore != dirnow) {
                    
                    DisplayDirection(dirnow);
                    dirbefore = dirnow;
                    headdir = inputdir;
                    iddle = 0;
                }
                else {
                    // calling again
                }
            } else {
                dirnow = 0;
                if (dirbefore != dirnow) {
                    DisplayDirection(0);
                    dirbefore = dirnow;
                }
            }

        }

        private void GetPointDirFOV(int inputdir) {
            double angle = ProximityToTarget();
            int motorIndx = AngletoIndex(angle);

            if (inputdir >= 0 && inputdir <= 4) {
                motorIndx++;
                if (motorIndx == 1) { fovnow = 1; if (fovbefore != fovnow) { DisplayDirectionFOV(7); fovbefore = fovnow; headdir = inputdir; iddle = 0; } } 	// West
                if (motorIndx == 2) { fovnow = 2; if (fovbefore != fovnow) { DisplayDirectionFOV(6); fovbefore = fovnow; headdir = inputdir; iddle = 0; } } 	// NW
                if (motorIndx == 3) {
                    if (FOVindex == -1) { fovnow = 3; if (fovbefore != fovnow) { DisplayDirectionFOV(5); fovbefore = fovnow; headdir = inputdir; iddle = 0; } } // Nleft
                    if (FOVindex == 0) { fovnow = 4; if (fovbefore != fovnow) { DisplayDirectionFOV(4); fovbefore = fovnow; headdir = inputdir; iddle = 0; } }  // NORTH
                    if (FOVindex == 1) { fovnow = 5; if (fovbefore != fovnow) { DisplayDirectionFOV(3); fovbefore = fovnow; headdir = inputdir; iddle = 0; } } 	// Nright
                }
                if (motorIndx == 4) { fovnow = 6; if (fovbefore != fovnow) { DisplayDirectionFOV(2); fovbefore = fovnow; headdir = inputdir; iddle = 0; } } 	// NE
                if (motorIndx == 5) { fovnow = 7; if (fovbefore != fovnow) { DisplayDirectionFOV(1); fovbefore = fovnow; headdir = inputdir; iddle = 0; } } 	// East
            }
            else {
                fovnow = 0;
                if (fovbefore != fovnow) {
                    DisplayDirection(0);
                    fovbefore = fovnow;
                }
            }
        }

        private void GetOnePointIntensity(int inputdir, string mode) {
            double angle = ProximityToTarget();
            int motorIndx = AngletoIndex(angle);

            if (inputdir >= 0 && inputdir <= 4) {
                motorIndx++;
                if (motorIndx == 1) { bitnow = 1; if (bitbefore != bitnow) { DisplayIntensity(7, mode); bitbefore = bitnow; headdir = inputdir; iddle = 0; } } 	// West
                if (motorIndx == 2) { bitnow = 2; if (bitbefore != bitnow) { DisplayIntensity(6, mode); bitbefore = bitnow; headdir = inputdir; iddle = 0; } } 	// NW
                if (motorIndx == 3) {
                    if (FOVindex == -1) { bitnow = 3; if (bitbefore != bitnow) { DisplayIntensity(5, mode); bitbefore = bitnow; headdir = inputdir; iddle = 0; } } // Nleft
                    if (FOVindex == 0) { bitnow = 4; if (bitbefore != bitnow) { DisplayIntensity(4, mode); bitbefore = bitnow; headdir = inputdir; iddle = 0; } }  // NORTH
                    if (FOVindex == 1) { bitnow = 5; if (bitbefore != bitnow) { DisplayIntensity(3, mode); bitbefore = bitnow; headdir = inputdir; iddle = 0; } } 	// Nright
                }
                if (motorIndx == 4) { bitnow = 6; if (bitbefore != bitnow) { DisplayIntensity(2, mode); bitbefore = bitnow; headdir = inputdir; iddle = 0; } } 	// NE
                if (motorIndx == 5) { bitnow = 7; if (bitbefore != bitnow) { DisplayIntensity(1, mode); bitbefore = bitnow; headdir = inputdir; iddle = 0; } } 	// East
            }
            else {
                bitnow = 0;
                if (bitbefore != bitnow) {
                    DisplayDirection(0);
                    bitbefore = bitnow;
                }
            }
        }

        private int AngletoIndex(double angle) {
            if (angle >= -112.5 && angle < -67.5) {          // WEST
                return 0;
            } else if (angle >= -67.5 && angle < -22.5) {    // NW
                return 1;
            } else if (angle >= -22.5 && angle < -7.5) {    // NORTH (left)
                FOVindex = -1;
                return 2;
            } else if (angle >= -7.5 && angle < 7.5) {      // NORTH
                FOVindex = 0;
                return 2;
            } else if (angle >= 7.5 && angle < 22.5) {      // NORTH (right)
                FOVindex = 1;
                return 2;
            } else if (angle >= 22.5 && angle < 67.5) {     // NE
                return 3;
            } else if (angle >= 67.5 && angle < 112.5) {    // EAST;
                return 4;
            } else {                                        // error
                //Console.WriteLine("errorr angle");
                return 333; 
            }
            
        }

        private int ThinAngletoIndex(double angle) {
            if (angle >= -97.5 && angle < -82.5) {      // WEST
                return 0;
            }
            else if (angle >= -52.5 && angle < -37.5) { // NW
                return 1;
            }
            else if (angle >= -7.5 && angle < 7.5) {    // NORTH
                FOVindex = 0;
                return 2;
            }
            else if (angle >= 37.5 && angle < 52.5) {   // NE
                return 3;
            }
            else if (angle >= 82.5 && angle < 97.5) {   // EAST
                return 4;
            }
            else {                                      // error
                return 333;
            }
        }

        private void GetVectors() {
            vectorTrgt = new Vector(TargetX[targetdir[trialNumber]], TargetY[targetdir[trialNumber]]);
            vectorLkng = new Vector(BX - AX, BY - AY);
        }

        private double ProximityToTarget() {
            angleTarget = Vector.AngleBetween(vectorLkng, vectorTrgt);

            //Console.WriteLine("<logVicon> <sys> Proximity: " + angleTarget + " indexTarget: " + targetdir[trialNumber] + " idx " + targetdir[trialNumber] + " X: "+ TargetX[targetdir[trialNumber]] + " Y: " + TargetY[targetdir[trialNumber]]);
            //LogRecord(logPath, "<logVicon> <sys> ProximitytoTarget: " + angleTarget);

            return angleTarget;
        }

        private int AskHeadDirection(string mode) {
            angleHead = Vector.AngleBetween(vectorLkng, vectorNrth);

            //Console.WriteLine("<logVicon> <sys> Head direction: " + DirectionsArray[AngletoIndex(angleHead)]);
            //LogRecord(logPath, "<logVicon> <sys> Head direction: " + DirectionsArray[AngletoIndex(angleHead)]);

            if (mode == "Thinner") { return ThinAngletoIndex(angleHead); }
            else { return AngletoIndex(angleHead); }
        }

        private void HapticFeedback(int inputdir) {
            switch (hapticmodality[userID - 1, sectionNumber]) {
                case 0:
                    GetPointDir(inputdir); // W NW N NE E
                    break;
                case 1:
                    GetPointDirFOV(inputdir); // W NW Nl N Nr NE E (with Tactile Fovea)
                    break;
                case 2:
                    GetOnePointIntensity(inputdir, "Down"); // back motor - less vibration towards target
                    break;
                case 3:
                    GetOnePointIntensity(inputdir, "Up"); // back motor - more vibration towards target
                    break;
            }
        }

        private void CreateLogFile(string path) {
            if (!File.Exists(path)) {
                File.Create(path);
            } else if (File.Exists(path)) {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine("<logVicon> Trying to create new log for UserID_"+ userID +" at: " + DateTime.Now.ToString("h:mm:ss tt"));
                tw.Close();
            }
        }

        private void LogRecord(string path, string logString) {
            if (File.Exists(path)) {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(logString);
                tw.Close();
            } else {
                Console.WriteLine("<logVicon> <sys> Trying to record log but file in "+ path + " does not exist!");
            }
        }

        private void verifyAnswer() {
            if (targetdir[trialNumber] == headdir) {
                LogRecord(logPath, "<logVicon> <answer> Right: " + DirectionsArray[headdir]);
                LogRecord(logPath, "<logVicon> <answer> Precision: " + ProximityToTarget());
                Console.WriteLine("<logVicon> Right Answer: " + DirectionsArray[headdir] + " angle:" + ProximityToTarget());
            }
            else {
                LogRecord(logPath, "<logVicon> <answer> Wrong: " + DirectionsArray[headdir] + ". It should be: " + DirectionsArray[targetdir[trialNumber]]);
                LogRecord(logPath, "<logVicon> <answer> Precision: " + ProximityToTarget());
                Console.WriteLine("<logVicon> Wrong Answer: " + DirectionsArray[headdir] + ". It should be: " + DirectionsArray[targetdir[trialNumber]] + " angle:" + ProximityToTarget());
            }
        }

        private void verifyAB() {
            double verify = BY - AY;
            double buffX = 0;
            double buffY = 0;

            if (verify < 0) {
                // if Y is negative, invert A and B
                buffX = AX;
                buffY = AY;
                AX = BX;
                AY = BY;
                BX = buffX;
                BY = buffY;
            }
        }

        private void setIddleState() {
            DisplayDirection(0);

            dirbefore = 0;
            fovbefore = 0;
            bitbefore = 0;

            firstArduinoCall = true;
        }

        private void SetStartSession() {
            trialNumber = 0;
            targetdir = MyFisherYatesShffl(targetdir);
        }

        static void Main(string[] args){
            // Program options
            bool TransmitMulticast = false;
            bool EnableHapticTest = false;
            bool bReadCentroids = false;
            bool trialState = false;
            int usingLabels = 0;
            int existA = 0;
            int existB = 0;
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C: \Users\victo\Documents\PinkNoise.wav");
            player.Load();

            List<String> HapticOnList = new List<String>();
            int Counter = 1;
            Program thisp = new Program();
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            
            long elapsed_time = 0;

            //////////////////////////////////////////////////////////////////////////////////////////////
            thisp.CreateLogFile(thisp.logPath);
            thisp.LogRecord(thisp.logPath, "<logVicon> StartTime " + DateTime.Now.ToString("h:mm:ss tt"));
            thisp.targetdir = MyFisherYatesShffl(thisp.targetdir);

            OpenConnectionBelt();
            //////////////////////////////////////////////////////////////////////////////////////////////

            string HostName = "localhost:801";
            if (args.Length > 0){
                HostName = args[0];
            }

            // Make a new client
            ViconDataStreamSDK.DotNET.Client MyClient = new ViconDataStreamSDK.DotNET.Client();

            // Connect to a server
            Console.Write("Connecting to {0} ...", HostName);
            while (!MyClient.IsConnected().Connected) {
                // Direct connection
                MyClient.Connect(HostName);

                // Multicast connection
                // MyClient.ConnectToMulticast( HostName, "224.0.0.0" );

                System.Threading.Thread.Sleep(200);
                Console.Write(".");

                try { if (Console.KeyAvailable) { } }
                catch (InvalidOperationException) { }
            }
            Console.WriteLine();
            Console.WriteLine("Session #" + thisp.sectionNumber + ": " + thisp.haptcmdrry[thisp.sectionNumber]);
            thisp.LogRecord(thisp.logPath, "<logVicon> Session #" + thisp.sectionNumber + ": " + thisp.haptcmdrry[thisp.sectionNumber]);
            Console.WriteLine();

            // Enable some different data types
            MyClient.EnableSegmentData();
            MyClient.EnableMarkerData();
            MyClient.EnableUnlabeledMarkerData();
            MyClient.EnableDeviceData();
            if (bReadCentroids) {
                MyClient.EnableCentroidData();
            }

            // Set the streaming mode
            MyClient.SetStreamMode(ViconDataStreamSDK.DotNET.StreamMode.ClientPull);

            // Set the global up axis
            MyClient.SetAxisMapping(ViconDataStreamSDK.DotNET.Direction.Forward,ViconDataStreamSDK.DotNET.Direction.Left,ViconDataStreamSDK.DotNET.Direction.Up); 

            if (TransmitMulticast) {
                MyClient.StartTransmittingMulticast("localhost", "224.0.0.0");
            }

            // Loop until a key is pressed
            while (true){
                ++Counter;
                // Console.KeyAvailable throws an exception if stdin is a pipe (e.g.
                // with TrackerDssdkTests.py), so we use try..catch:
                try{
                    if (Console.KeyAvailable){
                        cki = Console.ReadKey(true);
                        //Console.WriteLine("You pressed the '{0}' key.", cki.Key);

                        if (cki.Key == ConsoleKey.Escape) {
                            // Exit
                            thisp.LogRecord(thisp.logPath, "<logVicon> ExitTime " + DateTime.Now.ToString("h: mm:ss tt"));
                            thisp.firstArduinoCall = false;
                            thisp.DisplayDirection(0);

                            CloseConnection();
                            portopenBelt = false;
                            break;
                        }

                        // TRIAL INPUT

                        if (cki.Key == ConsoleKey.Spacebar) {
                            // Trigger trial
                            if (!trialState && thisp.trialNumber < thisp.NumberTrials) {
                                Console.Beep(3000, 500);
                                Console.WriteLine("Triggering stimulus #" + thisp.trialNumber);
                                trialState = true;
                                thisp.stopwatch.Reset();   
                            }
                            else {
                                player.Stop();
                                Console.WriteLine("Pink Noise is off");

                                Console.WriteLine("No more stimuli in this session");
                            }
                        }

                        if (cki.Key == ConsoleKey.O) {
                            player.Stop();
                            Console.WriteLine("Pink Noise is off");
                        }

                        if (cki.Key == ConsoleKey.P) {
                            player.Play();
                            Console.WriteLine("Pink Noise is on");
                        }


                        if (cki.Key == ConsoleKey.Enter) {
                            // Answer
                            if (trialState) {
                                thisp.stopwatch.Stop();
                                elapsed_time = thisp.stopwatch.ElapsedMilliseconds;
                                //Console.WriteLine("Time: " + elapsed_time);
                                thisp.LogRecord(thisp.logPath, "<logVicon> <answer> ReactionTime: " + elapsed_time);

                                thisp.verifyAnswer();
                                thisp.setIddleState();
                                trialState = false;

                                thisp.trialNumber++;
                                Console.WriteLine();
                            }
                        }

                        if (cki.Key == ConsoleKey.Z && thisp.trialNumber > 0) {
                            // Trial Anterior
                            thisp.trialNumber--;
                            Console.WriteLine("Ready to trigger stimulus #" + thisp.trialNumber);
                            thisp.LogRecord(thisp.logPath, "<logVicon> Return to stimulus #" + thisp.trialNumber);

                            if (trialState) {
                                thisp.stopwatch.Stop();
                                thisp.DisplayDirection(0);
                                trialState = false;
                                thisp.firstArduinoCall = true;
                            }
                        }

                        if (cki.Key == ConsoleKey.X && thisp.trialNumber < (thisp.targetdir.Length) - 1) {
                            // Trial seguinte
                            thisp.trialNumber++;
                            Console.WriteLine("Ready to trigger stimulus #" + thisp.trialNumber);
                            thisp.LogRecord(thisp.logPath, "<logVicon> Jump to stimulus #" + thisp.trialNumber);

                            if (trialState) {
                                thisp.stopwatch.Stop();
                                thisp.DisplayDirection(0);
                                trialState = false;
                                thisp.firstArduinoCall = true;
                            }
                        }

                        // SESSION INPUT

                        if (cki.Key == ConsoleKey.S) {
                            // Start session
                            thisp.SetStartSession();
                            Console.WriteLine("Play Session #" + thisp.sectionNumber + ": " + thisp.haptcmdrry[thisp.sectionNumber]);
                            thisp.LogRecord(thisp.logPath, "<logVicon> RestartingSession #" + thisp.sectionNumber + ": " + thisp.haptcmdrry[thisp.sectionNumber]);

                            player.Play();
                            Console.WriteLine("Pink Noise is on");
                        }

                        if (cki.Key == ConsoleKey.Q && thisp.sectionNumber > 0) {
                            // Sessao Anterior
                            thisp.sectionNumber--;

                            thisp.SetStartSession();
                            Console.WriteLine("Restarting Session #" + thisp.sectionNumber + ": " + thisp.haptcmdrry[thisp.sectionNumber]);
                            thisp.LogRecord(thisp.logPath, "<logVicon> RestartingSession #" + thisp.sectionNumber + ": " + thisp.haptcmdrry[thisp.sectionNumber]);
                        }

                        if (cki.Key == ConsoleKey.W && thisp.sectionNumber < 2) { // (thisp.hapticmodality.Length) - 1 = 2
                            // Proxima Sessao
                            thisp.sectionNumber++;

                            thisp.SetStartSession();
                            Console.WriteLine("Session #" + thisp.sectionNumber + ": " + thisp.haptcmdrry[thisp.sectionNumber]);
                            thisp.LogRecord(thisp.logPath, "<logVicon> Session #" + thisp.sectionNumber + ": " + thisp.haptcmdrry[thisp.sectionNumber]);
                        }
                    }
                }
                catch (InvalidOperationException){}

                // Get a frame
                //Console.Write("Waiting for new frame...");
                while (MyClient.GetFrame().Result != ViconDataStreamSDK.DotNET.Result.Success){
                    System.Threading.Thread.Sleep(200);
                    //Console.Write(".");
                }
                //Console.WriteLine();

                Output_GetFrameNumber _Output_GetFrameNumber = MyClient.GetFrameNumber();
                //Console.WriteLine("Frame Number: {0}", _Output_GetFrameNumber.FrameNumber);
                Output_GetFrameRate _Output_GetFrameRate = MyClient.GetFrameRate();
                //Console.WriteLine("Frame rate: {0}", _Output_GetFrameRate.FrameRateHz);

                // Count the number of subjects
                uint SubjectCount = MyClient.GetSubjectCount().SubjectCount;
                if (SubjectCount > 0) usingLabels = 1;
                existA = 0;
                existB = 0;

                for (uint SubjectIndex = 0; SubjectIndex < SubjectCount; ++SubjectIndex) {

                    string SubjectName = MyClient.GetSubjectName(SubjectIndex).SubjectName;
                    string RootSegment = MyClient.GetSubjectRootSegmentName(SubjectName).SegmentName;           

                    uint MarkerCount = MyClient.GetMarkerCount(SubjectName).MarkerCount;

                    for (uint MarkerIndex = 0; MarkerIndex < MarkerCount; ++MarkerIndex) {

                        string MarkerName = MyClient.GetMarkerName(SubjectName, MarkerIndex).MarkerName;
                        string MarkerParentName = MyClient.GetMarkerParentName(SubjectName, MarkerName).SegmentName;
                        Output_GetMarkerGlobalTranslation _Output_GetMarkerGlobalTranslation = MyClient.GetMarkerGlobalTranslation(SubjectName, MarkerName);
                        //Console.WriteLine("      Marker #{0}: {1} ({2}, {3}, {4}) {5}",MarkerIndex,MarkerName, _Output_GetMarkerGlobalTranslation.Translation[0],_Output_GetMarkerGlobalTranslation.Translation[1], _Output_GetMarkerGlobalTranslation.Translation[2],_Output_GetMarkerGlobalTranslation.Occluded);

                        if (MarkerName == "A") {
                            thisp.AX = _Output_GetMarkerGlobalTranslation.Translation[0];
                            thisp.AY = _Output_GetMarkerGlobalTranslation.Translation[1];
                            existA = 1;
                        }

                        if (MarkerName == "B") {
                            thisp.BX = _Output_GetMarkerGlobalTranslation.Translation[0];
                            thisp.BY = _Output_GetMarkerGlobalTranslation.Translation[1];
                            existB = 1;
                        }
                    }
                }

                // Get the unlabeled markers
                uint UnlabeledMarkerCount = MyClient.GetUnlabeledMarkerCount().MarkerCount;
                //Console.WriteLine("    Unlabeled Markers ({0}):", UnlabeledMarkerCount);
                for (uint UnlabeledMarkerIndex = 0; UnlabeledMarkerIndex < UnlabeledMarkerCount; ++UnlabeledMarkerIndex){
                    // Get the global marker translation
                    Output_GetUnlabeledMarkerGlobalTranslation _Output_GetUnlabeledMarkerGlobalTranslation = MyClient.GetUnlabeledMarkerGlobalTranslation(UnlabeledMarkerIndex);
                    //Console.WriteLine("      Marker #{0}: ({1}, {2}, {3})",UnlabeledMarkerIndex,_Output_GetUnlabeledMarkerGlobalTranslation.Translation[0], _Output_GetUnlabeledMarkerGlobalTranslation.Translation[1],_Output_GetUnlabeledMarkerGlobalTranslation.Translation[2]);

                    if(usingLabels == 0) {
                        // initialize A and B (markers)
                        if (UnlabeledMarkerIndex == 0) {
                            thisp.AX = _Output_GetUnlabeledMarkerGlobalTranslation.Translation[0];
                            thisp.AY = _Output_GetUnlabeledMarkerGlobalTranslation.Translation[1];
                            existA = 1;
                        }

                        if (UnlabeledMarkerIndex == 1) {
                            thisp.BX = _Output_GetUnlabeledMarkerGlobalTranslation.Translation[0];
                            thisp.BY = _Output_GetUnlabeledMarkerGlobalTranslation.Translation[1];
                            existB = 1;
                        }
                    }
                }

                ////////////////////////////////////////////////////////////////
                if (existA == 1 && existB == 1 && trialState == true) {
                    
                    // verify A and B to make sure the vector has positive Y
                    if (usingLabels == 0) thisp.verifyAB();

                    thisp.GetVectors();
                    // ask head direction and send it to Haptic Feedback function 
                    
                    switch (thisp.hapticmodality[thisp.userID - 1, thisp.sectionNumber]) {
                        case 0:
                            thisp.GetPointDir(thisp.AskHeadDirection("Default")); // W NW N NE E (45°)
                            break;
                        case 1:
                            thisp.GetPointDir(thisp.AskHeadDirection("Thinner")); // W NW N NE E (15°)
                            break;
                        case 2:
                            thisp.GetPointDirFOV(thisp.AskHeadDirection("Default")); // W NW Nl N Nr NE E (with Tactile Fovea)
                            break;
                    }

                    thisp.ProximityToTarget();
                }
                ////////////////////////////////////////////////////////////////
            }

            if (TransmitMulticast){ MyClient.StopTransmittingMulticast(); }

            // Disconnect and dispose
            MyClient.Disconnect();
            MyClient = null;
        }

        private void OnApplicationExit(object sender, EventArgs e){
            LogRecord(logPath, "< logVicon > Exit time" + DateTime.Now.ToString("h: mm:ss tt"));
            //clean...
            DisplayDirection(0);
            //and close
            CloseConnection();
            portopenBelt = false;
        }
    }
}

