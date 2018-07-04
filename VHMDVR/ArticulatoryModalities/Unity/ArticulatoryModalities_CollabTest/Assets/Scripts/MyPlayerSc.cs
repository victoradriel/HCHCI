﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;
using System.Timers;

namespace Leap.Unity.DetectionExamples {

    public class MyPlayerSc : NetworkBehaviour {
        bool somethingslctd = false;
        public GameObject pointnow = null;
        public GameObject objectset = null;
        public GameObject pointselected = null;
        public GameObject pointselectedother = null;
        GameObject previouspoint = null;
        Text trialtxt;
        Text hartc;
        GameObject TrialText;
        //GameObject HArticulation;
        GameObject OtherChoice;
        GameObject OuttaSight;
        GameObject Arrow;
        GameObject PinchDetector_L;
        GameObject PinchDetector_R;
        GameObject Leapcam;

        Vector3[] previousposition;
        Quaternion[] previousrotation;

        Answer chosenObj;
        GameObject caldeira;
        
        public int trialid = 0;
        public int otrialid = 0;
        GameObject AudioObj;
        GameObject sSettings;
        GameObject netObj;
        int trialLeng = 0;
        bool sessionSet = false;
        public List<string> myIng;
        public List<string> otIng;
        Scene scene;
        bool scenechange = false;
        bool sceneclosing = false;
        public int taskid = 1;
        int dyadid; 
        public string ingpointingnow = "";
        bool flush = false;

        // Articulation
        bool hartstate = true;
        public int displaystate = 0;
        public int gesture;
        public int enableDraw = 0;
        public System.Timers.Timer timerDrawWave;

        int res = 2560; // GS6
        public float xps = 0;
        float delta = 0;
        public int balls = 5; 
        int ansstart = 0;
        int ansend = 0;
        long ansdur = 0;

        [Tooltip("Each pinch detector can draw one line at a time.")]
        [SerializeField]
        private PinchDetector[] _pinchDetectors = new PinchDetector[2];

