using UnityEngine;
using System.Collections;

public class TimerScript : MonoBehaviour {

	public float timerSeconds = 0.0f;
	public int timerMinutes = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		// Time.deltaTime increases the value by 1 in every second.
		timerSeconds += Time.deltaTime;

		if (timerSeconds >= 60.0f) 
		{
			timerMinutes += 1;
			timerSeconds = 0.0f;
		}

		//Debug.Log(timerMinutes + " minutes and " + timerSeconds +  " seconds have passed.");
	}
}
