using UnityEngine;
using System.Collections;

// Require a character controller to be attached to the same game object
[RequireComponent (typeof (CharacterMotorC))]
[AddComponentMenu("Character/FPS Input Controller C")]

public class MouseKeyboardController : MonoBehaviour {
	private CharacterMotorC cmotor;

	// Use this for initialization
	void Awake() {
		cmotor = GetComponent<CharacterMotorC>();
	}

	// Update is called once per frame
	void Update () {
		// Get the input vector from keyboard or analog stick
		Vector3 directionVector;		
		directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	}
}
