using UnityEngine;
using System.Collections;

public class PrintControllerNames : MonoBehaviour {

	// Use this for initialization
	void Start () {

		//Debug.Log (Input.GetJoystickNames().Length);
		foreach(string name in Input.GetJoystickNames()) {
			Debug.Log (name);
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