        // Use this for initialization
        public override void OnStartLocalPlayer() {
            scene = SceneManager.GetActiveScene();
            if (!isLocalPlayer || scene.buildIndex == 0)
                return;

            FindObjects();             
            if (isServer) sSettings.GetComponent<Sessions>().CreateLogFile();
            if (isServer) sSettings.GetComponent<Sessions>().LogIt("<logTactWiz><sys>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";StartTime;Dyad;" + netObj.GetComponent<NetworkMenu>().dyadID + ";Scene;"+(scene.buildIndex -1));

            timerDrawWave = new System.Timers.Timer(200);
            timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
		    timerDrawWave.Start();
            delta = res / balls;
        }     
        
        void FixedUpdate( ) {
            if (!isLocalPlayer || scene.buildIndex == 0) return;

            if (sessionSet) {
                if (displaystate > 0 && enableDraw == 1) {
                    enableDraw = 0;
                    displaystate++;
                    if (displaystate > 3) { displaystate = 0; CmdArticulate(gesture, ansstart, ansend, ansdur, ingpointingnow, netObj.GetComponent<NetworkMenu>().netmode); }   
                }

                if (MainController.control.sentSmthng && hartstate) {
                    hartstate = false;
                    MainController.control.sentSmthng = false;
                    if(taskid == 1) GetAnswerA(MainController.control.mode, MainController.control.xstrt, MainController.control.xend, MainController.control.rtbtDur);
                    else if(taskid == 2) GetAnswerB(MainController.control.mode, MainController.control.xstrt, MainController.control.xend, MainController.control.rtbtDur);
                    else GetAnswerC(MainController.control.mode, MainController.control.xstrt, MainController.control.xend, MainController.control.rtbtDur);
                    ingpointingnow = pointnow.name;
                }
            }            
        }   
        
        /* Static Gestures */
        public void GetAnswerA(float thismode, float thisstrt, float thisend, long thisdur) {
            ansstart = 0;
            ansend = 0;
            ansdur = thisdur;
            displaystate = 3;

            bool pos = true;
            float thisangle = thisend - thisstrt;
            if (thisangle < 0) { thisangle = thisangle * (-1); pos = false;}
            thisangle = (90 * thisangle) / res;

            // Start ID
            if (thisstrt % delta > 0) ansstart = (int) (thisstrt / delta + 1); 
            else ansstart = (int) (thisstrt / delta);
            if (ansstart < 1) ansstart = 1;
            if (ansstart > balls) ansstart = balls;
            // End ID
            if (thisend % delta > 0) ansend = (int) (thisend / delta + 1); 
            else ansend = (int) (thisend / delta);
            if (ansend < 1) ansend = 1;
            if (ansend > balls) ansend = balls;           

            //"2 Taps", "Tap", "Long Tap", "45 right", "45 left", "90 right", "90 left", "other" 
            int intmode = Mathf.RoundToInt(thismode);
            switch (intmode) {
                case 22:
                    gesture = 0;       
                    break;
                case 33:
                    gesture = 2;       
                    break;
                case 44:
                    if (thisangle <= 65) {
                        if (pos) gesture = 3;
                        else gesture = 4;
                    } else if (thisangle > 65) {
                        if (pos) gesture = 5;
                        else gesture = 6;
                    }
                    break;
                default:
                    gesture = 7;       
                    break;
            }

            hartstate = true;
        }

        /* Free Articulation */
        public void GetAnswerB(float thismode, float thisstrt, float thisend, long thisdur) {
            ansstart = 0;
            ansend = 0;
            ansdur = thisdur;
            displaystate = 3;

            // Start ID
            if (thisstrt % delta > 0) ansstart = (int) (thisstrt / delta + 1); 
            else ansstart = (int) (thisstrt / delta);
            if (ansstart < 1) ansstart = 1;
            if (ansstart > balls) ansstart = balls;
            // End ID
            if (thisend % delta > 0) ansend = (int) (thisend / delta + 1); 
            else ansend = (int) (thisend / delta);
            if (ansend < 1) ansend = 1;
            if (ansend > balls) ansend = balls;           

            //"2 Taps", "Tap", "Long Tap", "45 right", "45 left", "90 right", "90 left", "other" 
            int intmode = Mathf.RoundToInt(thismode);
            switch (intmode) {
                case 11:
                    gesture = 1;  
                    displaystate = 1;
                    break;
                case 22:
                    gesture = 0;       
                    break;
                case 33:
                    gesture = 2;       
                    break;
                case 44:
                    if (ansstart < ansend) gesture = 9;
                    else if (ansstart > ansend) gesture = 10;
                    break;
                default:
                    gesture = 7;       
                    break;
            }

            hartstate = true;
        }

        /* Mixed Articulation */
        public void GetAnswerC(float thismode, float thisstrt, float thisend, long thisdur) {
            ansstart = 0;
            ansend = 0;
            ansdur = thisdur;
            displaystate = 3;

            bool pos = true;
            float thisangle = thisend - thisstrt;
            if (thisangle < 0) { thisangle = thisangle * (-1); pos = false;}
            thisangle = (90 * thisangle) / res;

            // Start ID
            if (thisstrt % delta > 0) ansstart = (int) (thisstrt / delta + 1); 
            else ansstart = (int) (thisstrt / delta);
            if (ansstart < 1) ansstart = 1;
            if (ansstart > balls) ansstart = balls;
            // End ID
            if (thisend % delta > 0) ansend = (int) (thisend / delta + 1); 
            else ansend = (int) (thisend / delta);
            if (ansend < 1) ansend = 1;
            if (ansend > balls) ansend = balls;           

            //"2 Taps", "Tap", "Long Tap", "45 right", "45 left", "90 right", "90 left", "other" 
            int intmode = Mathf.RoundToInt(thismode);
            switch (intmode) {
                case 11:
                    gesture = 1;  
                    displaystate = 1;
                    break;
                case 22:
                    gesture = 0;       
                    break;
                case 33:
                    gesture = 2;       
                    break;
                case 44:
                    if (thisangle <= 65) {
                        if (pos) gesture = 3;
                        else gesture = 4;
                    } else if (thisangle > 65) {
                        if (pos) gesture = 5;
                        else gesture = 6;
                    }
                    break;
                default:
                    gesture = 7;       
                    break;
            }

            hartstate = true;
        }

        void EnableDraw(object source, ElapsedEventArgs e){
		    enableDraw = 1;
	    }

        public void OnApplicationQuit() {
            if(timerDrawWave != null) timerDrawWave.Stop();     
	    }  

        // Update is called once per frame
        void Update() {
            if (!isLocalPlayer || scene.buildIndex == 0) return;

            // Initial Setup
            if (PinchDetector_L == null) PinchDetector_L = GameObject.Find("PinchDetector_L");
            if (PinchDetector_R == null) PinchDetector_R = GameObject.Find("PinchDetector_R");        

            if (!sessionSet && PinchDetector_L != null && PinchDetector_R != null) {
                sessionSet = true;
                if(isServer) sSettings.GetComponent<Sessions>().CmdResetList();
                SetSession();
            }

            // Main
            CheckUpdate();    

            if (sessionSet && !sceneclosing) {
                if (trialid < trialLeng) {
                    //trialtxt.text = myIng [trialid];
                    if (!somethingslctd) LookingAt();
                    Interact();
                } else {
                    trialtxt.text = "...";
                    if (otrialid >= trialLeng) {
                        trialtxt.text = "Game Over";
                        AudioObj.GetComponent<SoundScript>().setStop = true;
                        if (isServer) sSettings.GetComponent<Sessions>().LogIt("<logTactWiz><sys>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";EndTime");
                    } 
                }
            }

            // Keys

            if (Input.GetKey(KeyCode.Space) && isServer && !scenechange) {
                scenechange = true;
                netObj.GetComponent<NetworkMenu>().scenechange = false;
                //netObj.GetComponent<NetworkMenu>().setupdone = false;
                netObj.GetComponent<NetworkMenu>().thisnetm.ServerChangeScene("Intro");
            }  
            
            if (Input.GetKey(KeyCode.Escape) && isServer && !sceneclosing) {
                sceneclosing = true;
                if (isServer) sSettings.GetComponent<Sessions>().logFile.Close();
                CmdCloseClients();
            } 
            
            if (Input.GetKey(KeyCode.Keypad7) && isServer && taskid != 1) CmdChangeTaskID(1);      
            if (Input.GetKey(KeyCode.Keypad8) && isServer && taskid != 2) CmdChangeTaskID(2);   
            if (Input.GetKey(KeyCode.Keypad9) && isServer && taskid != 3) CmdChangeTaskID(3);                 
        }

        void FindObjects( ) {
            sSettings = GameObject.Find("SessionSettings");
            netObj = GameObject.Find("NetworkObj");
            AudioObj = GameObject.Find("AudioObj");

            objectset = GameObject.Find("Ingredients");     // Ingredients
            caldeira = GameObject.Find("Cauldron");         // Cauldron                      
            TrialText = GameObject.Find("TrialText");       // Output            
            OuttaSight = GameObject.Find("outta");          // Obj out of sight *optional
            Leapcam = GameObject.Find("CenterEyeAnchor");   // Camera
            
            OtherChoice = GameObject.Find("Spotlight");
            Arrow = GameObject.Find("ArrowG");
        }

        void CheckUpdate( ) {
            if (netObj.GetComponent<NetworkMenu>().netmode == "h") {
                if (sSettings.GetComponent<Sessions>().htrialID != trialid)
                    trialid = sSettings.GetComponent<Sessions>().htrialID;
                if (sSettings.GetComponent<Sessions>().ctrialID != otrialid) {
                    otrialid = sSettings.GetComponent<Sessions>().ctrialID;
                    placeArrow();
                }
            }
            else {
                if (sSettings.GetComponent<Sessions>().ctrialID != trialid)
                    trialid = sSettings.GetComponent<Sessions>().ctrialID;
                if (sSettings.GetComponent<Sessions>().htrialID != otrialid) {
                    otrialid = sSettings.GetComponent<Sessions>().htrialID;
                    placeArrow();
                } 
            }

            if (Leapcam.GetComponent<Vibration>().gestStat == 15) {
                //print("long playing?");
                //Leapcam.GetComponent<Vibration>().gestStat = 66;
                //CmdShutUp(gesture);
            } else if (Leapcam.GetComponent<Vibration>().gestStat == 33) {
                //print("update 33?");
                Leapcam.GetComponent<Vibration>().gestStat = 77;
                CmdShutUp(gesture);
            }

            if (sSettings.GetComponent<Sessions>().close) {
                if(timerDrawWave != null) timerDrawWave.Stop();    
                Leapcam.GetComponent<Vibration>().CloseAll();
                trialtxt.text = "Exit";
            }

            if (taskid != sSettings.GetComponent<Sessions>().taskID) {
                taskid = sSettings.GetComponent<Sessions>().taskID;
                trialtxt.text = "TASK " + taskid;
            }    
        }

        void SetSession() {
            _pinchDetectors [0] = PinchDetector_L.GetComponent<PinchDetector>();
            _pinchDetectors [1] = PinchDetector_R.GetComponent<PinchDetector>();

            chosenObj = caldeira.GetComponent<Answer>();   
            trialtxt = TrialText.GetComponent<Text>();

            dyadid = System.Int32.Parse(netObj.GetComponent<NetworkMenu>().dyadID);

            trialLeng = sSettings.GetComponent<Sessions>().IngHost.Count;
            previousposition = new Vector3[trialLeng];
            previousrotation = new Quaternion[trialLeng];

            if (netObj.GetComponent<NetworkMenu>().netmode == "h") {
                myIng = new List<string>(sSettings.GetComponent<Sessions>().IngHost);
                otIng = new List<string>(sSettings.GetComponent<Sessions>().IngClnt);
            } else {
                myIng = new List<string>(sSettings.GetComponent<Sessions>().IngClnt);
                otIng = new List<string>(sSettings.GetComponent<Sessions>().IngHost);
            }

            GameObject ing;
            // MINE
            for (int i = 0; i < trialLeng; i++) {
                if (netObj.GetComponent<NetworkMenu>().netmode == "h") ing = GameObject.Find(sSettings.GetComponent<Sessions>().IngHost [i]);
                else ing = GameObject.Find(sSettings.GetComponent<Sessions>().IngClnt [i]);

                previousposition [i] = ing.transform.position;
                previousrotation [i] = ing.transform.rotation;
            }

            // OF OTHER
            Color color = new Color(0.3F, 0.3F, 0.3F, 0.5F);
            for (int i = 0; i < trialLeng; i++) {
                if (netObj.GetComponent<NetworkMenu>().netmode == "h") ing = GameObject.Find(sSettings.GetComponent<Sessions>().IngClnt[i]);
                else ing = GameObject.Find(sSettings.GetComponent<Sessions>().IngHost[i]);

                ing.GetComponent<Renderer>().material.color = color;
            }

            if (isServer) {
                sSettings.GetComponent<Sessions>().LogIt("<logTactWiz><host>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";Trial;" + trialid + ";" + myIng [trialid] );
                sSettings.GetComponent<Sessions>().LogIt("<logTactWiz><client>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";Trial;" + otrialid + ";" + otIng[otrialid] );
            } 

            placeArrow();

            AudioObj.GetComponent<SoundScript>().setStart = true;
        }

        void placeArrow() {
            if (otrialid < trialLeng) {
                GameObject nextchoice = GameObject.Find(otIng[otrialid]);
                Vector3 objpos = new Vector3(nextchoice.transform.position.x, -13.9f, nextchoice.transform.position.z);
                Arrow.transform.position = objpos;  
            } else {
                Vector3 objpos = new Vector3(OuttaSight.transform.position.x, -14, OuttaSight.transform.position.z);
                Arrow.transform.position = objpos;
                OtherChoice.transform.position = objpos;
            }
        }

        [Command]
        public void CmdCloseClients() {
            GameObject thissSettings = GameObject.Find("SessionSettings");
            thissSettings.GetComponent<Sessions>().close = true;
        }

        [Command]
        public void CmdChangeTaskID(int id) {
            GameObject thissSettings = GameObject.Find("SessionSettings");
            thissSettings.GetComponent<Sessions>().taskID = id;
        }

        [Command]
        public void CmdSetDeletedObj(string Ingname) {
            GameObject thissSettings = GameObject.Find("SessionSettings");
            thissSettings.GetComponent<Sessions>().IngStat [thissSettings.GetComponent<Sessions>().Ingredients.IndexOf(Ingname)] = false;
        }

        [Command]
        public void CmdGrabObj(string Ingname) {
            GameObject thissSettings = GameObject.Find("SessionSettings");
            thissSettings.GetComponent<Sessions>().IngHolding [thissSettings.GetComponent<Sessions>().Ingredients.IndexOf(Ingname)] = true;
        }

        [Command]
        public void CmdReleaseObj(string Ingname) {
            GameObject thissSettings = GameObject.Find("SessionSettings");
            thissSettings.GetComponent<Sessions>().IngHolding [thissSettings.GetComponent<Sessions>().Ingredients.IndexOf(Ingname)] = false;
        }

        [Command]
        public void CmdArticulate(int gestid, int startid, int endid, long durid, string Ingname, string interlocutor) {
            GameObject thissSettings = GameObject.Find("SessionSettings");
            if (thissSettings.GetComponent<Sessions>().TriggerGest.Contains(true))
                thissSettings.GetComponent<Sessions>().TriggerGest [thissSettings.GetComponent<Sessions>().TriggerGest.IndexOf(true)] = false;

            thissSettings.GetComponent<Sessions>().ingID = thissSettings.GetComponent<Sessions>().Ingredients.IndexOf(Ingname);
            thissSettings.GetComponent<Sessions>().startvib = startid;
            thissSettings.GetComponent<Sessions>().endvib = endid;
            thissSettings.GetComponent<Sessions>().durvib = durid;

            thissSettings.GetComponent<Sessions>().TriggerGest [gestid] = true;            

            if (interlocutor == "h") { thissSettings.GetComponent<Sessions>().hisTalking = true; }
            else { thissSettings.GetComponent<Sessions>().hisTalking = false; }
        }

        [Command]
        public void CmdArticulateOld(int gestid, string Ingname, string interlocutor) {
            GameObject thissSettings = GameObject.Find("SessionSettings");
            if (thissSettings.GetComponent<Sessions>().TriggerGest.Contains(true))
                thissSettings.GetComponent<Sessions>().TriggerGest [thissSettings.GetComponent<Sessions>().TriggerGest.IndexOf(true)] = false;

            thissSettings.GetComponent<Sessions>().TriggerGest [gestid] = true;
            thissSettings.GetComponent<Sessions>().ingID = thissSettings.GetComponent<Sessions>().Ingredients.IndexOf(Ingname);

            if (interlocutor == "h") { thissSettings.GetComponent<Sessions>().hisTalking = true; }
            else { thissSettings.GetComponent<Sessions>().hisTalking = false; }
        }

        [Command]
        public void CmdShutUp(int gestid) {
            GameObject thissSettings = GameObject.Find("SessionSettings");
            thissSettings.GetComponent<Sessions>().TriggerGest [gestid] = false;          
        }

        [Command]
        public void CmdErrorMsg(string msg) {
            GameObject thissSettings = GameObject.Find("SessionSettings");
            thissSettings.GetComponent<Sessions>().LogIt(msg);
        }

        void Interact() {
            for (int i = 0; i < _pinchDetectors.Length; i++) {
                var detector = _pinchDetectors[i];

                if (detector.DidStartHold && myIng.Contains(pointnow.name)) {
                    pointselected = pointnow;
                    somethingslctd = true;
                    pointselected.transform.position = _pinchDetectors[i].transform.position;
                    pointselected.transform.parent = _pinchDetectors[i].transform;
                    CmdGrabObj(pointselected.name);
                    CmdArticulate(8, 0, 0, 0, pointnow.name, netObj.GetComponent<NetworkMenu>().netmode);
                }

                if (detector.DidRelease && somethingslctd) {
                    CmdReleaseObj(pointselected.name);

                    if (chosenObj.overthecauldron != "") {
                        if (chosenObj.overthecauldron == myIng[trialid]) {
                            CmdSetDeletedObj(myIng[trialid]);
                        }
                        else {
                            pointselected.transform.parent = objectset.transform;
                            CmdErrorMsg("<logTactWiz><answer>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";0;" + chosenObj.overthecauldron + ";" + myIng[trialid] + ";" + System.DateTime.Now.ToString("h:mm:ss tt"));
                            i = myIng.IndexOf(pointselected.name);
                            if (i != -1) {
                                pointselected.transform.position = previousposition[i];
                                pointselected.transform.rotation = previousrotation[i];
                            }
                        }

                        chosenObj.overthecauldron = "";

                    } else {
                        pointselected.transform.parent = objectset.transform;
                        i = myIng.IndexOf(pointselected.name);

                        if (i != -1) {
                            pointselected.transform.position = previousposition[i];
                            pointselected.transform.rotation = previousrotation[i];
                        }
                    }

                    // reset
                    pointselected = null;
                    somethingslctd = false;                    
                }
            }
        }

        void LookingAt() {
            if (Leapcam.GetComponent<LookingAt>().islooking) {
                pointnow = Leapcam.GetComponent<LookingAt>().pointnow;

                if (myIng.Contains(pointnow.name)) {
                    pointnow.GetComponent<Renderer>().material.color = Color.yellow;
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
        }

        void Selectit(GameObject point) {
            print(point.name);
            point.GetComponent<Renderer>().material.color = Color.grey;
        }

    }
}
