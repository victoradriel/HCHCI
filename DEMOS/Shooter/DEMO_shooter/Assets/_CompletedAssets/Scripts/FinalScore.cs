using UnityEngine;
using System.Collections;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class FinalScore : MonoBehaviour {
    Text text;
    static string COMPortBelt = "COM3";
    static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
    static bool portopenBelt = false; //if port is open or not
    char[] SendToArduino = new char[] { '0', ',', '0', ',', '0', ',', '1', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0' };

    // Use this for initialization
    void Start () {
        text = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        try { text.text = "Final " + PlayerPrefs.GetString("FinalScore"); }
        catch (Exception e) { text.text = ""; }

        if (Input.GetKey(KeyCode.R)) {
            SceneManager.LoadScene(0);
        }
    }

}
