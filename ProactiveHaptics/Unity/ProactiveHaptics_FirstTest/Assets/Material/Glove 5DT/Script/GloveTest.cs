using UnityEngine;
using System;
using System.Collections;
using FDTGloveUltraCSharpWrapper;

public class GloveTest : MonoBehaviour 
{
	
	CfdGlove leftHandGuanteObj;
	CfdGlove rightHandGuanteObj;
	
	// Use this for initialization
	void Start () {
	
		leftHandGuanteObj = new CfdGlove();
		try{
			leftHandGuanteObj.Open("USB0");
			GetComponent<Light>().intensity = 5.0f;
		} catch {
			GetComponent<Light>().intensity = 1.0f;
		}

		try{
			rightHandGuanteObj.Open ("USB1");
			GetComponent<Light>().intensity = 5.0f;
		} catch {
			GetComponent<Light>().intensity = 1.0f;
		}

		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(leftHandGuanteObj.IsOpen())
		{
			int gesture = leftHandGuanteObj.GetGesture();
			GetComponent<Light>().color = Color.green;
			if (gesture.Equals(0))
			{
				GetComponent<Light>().color = Color.red;
			} else if (gesture.Equals(1)) {
				GetComponent<Light>().color = Color.yellow;
			}
		} else {
			if (Input.GetKey("1"))
			{
				GetComponent<Light>().color = Color.green;
			} else if (Input.GetKey("2")) {
				GetComponent<Light>().color = Color.yellow;
			} else if (Input.GetKey("3")) {
				GetComponent<Light>().color = Color.red;
			} else if (Input.GetKey("0")) {
				GetComponent<Light>().color = Color.white;
			}
		}
	
	}
	
	void OnGUI () 
	{
		string s;
		int offset = 20;
		for (int i = 0; i < 16; i++)
		{
			s = "Sensor " + i.ToString() + " scaled value = " + leftHandGuanteObj.GetSensorScaled(i).ToString();
			GUI.Label(new Rect(10,offset,400,20+offset), s);
			offset += 20;
		}
		

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
//		if(GUI.Button(new Rect(20,40,80,20), "Level 1")) {
//			Application.LoadLevel(1);
//		}
//
//		// Make the second button.
//		if(GUI.Button(new Rect(20,70,80,20), "Level 2")) {
//			Application.LoadLevel(2);
//		}
	}
}
