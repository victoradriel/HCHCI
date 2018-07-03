using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	public float X = 0F;
	public float Y = 0F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		X = Input.GetAxis ("Horizontal");
		Y = Input.GetAxis ("Vertical");

		if (X >= -1 && X < -0.5){

			if(Y <= 1 && Y > 0.5) print ("NW");
			if (Y <= 0.5 && Y >= -0.5) print("West");
			if(Y < -0.5 && Y >= -1) print ("SW");
		} 
		else if (X >= -0.5 && X <= 0.5){

			if(Y <= 1 && Y >= 0.5) print ("North");
			if(Y <= -0.5 && Y >= -1) print ("South");
		}
		else if (X > 0.5 && X <= 1){

			if(Y <= 1 && Y > 0.5) print ("NE");
			if (Y <= 0.5 && Y >= -0.5) print("East");
			if(Y < -0.5 && Y >= -1) print ("SE");
		}

		if (Input.GetButtonDown("JoystickButton0"))
			print ("Epaaaaaa");

	}
}
