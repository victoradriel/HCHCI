using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class NetworkMenu : MonoBehaviour {

    Text mode;
    Text modeID;
    Text userID;
    GameObject objmode;
    GameObject objmodeID;
    GameObject objuserID;
    GameObject fieldip;
    GameObject fieldipph;
	public NetworkManager thisnetm;
    public int slctMode = 0;
    public string dyadID = "";
    public string netmode = "";
    public bool setupdone = false;
    public bool scenechange = false;

	// Use this for initialization
	void Start () {
        UnityEngine.VR.InputTracking.Recenter();
        thisnetm = gameObject.GetComponent<NetworkManager>();

        objmode = GameObject.Find("Mode");
        mode = objmode.GetComponent<Text>();
        objmodeID = GameObject.Find("modeID");
        modeID = objmodeID.GetComponent<Text>();
        objuserID = GameObject.Find("userID");
        userID = objuserID.GetComponent<Text>();

        fieldip = GameObject.Find("fieldip");
        fieldipph = GameObject.Find("PlaceholderIP");
        fieldipph.GetComponent<Text>().text = thisnetm.networkAddress;

        setupdone = false;
        scenechange = false;
	}
	
	// Update is called once per frame
	void Update () {

        

        if (Input.GetKey(KeyCode.KeypadEnter)) {
            dyadID = userID.text;
            netmode = modeID.text;
            mode.text = "";

            if (fieldip.GetComponent<Text>().text.Length > 0) thisnetm.networkAddress = fieldip.GetComponent<Text>().text;
            else thisnetm.networkAddress = "192.168.1.100";

            if (slctMode == 0) {
                switch (netmode) {
                    case "c":
                        slctMode = 1;
                        thisnetm.StartClient();                        
                        break;
                    case "h":
                        slctMode = 2;
                        thisnetm.StartHost();
                        break;
                    case "s":
                        slctMode = 3;
                        thisnetm.StartServer();
                        break;
                    default:
                        mode.text = "Please, enter mode.";
                        break;
                }
            }
            else if (dyadID != "") {
                mode.text = "Setup done";
                setupdone = true;
                
            }
            else {
                mode.text = "Oxe, cade o ID?";
            }
            
        }

        if (Input.GetKey(KeyCode.Keypad1) && setupdone && !scenechange) {
            if (GameObject.FindGameObjectsWithTag("CubePlayer").Length > 0) {
                scenechange = true;
                thisnetm.ServerChangeScene("Treino");
            }
        }  
        
        if (Input.GetKey(KeyCode.Keypad0) && setupdone && !scenechange) {
            if (GameObject.FindGameObjectsWithTag("CubePlayer").Length > 0) {
                scenechange = true;
                thisnetm.ServerChangeScene("SceneA");
            }
        }     
	}
    
    void OnApplicationQuit( ) {
        print("closing");
        switch (slctMode) {
            case 1:
                thisnetm.StopClient();
                break;
            case 2:
                thisnetm.StopHost();
                break;
            case 3:
                thisnetm.StopServer();
                break;
        }
    }
}
