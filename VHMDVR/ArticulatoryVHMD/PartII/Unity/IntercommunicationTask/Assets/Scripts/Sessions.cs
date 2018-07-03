using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class Sessions : NetworkBehaviour {
    
    public SyncListString Ingredients = new SyncListString();
    public SyncListBool IngStat = new SyncListBool();
    public SyncListBool IngHolding = new SyncListBool();
    public SyncListString IngHost = new SyncListString();
    public SyncListString IngClnt = new SyncListString();
    public SyncListBool TriggerGest = new SyncListBool();
    public string [] GesturesSt = new string [] { "2 Taps", "Tap", "Long Tap", "45 right", "45 left", "90 right", "90 left", "other" };
    public string [] IngredientsSt = new string[] { "Skull", "PotionRed", "ChestWhite", "Cheese", "Bread", "Bone", "AppleGreen", "Pumpkin", "BottleGreen", "SackBlack", "BottlePurple", "BottleRed", "BottleBlue", "BottleDBlue", "PotionPink", "PotionGreen", "SackPurple", "ChestBlack", "AppleRed", "SackWhite" };
    public string [] IngredientsTreino = new string[] { "PotionRed", "Bread", "Bone", "AppleGreen", "SackBlack", "BottleDBlue", "PotionGreen", "AppleRed" };
    GameObject lightother;
    GameObject netObj;
    GameObject OuttaSight;
    GameObject pointselectedother;
    GameObject Vibration;
    public StreamWriter logFile;
    public bool cmdshutup = false;
    Scene scene;

    [SyncVar]
    public int htrialID = 0;

    [SyncVar]
    public bool close = false;

    [SyncVar]
    public int ctrialID = 0;

    [SyncVar]
    public bool isTalking = false;

    [SyncVar]
    public int ingID = -1;

    [SyncVar]
    public bool hisTalking = false;    

    public override void OnStartClient(){
        scene = SceneManager.GetActiveScene();
        if (scene.buildIndex == 1) for (int i = 0; i < IngredientsTreino.Length; i++) { IngStat.Add(true); IngHolding.Add(false); } 
        else for (int i = 0; i < IngredientsSt.Length; i++) { IngStat.Add(true); IngHolding.Add(false); } 

        for (int i = 0; i < GesturesSt.Length; i++) { TriggerGest.Add(false); }
        IngStat.Callback = IngStatChanged;
        IngHolding.Callback = IngHoldingChanged;
        TriggerGest.Callback = GestureChanged;

        lightother = GameObject.Find("Spotlight");
        netObj = GameObject.Find("NetworkObj");
        OuttaSight = GameObject.Find("outta");
        Vibration = GameObject.Find("CenterEyeAnchor");  
    }

    public void CreateLogFile() {
        logFile = File.CreateText(Application.persistentDataPath + "/logDyad " + netObj.GetComponent<NetworkMenu>().dyadID + " _ " + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv");
    }

    public void LogIt(string logmsg) {
        logFile.WriteLine(logmsg);
    }

    [Command]
    public void CmdResetList() {
        Ingredients.Clear();
        IngHost.Clear();
        IngClnt.Clear();

        if (scene.buildIndex == 1) {
            IngredientsTreino = MyFisherYatesShffl(IngredientsTreino);
            for (int i = 0; i < IngredientsTreino.Length; i++)  Ingredients.Add(IngredientsTreino [i]); 
            
            int halfList = (IngredientsTreino.Length) / 2;
            for (int i = 0; i < halfList; i++) IngHost.Add(IngredientsTreino[i]); 
            for (int i = halfList; i < IngredientsTreino.Length; i++) IngClnt.Add(IngredientsTreino[i]);
        }
        else {
            IngredientsSt = MyFisherYatesShffl(IngredientsSt);
            for (int i = 0; i < IngredientsSt.Length; i++)  Ingredients.Add(IngredientsSt [i]); 
            
            int halfList = (IngredientsSt.Length) / 2;
            for (int i = 0; i < halfList; i++) IngHost.Add(IngredientsSt[i]); 
            for (int i = halfList; i < IngredientsSt.Length; i++) IngClnt.Add(IngredientsSt[i]);
        }         
    }

    private void IngStatChanged(SyncListBool.Operation op, int index){
        GameObject IngtoDestroy = GameObject.Find(Ingredients[index]);
        Destroy(IngtoDestroy);

        if (IngHost.Contains(Ingredients [index])) {
            htrialID++;

            if (isServer) {
                LogIt("<logTactWiz><host><answer>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";1;" + Ingredients [index] + ";" + Ingredients [index]);
                if (htrialID < IngHost.Count) LogIt("<logTactWiz><host>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";Trial;" + htrialID + ";" + IngHost [htrialID]);
            }
        }
        else if (IngClnt.Contains(Ingredients [index])) {
            ctrialID++;

            if (isServer) {
                LogIt("<logTactWiz><client><answer>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";1;" + Ingredients [index] + ";" + Ingredients [index]);
                if (ctrialID < IngClnt.Count) LogIt("<logTactWiz><client>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";Trial;" + ctrialID + ";" + IngClnt [ctrialID]);
            }
        }
    }   
    
    private void IngHoldingChanged(SyncListBool.Operation op, int index){
        if (IngHolding [index]) SelectOther(index);
        else resetLight(); 
    }  

    private void GestureChanged(SyncListBool.Operation op, int index){
        if (TriggerGest [index]) ArticulateIt(index);
        else resetVibes(index);                          
    }

    void ArticulateIt(int index) {
        isTalking = true;
        string ingpointed = "";

        if (ingID != (-1)) {
            Vibration.GetComponent<Vibration>().targetname = Ingredients[ingID];
            ingpointed = ";" +Ingredients [ingID];    
        } 

        if(index == 2 && ingID != (-1)) Vibration.GetComponent<Vibration>().gestStat = index;   
        if(index != 2) Vibration.GetComponent<Vibration>().gestStat = index;
        
        if (isServer) {
            if (hisTalking) LogIt("<logTactWiz><host><said>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";" + GesturesSt [index] + ingpointed);
            else LogIt("<logTactWiz><client><said>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";" + GesturesSt [index] + ingpointed);
        }
    }

    void resetVibes(int index) {
        isTalking = false;
    }

    void SelectOther(int index) {
        if (netObj.GetComponent<NetworkMenu>().netmode == "h") {
            if (IngClnt.Contains(Ingredients [index])) {
                pointselectedother = GameObject.Find(Ingredients [index]);
                if (pointselectedother != null) {
                    Vector3 objpos = new Vector3(pointselectedother.transform.position.x, -14, pointselectedother.transform.position.z);
                    lightother.transform.position = objpos;
                    if (isServer) LogIt("<logTactWiz><client><grab>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";" + Ingredients [index]);
                }
            } else if (isServer) LogIt("<logTactWiz><host><grab>;" + System.DateTime.Now.ToString("h:mm:ss tt") + ";" + Ingredients [index]);
        } else {
            if (IngHost.Contains(Ingredients [index])) {
                pointselectedother = GameObject.Find(Ingredients [index]);
                if (pointselectedother != null) {
                    Vector3 objpos = new Vector3(pointselectedother.transform.position.x, -14, pointselectedother.transform.position.z);
                    lightother.transform.position = objpos;
                }
            }
        }
    }

    void resetLight() {
        Vector3 objpos = new Vector3(OuttaSight.transform.position.x, -14, OuttaSight.transform.position.z);
        lightother.transform.position = objpos;

        if (isServer) LogIt("<logTactWiz><sys><release>;" + System.DateTime.Now.ToString("h:mm:ss tt"));
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

    void OnApplicationQuit( ) {
        if(logFile != null)
            logFile.Close();
    }
}
