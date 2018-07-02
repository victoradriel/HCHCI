using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiskInformation : MonoBehaviour {

	public Dictionary<string, float[]> riskTimes;

	
	// Use this for initialization
	void Awake()
	{
		DontDestroyOnLoad (this);
		riskTimes = new Dictionary<string, float[]> (); // float[0] = seconds; float[1] = minutes
	}


	// Update is called once per frame
	void Update () {
	
		// TEST
				//if (Input.GetKey (KeyCode.T)) 
		//	for(int i=0; i < riskFoundMinutesList.Length; i++)
		//		Debug.Log (" minutes list: " + riskFoundMinutesList[i] + "seconds list: " + riskFoundSecondsList[i]);

		// Test: Loads the "report screen" level.
		if (Input.GetKeyDown (KeyCode.R)) 
		{
			foreach(var entry in riskTimes)
				Debug.Log (entry.Key + " " + entry.Value);


			//Application.LoadLevel(5);
		}
	}

}
