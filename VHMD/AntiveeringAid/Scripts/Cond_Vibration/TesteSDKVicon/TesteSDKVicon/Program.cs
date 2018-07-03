using System;
using System.Timers;
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

namespace CSharpClient {
    class Program {

        int userID = 1;
        int stmIndx = 6;
        bool blankN = true;
        bool enableSound = false;
        double TrgtX = -72;
        double TrgtY = 2534;
        //string logPath = @"C:\Users\victo\Documents\LogFileVicon.txt";
        string logPath = @"C:\Users\BLINDPAD\Documents\LogFileVicon.txt";

        /* Aux variables */
        int iddle = 1;
        int trialNumber = 0;
        bool firsttrial = false;
        int TargetasStart = 1;
        int fovnow = 0;
        int fovbefore = 0;
        int dirnow = 0;
        int dirbefore = 0;
        double AX = 0;
        double AY = 0;
        double BX = 0;
        double BY = 0;
        double StrtX = 0;
        double StrtY = 0;
        Vector vectorLkng;
        Vector vectorTrgt;
        double SumA = 0;
        double SumB = 0;
        int Frame = 0;
        double ErrorA = 0;
        double ErrorB = 0;
        double angleTarget;
        int FOVindex = 0;
        int headdir = 0;
        double angleHead;
        Vector vectorNrth = new Vector(0, 1);

        /* Serial Port communication with the vibrotactile device */
        static string COMPortBelt = "COM3";
        static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
        static bool portopenBelt = false; //if port is open or not
        char[] SendToArduino = new char[] { '0', ',', '0', ',', '0', ',', '1', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0' };

        /* Method used by DataStream SDK */
        static string Adapt(Direction i_Direction) {
            switch (i_Direction) {
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

        /* Method used by DataStream SDK */
        static string Adapt(DeviceType i_DeviceType) {
            switch (i_DeviceType) {
                case DeviceType.ForcePlate:
                    return "ForcePlate";
                case DeviceType.Unknown:
                default:
                    return "Unknown";
            }
        }

        /* Method used by DataStream SDK */
        static string Adapt(Unit i_Unit) {
            switch (i_Unit) {
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

        /* Set Serial Port communication with the vibrotactile device */
        public static void OpenConnectionBelt() {
            if (_SerialPortBelt != null) {
                if (_SerialPortBelt.IsOpen) {
                    _SerialPortBelt.Close();
                    Console.WriteLine("<logVicon> <sys> Closing port, because it was already open!");
                }
                else {
                    //very important!, this opens the connection
                    _SerialPortBelt.Open();
                    //sets the timeout value (how long it takes before timeout error occurs)
                    //zet de timeout niet te hoog, dit vertraagd Unity enorm en kan zelfs voor crashes zorgen.
                    //_SerialPortBelt.ReadTimeout = 1000;
                    //_SerialPortBelt.WriteTimeout = 50;
                    portopenBelt = true;
                }
            }
            else {
                if (_SerialPortBelt.IsOpen) { Console.WriteLine("<logVicon> <sys> Port is already open"); }
                else { Console.WriteLine("<logVicon> <sys> Port == null"); }
            }
        }

        /* Ends Serial Port communication with the vibrotactile device */
        public static void CloseConnection() {
            if (portopenBelt == true) {
                try {
                    _SerialPortBelt.Close();
                    Console.WriteLine("<logVicon> <sys> Closing port: {0}", COMPortBelt);
                }
                catch {
                    Console.WriteLine("<logVicon> <sys> ERROR in closing {0}", COMPortBelt);
                }
            }
        }

        /* Send string to vibrotactile device */
        private void Send() {
            try {
                _SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
                //LogRecord(logPath, "<logVicon> <sys> Submitted to Arduino");
            }
            catch {
                Console.WriteLine("<logVicon> <sys> ERROR Could not write in Serial Port");
                LogRecord(logPath, "<logVicon> <sys> ERROR Could not write in Serial Port");
            }
        }

        /* Create log file if it does not exist */
        private void CreateLogFile(string path) {
            if (!File.Exists(path)) {
                File.Create(path);
            }
            else if (File.Exists(path)) {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine("<logVicon> Trying to create new log for UserID_" + userID + " at: " + DateTime.Now.ToString("h:mm:ss tt"));
                tw.Close();
            }
        }

        /* Write on log file */
        private void LogRecord(string path, string logString) {
            if (File.Exists(path)) {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(logString);
                tw.Close();
            }
            else {
                Console.WriteLine("<logVicon> <sys> Trying to record log but file in " + path + " does not exist!");
            }
        }

        /* Display Section Heading */
        private void DisplayStmInfo() {
            Console.WriteLine();
            Console.WriteLine("Session #" + stmIndx);
            LogRecord(logPath, "<logVicon> Stimuli #" + stmIndx);
            Console.WriteLine();
        }

        /* Trigger tactor with direction (1-6) or set all tactors off (0) */
        private void DisplayDirection(int type) {

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
                    if (blankN) {
                        SendToArduino[0] = '0'; // ON 		0-2
                        SendToArduino[2] = '0'; // MOTOR 	1-8
                    }
                    else {
                        SendToArduino[2] = '4';
                        iddle = 0;
                    }
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

        /* Reset aux variables */
        private void setIddleState() {
            if (!enableSound) {
                DisplayDirection(0);

                dirbefore = 0;
                fovbefore = 0;
            }
            SumA = 0;
            Frame = 0;
            ErrorA = 0;
            SumB = 0;
            ErrorB = 0;

            trialNumber++;

            Console.WriteLine();
        }

        /* If Y is negative, invert A and B */
        private void verifyAB() {
            double verify = BY - AY;
            double buffX = 0;
            double buffY = 0;

            if (verify < 0) {
                buffX = AX;
                buffY = AY;
                AX = BX;
                AY = BY;
                BX = buffX;
                BY = buffY;
            }
        }

        /* Get target and headDirection vectors */
        private void GetVectors() {
            vectorTrgt = new Vector(TrgtX - AX, TrgtY - AY);
            vectorLkng = new Vector(BX - AX, BY - AY);
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
                //Console.WriteLine("errorr angle");
                return 333;
            }

        }

        /* Detection of cardinal point - Thinner angle in Fovea */
        private int AngletoIndexTwentybyFive(double angle) {
            if (angle >= -112.5 && angle < -67.5) {          // WEST
                return 0;
            }
            else if (angle >= -67.5 && angle < -22.5) {    // NW
                return 1;
            }
            else if (angle >= -22.5 && angle < -2.5) {    // NORTH (left)
                FOVindex = -1;
                return 2;
            }
            else if (angle >= -2.5 && angle < 2.5) {      // NORTH
                FOVindex = 0;
                return 2;
            }
            else if (angle >= 2.5 && angle < 22.5) {      // NORTH (right)
                FOVindex = 1;
                return 2;
            }
            else if (angle >= 22.5 && angle < 67.5) {     // NE
                return 3;
            }
            else if (angle >= 67.5 && angle < 112.5) {    // EAST;
                return 4;
            }
            else {                                        // error
                //Console.WriteLine("errorr angle");
                return 333;
            }

        }

        /* Detection of cardinal point - Thinner angle in Fovea */
        private int AngletoIndexTwentyonebyThree(double angle) {
            if (angle >= -112.5 && angle < -67.5) {          // WEST
                return 0;
            }
            else if (angle >= -67.5 && angle < -22.5) {    // NW
                return 1;
            }
            else if (angle >= -22.5 && angle < -1.5) {    // NORTH (left)
                FOVindex = -1;
                return 2;
            }
            else if (angle >= -1.5 && angle < 1.5) {      // NORTH
                FOVindex = 0;
                return 2;
            }
            else if (angle >= 1.5 && angle < 22.5) {      // NORTH (right)
                FOVindex = 1;
                return 2;
            }
            else if (angle >= 22.5 && angle < 67.5) {     // NE
                return 3;
            }
            else if (angle >= 67.5 && angle < 112.5) {    // EAST;
                return 4;
            }
            else {                                        // error
                //Console.WriteLine("errorr angle");
                return 333;
            }

        }

        /* Detection of cardinal point - Thinner angle in Fovea */
        private int AngletoIndexTwentytwobyOne(double angle) {
            if (angle >= -112.5 && angle < -67.5) {          // WEST
                return 0;
            }
            else if (angle >= -67.5 && angle < -22.5) {    // NW
                return 1;
            }
            else if (angle >= -22.5 && angle < -0.1) {    // NORTH (left)
                FOVindex = -1;
                return 2;
            }
            else if (angle >= -0.1 && angle < 0.1) {      // NORTH
                FOVindex = 0;
                return 2;
            }
            else if (angle >= 0.1 && angle < 22.5) {      // NORTH (right)
                FOVindex = 1;
                return 2;
            }
            else if (angle >= 22.5 && angle < 67.5) {     // NE
                return 3;
            }
            else if (angle >= 67.5 && angle < 112.5) {    // EAST;
                return 4;
            }
            else {                                        // error
                //Console.WriteLine("errorr angle");
                return 333;
            }

        }

        /* Detection of cardinal point - Thinner angle in Fovea */
        private int AngletoIndexSeventeenbyTen(double angle) {
            if (angle >= -112.5 && angle < -67.5) {          // WEST
                return 0;
            }
            else if (angle >= -67.5 && angle < -22.5) {    // NW
                return 1;
            }
            else if (angle >= -22.5 && angle < -5) {    // NORTH (left)
                FOVindex = -1;
                return 2;
            }
            else if (angle >= -5 && angle < 5) {      // NORTH
                FOVindex = 0;
                return 2;
            }
            else if (angle >= 5 && angle < 22.5) {      // NORTH (right)
                FOVindex = 1;
                return 2;
            }
            else if (angle >= 22.5 && angle < 67.5) {     // NE
                return 3;
            }
            else if (angle >= 67.5 && angle < 112.5) {    // EAST;
                return 4;
            }
            else {                                        // error
                //Console.WriteLine("errorr angle");
                return 333;
            }

        }

        /* Detection of cardinal point - Thinner angle */
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

        /* Return cardinal drection of the head related to the target */
        private int AskHeadDirection(string mode) {
            angleHead = Vector.AngleBetween(vectorLkng, vectorNrth);
            //Console.WriteLine("<logVicon> <sys> Head direction: " + DirectionsArray[AngletoIndex(angleHead)]);
            //LogRecord(logPath, "<logVicon> <sys> Head direction: " + DirectionsArray[AngletoIndex(angleHead)]);

            return AngletoIndexTwentytwobyOne(angleHead);

            /*if (mode == "Thinner") { return ThinAngletoIndex(angleHead); }
            else if (mode == "20x05") { return AngletoIndexTwentybyFive(angleHead); }
            else if (mode == "21x03") { return AngletoIndexTwentyonebyThree(angleHead); }
            else if (mode == "22x01") { return AngletoIndexTwentytwobyOne(angleHead); }
            else if (mode == "17x10") { return AngletoIndexSeventeenbyTen(angleHead); }
            else { return AngletoIndex(angleHead); }*/
        }

        /* Angle between head direction and target */
        private double ProximityToTarget() {
            angleTarget = Vector.AngleBetween(vectorLkng, vectorTrgt);
            return angleTarget;
        }

        /* Get an cardinal point and display it related to the head - no FOV*/
        private void GetPointDir(int inputdir) {
            double angle = ProximityToTarget();
            int motorIndx = AngletoIndex(angle);

            if (inputdir >= 0 && inputdir <= 4) {
                motorIndx++;
                if (motorIndx == 1) { dirnow = 1; if (dirbefore != dirnow) { DisplayDirectionFOV(7); dirbefore = dirnow; headdir = inputdir; iddle = 0; } } 	// West
                if (motorIndx == 2) { dirnow = 2; if (dirbefore != dirnow) { DisplayDirectionFOV(6); dirbefore = dirnow; headdir = inputdir; iddle = 0; } } 	// NW
                if (motorIndx == 3) { dirnow = 4; if (dirbefore != dirnow) { DisplayDirectionFOV(4); dirbefore = dirnow; headdir = inputdir; iddle = 0; } }     // NORTH
                if (motorIndx == 4) { dirnow = 6; if (dirbefore != dirnow) { DisplayDirectionFOV(2); dirbefore = dirnow; headdir = inputdir; iddle = 0; } } 	// NE
                if (motorIndx == 5) { dirnow = 7; if (dirbefore != dirnow) { DisplayDirectionFOV(1); dirbefore = dirnow; headdir = inputdir; iddle = 0; } } 	// East
            }
            else {
                dirnow = 0;
                if (dirbefore != dirnow) {
                    DisplayDirection(0);
                    dirbefore = dirnow;
                }
            }
        }

        /* Get an cardinal point and display it related to the head - with FOV */
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

        /* Alternate between two targets */
        private void ChangeTarget() {
            if (TargetasStart == 1) {
                TargetasStart = 0;
                vectorNrth = new Vector(0, -1);
                Console.WriteLine("Set target as box");
            }
            else {
                TargetasStart = 1;
                vectorNrth = new Vector(0, 1);
                Console.WriteLine("Set target as bottle");
            }
        }

        static void Main(string[] args) {

            Program thisp = new Program();
            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            int contFrames = 0;
            bool trialState = false;
            int usingLabels = 0;
            int existA = 0;
            int existB = 0;
            int soundid = 1;

            //System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\victo\Documents\audioVicon\audio1.wav");
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio1.wav");
            if (thisp.enableSound) player.Load();

            thisp.CreateLogFile(thisp.logPath);
            thisp.LogRecord(thisp.logPath, "<logVicon> <StartTime> " + DateTime.Now.ToString("h:mm:ss tt"));


            if (!thisp.enableSound) OpenConnectionBelt();

            //////////////////////////////////////////////////////////////////////////////////////////////

            //string HostName = "localhost:801";
            string HostName = "192.168.200.173";
            if (args.Length > 0) { HostName = args[0]; }

            // Make a new client
            ViconDataStreamSDK.DotNET.Client MyClient = new ViconDataStreamSDK.DotNET.Client();

            // Connect to a server
            Console.Write("Connecting to {0} ...", HostName);
            while (!MyClient.IsConnected().Connected) {
                MyClient.Connect(HostName);

                System.Threading.Thread.Sleep(200);
                Console.Write(".");

                try { if (Console.KeyAvailable) { } }
                catch (InvalidOperationException) { }
            }

            // Enable some different data types - Used by DataStream SDK
            MyClient.EnableSegmentData();
            MyClient.EnableMarkerData();
            MyClient.EnableUnlabeledMarkerData();
            MyClient.EnableDeviceData();
            MyClient.SetStreamMode(ViconDataStreamSDK.DotNET.StreamMode.ClientPull);
            MyClient.SetAxisMapping(ViconDataStreamSDK.DotNET.Direction.Forward, ViconDataStreamSDK.DotNET.Direction.Left, ViconDataStreamSDK.DotNET.Direction.Up);

            thisp.DisplayStmInfo();

            // Loop until a key is pressed
            while (true) {
                try {
                    if (Console.KeyAvailable) {
                        cki = Console.ReadKey(true);
                        //Console.WriteLine("You pressed the '{0}' key.", cki.Key);
                        // Spacebar: Trigger trial / Enter: end trial / ZX: Jump trials / O: Play sound / P: Pause sound / ESC: Close 

                        if (cki.Key == ConsoleKey.Escape) {
                            // Exit
                            thisp.LogRecord(thisp.logPath, "<logVicon> <ExitTime> " + DateTime.Now.ToString("h:mm:ss tt"));
                            thisp.DisplayDirection(0);

                            CloseConnection();
                            portopenBelt = false;
                            break;
                        }

                        if (cki.Key == ConsoleKey.O && thisp.enableSound) {
                            switch (soundid) {
                                case 1:
                                    soundid = 5;
                                    player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio5.wav");
                                    Console.WriteLine("Sound Five: 880Hz . 200ms beep . 800ms interval");
                                    if (thisp.enableSound) player.Load();
                                    break;
                                case 2:
                                    soundid = 1;
                                    player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio1.wav");
                                    Console.WriteLine("Sound One: 880Hz . 200ms beep . 800ms interval");
                                    if (thisp.enableSound) player.Load();
                                    break;
                                case 3:
                                    soundid = 2;
                                    player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio2.wav");
                                    Console.WriteLine("Sound Two: 880Hz . 200ms beep . 800ms interval");
                                    if (thisp.enableSound) player.Load();
                                    break;
                                case 4:
                                    soundid = 3;
                                    player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio3.wav");
                                    Console.WriteLine("Sound Three: 880Hz . 200ms beep . 800ms interval");
                                    if (thisp.enableSound) player.Load();
                                    break;
                                case 5:
                                    soundid = 4;
                                    player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio4.wav");
                                    Console.WriteLine("Sound Four: 880Hz . 200ms beep . 800ms interval");
                                    if (thisp.enableSound) player.Load();
                                    break;
                            }
                        }

                        if (cki.Key == ConsoleKey.P && thisp.enableSound) {
                            switch (soundid) {
                                case 1:
                                    soundid = 2;
                                    player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio2.wav");
                                    Console.WriteLine("Sound Two: 880Hz . 200ms beep . 800ms interval");
                                    if (thisp.enableSound) player.Load();
                                    break;
                                case 2:
                                    soundid = 3;
                                    player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio3.wav");
                                    Console.WriteLine("Sound Three: 880Hz . 200ms beep . 800ms interval");
                                    if (thisp.enableSound) player.Load();
                                    break;
                                case 3:
                                    soundid = 4;
                                    player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio4.wav");
                                    Console.WriteLine("Sound Four: 880Hz . 200ms beep . 800ms interval");
                                    if (thisp.enableSound) player.Load();
                                    break;
                                case 4:
                                    soundid = 5;
                                    player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio5.wav");
                                    Console.WriteLine("Sound Five: 880Hz . 200ms beep . 800ms interval");
                                    if (thisp.enableSound) player.Load();
                                    break;
                                case 5:
                                    soundid = 1;
                                    player = new System.Media.SoundPlayer(@"C:\Users\BLINDPAD\Documents\audioVicon\audio1.wav");
                                    Console.WriteLine("Sound One: 880Hz . 200ms beep . 800ms interval");
                                    if (thisp.enableSound) player.Load();
                                    break;
                            }
                        }

                        if (cki.Key == ConsoleKey.Spacebar) {
                            // Trigger trial
                            if (!trialState) {
                                //Console.Beep(3000, 500);
                                Console.WriteLine("Trial #" + thisp.trialNumber);
                                thisp.LogRecord(thisp.logPath, "<logVicon> <startTrial> Trial #" + thisp.trialNumber + " at " + DateTime.Now.ToString("h:mm:ss tt"));
                                trialState = true;

                                if (thisp.enableSound) {
                                    player.Play();
                                    Console.WriteLine("Sound is on");
                                }
                            }
                        }

                        if (cki.Key == ConsoleKey.Enter) {
                            // Answer
                            if (trialState) {
                                // need to calculate elapsed time
                                thisp.LogRecord(thisp.logPath, "<logVicon> <endTrial> at " + DateTime.Now.ToString("h:mm:ss tt"));
                                thisp.ErrorA = thisp.SumA / thisp.Frame;
                                thisp.ErrorB = thisp.SumB / thisp.Frame;
                                thisp.LogRecord(thisp.logPath, "<logVicon> <errora> " + thisp.ErrorA);
                                thisp.LogRecord(thisp.logPath, "<logVicon> <errorb> " + thisp.ErrorB);
                                Console.WriteLine("<error a> " + thisp.ErrorA + " - <error b> " + thisp.ErrorB);

                                thisp.setIddleState();
                                trialState = false;
                                thisp.ChangeTarget();

                                if (thisp.enableSound) {
                                    player.Stop();
                                    Console.WriteLine("Sound is off");
                                }
                            }
                        }

                        if (cki.Key == ConsoleKey.Z && thisp.trialNumber > 0) {
                            // Trial Anterior
                            thisp.trialNumber--;
                            Console.WriteLine("Ready to trigger stimulus #" + thisp.trialNumber);
                            thisp.LogRecord(thisp.logPath, "<logVicon> Return to stimulus #" + thisp.trialNumber);

                            if (trialState) {
                                thisp.DisplayDirection(0);
                                trialState = false;
                            }
                        }

                        if (cki.Key == ConsoleKey.X) {
                            // Trial seguinte
                            thisp.trialNumber++;
                            Console.WriteLine("Ready to trigger stimulus #" + thisp.trialNumber);
                            thisp.LogRecord(thisp.logPath, "<logVicon> Jump to stimulus #" + thisp.trialNumber);

                            if (trialState) {
                                thisp.DisplayDirection(0);
                                trialState = false;
                            }
                        }

                        if (cki.Key == ConsoleKey.T) {
                            thisp.ChangeTarget();
                        }

                        // Stimuli
                        if (cki.Key == ConsoleKey.S) {
                            // change stimuli
                            switch (thisp.stmIndx) {
                                case 0:
                                    thisp.stmIndx = 1;
                                    break;
                                case 1:
                                    thisp.stmIndx = 2;
                                    break;
                                case 2:
                                    thisp.stmIndx = 3;
                                    break;
                                case 3:
                                    thisp.stmIndx = 4;
                                    break;
                                case 4:
                                    thisp.stmIndx = 5;
                                    break;
                                case 5:
                                    thisp.stmIndx = 6;
                                    break;
                                case 6:
                                    thisp.stmIndx = 0;
                                    break;
                            }
                        }

                        // Balck on north
                        if (cki.Key == ConsoleKey.B) {
                            if (thisp.blankN) thisp.blankN = false;
                            else thisp.blankN = true;
                        }

                    }
                }
                catch (InvalidOperationException) { }

                // Get a frame
                while (MyClient.GetFrame().Result != ViconDataStreamSDK.DotNET.Result.Success) {
                    System.Threading.Thread.Sleep(200);
                }

                Output_GetFrameNumber _Output_GetFrameNumber = MyClient.GetFrameNumber();
                //Console.WriteLine("Frame Number: {0}", _Output_GetFrameNumber.FrameNumber);
                Output_GetFrameRate _Output_GetFrameRate = MyClient.GetFrameRate();
                //Console.WriteLine("Frame rate: {0}", _Output_GetFrameRate.FrameRateHz);

                // Count the number of subjects
                uint SubjectCount = MyClient.GetSubjectCount().SubjectCount;
                if (SubjectCount > 0) usingLabels = 1;
                existA = 0; existB = 0;

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

                        if (MarkerName == "sona") {
                            if (thisp.TargetasStart == 1) {
                                thisp.StrtX = _Output_GetMarkerGlobalTranslation.Translation[0];
                                thisp.StrtY = _Output_GetMarkerGlobalTranslation.Translation[1];
                            }
                            else {
                                thisp.TrgtX = _Output_GetMarkerGlobalTranslation.Translation[0];
                                thisp.TrgtY = _Output_GetMarkerGlobalTranslation.Translation[1];
                            }

                        }

                        if (MarkerName == "top") {
                            if (thisp.TargetasStart == 1) {
                                thisp.TrgtX = _Output_GetMarkerGlobalTranslation.Translation[0];
                                thisp.TrgtY = _Output_GetMarkerGlobalTranslation.Translation[1];
                            }
                            else {
                                thisp.StrtX = _Output_GetMarkerGlobalTranslation.Translation[0];
                                thisp.StrtY = _Output_GetMarkerGlobalTranslation.Translation[1];
                            }
                        }
                    }
                }

                // Get the unlabeled markers
                uint UnlabeledMarkerCount = MyClient.GetUnlabeledMarkerCount().MarkerCount;
                for (uint UnlabeledMarkerIndex = 0; UnlabeledMarkerIndex < UnlabeledMarkerCount; ++UnlabeledMarkerIndex) {
                    Output_GetUnlabeledMarkerGlobalTranslation _Output_GetUnlabeledMarkerGlobalTranslation = MyClient.GetUnlabeledMarkerGlobalTranslation(UnlabeledMarkerIndex);
                    //Console.WriteLine("      Marker #{0}: ({1}, {2}, {3})",UnlabeledMarkerIndex,_Output_GetUnlabeledMarkerGlobalTranslation.Translation[0], _Output_GetUnlabeledMarkerGlobalTranslation.Translation[1],_Output_GetUnlabeledMarkerGlobalTranslation.Translation[2]);

                    /*if (usingLabels == 0) {
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
                    }*/
                }

                //////////////////////////////////////////////////////////////////////////////////////////////
                if (existA == 1 && existB == 1 && trialState == true) {

                    // verify A and B to make sure the vector has positive Y
                    //if (usingLabels == 0) thisp.verifyAB();

                    thisp.GetVectors();

                    contFrames++;
                    if (contFrames > 5) {
                        contFrames = 0;
                        //Console.WriteLine("<pos> A X: " + Math.Truncate(thisp.AX) + ": Y :" + Math.Truncate(thisp.AY) + ": B X: " + Math.Truncate(thisp.BX) + ": Y :" + Math.Truncate(thisp.BY) + ": at :" + DateTime.Now.ToString("h_mm_ss tt"));
                        thisp.LogRecord(thisp.logPath, "<logVicon> <pos> A X: " + Math.Truncate(thisp.AX) + ": Y :" + Math.Truncate(thisp.AY) + ": B X: " + Math.Truncate(thisp.BX) + ": Y :" + Math.Truncate(thisp.BY) + ": at :" + DateTime.Now.ToString("h_mm_ss tt"));
                        thisp.SumA = thisp.SumA + (Math.Abs((thisp.TrgtX - thisp.StrtX) * (thisp.StrtY - thisp.AY) - (thisp.StrtX - thisp.AX) * (thisp.TrgtY - thisp.StrtY)) / Math.Sqrt(Math.Pow(thisp.TrgtX - thisp.StrtX, 2) + Math.Pow(thisp.TrgtY - thisp.StrtY, 2)));
                        thisp.SumB = thisp.SumB + (Math.Abs((thisp.TrgtX - thisp.StrtX) * (thisp.StrtY - thisp.BY) - (thisp.StrtX - thisp.BX) * (thisp.TrgtY - thisp.StrtY)) / Math.Sqrt(Math.Pow(thisp.TrgtX - thisp.StrtX, 2) + Math.Pow(thisp.TrgtY - thisp.StrtY, 2)));
                        thisp.Frame++;
                    }

                    if (!thisp.enableSound) {
                        thisp.GetPointDirFOV(thisp.AskHeadDirection("22x01"));
                        /*
                        switch (thisp.stmIndx) {
                            case 0:
                                thisp.GetPointDir(thisp.AskHeadDirection("Default")); // W NW N NE E (45°)
                                break;
                            case 1:
                                thisp.GetPointDir(thisp.AskHeadDirection("Thinner")); // W NW N NE E (15°)
                                break;
                            case 2:
                                thisp.GetPointDirFOV(thisp.AskHeadDirection("Default")); // W NW Nl N Nr NE E (with Tactile Fovea)
                                break;
                            case 3:
                                thisp.GetPointDirFOV(thisp.AskHeadDirection("17x10")); // W NW Nl N Nr NE E (with Tactile Fovea)
                                break;
                            case 4:
                                thisp.GetPointDirFOV(thisp.AskHeadDirection("20x05")); // W NW Nl N Nr NE E (with Tactile Fovea)
                                break;
                            case 5:
                                thisp.GetPointDirFOV(thisp.AskHeadDirection("21x03")); // W NW Nl N Nr NE E (with Tactile Fovea)
                                break;
                            case 6:
                                thisp.GetPointDirFOV(thisp.AskHeadDirection("22x01")); // W NW Nl N Nr NE E (with Tactile Fovea)
                                break;
                        }*/
                    }

                    thisp.ProximityToTarget();
                }
                //////////////////////////////////////////////////////////////////////////////////////////////
            }

            // Disconnect and dispose
            MyClient.Disconnect();
            MyClient = null;
        }

        /* Quit/Exit/Close */
        private void OnApplicationExit(object sender, EventArgs e) {
            LogRecord(logPath, "< logVicon > Exit time" + DateTime.Now.ToString("h: mm:ss tt"));
            //clean...
            DisplayDirection(0);
            //and close
            CloseConnection();
            portopenBelt = false;
        }
    }
}