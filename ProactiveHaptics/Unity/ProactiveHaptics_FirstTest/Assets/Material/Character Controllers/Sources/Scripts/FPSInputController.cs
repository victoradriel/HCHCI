using UnityEngine;
using System.Collections;

// Require a character controller to be attached to the same game object
[RequireComponent (typeof (CharacterMotorC))]
[AddComponentMenu("Character/FPS Input Controller C")]


public class FPSInputController : MonoBehaviour {
	private CharacterMotorC cmotor;

	public string axisNameX = "Horizontal";
	public string axisNameY = "Vertical";

	private GameObject objectHit; // The object that the raycast hit, to be selected.
	private Color oldColor; // Stores the original color of the object, before it was selected.


	// Use this for initialization
	void Awake() {
		cmotor = GetComponent<CharacterMotorC>();
	}
	
	// Update is called once per frame
	void Update () {

		if(GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isGameMultiplayer == true)
		{
			if(GetComponent<NetworkView>().isMine) // This test is needed to only control MY character during network play! (to not control OTHER players's characters)
			{
				ProcessPlayerInputs();
			}
		}
		else
			ProcessPlayerInputs ();
	}

	void ProcessPlayerInputs()
	{
		// Get the input vector from keyboard or analog stick
		Vector3 directionVector;

		directionVector = new Vector3(Input.GetAxis(axisNameX), 0, Input.GetAxis(axisNameY));
		
		if (directionVector != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		
		if(Mathf.Abs(Input.GetAxis(axisNameY)) > 0.25)
		{
			int directionMultiplier;
			
			if(directionVector.z > 0)
				directionMultiplier = 1;
			else
				directionMultiplier = -1;
			
			cmotor.inputMoveDirection = Vector3.Scale (Camera.main.transform.forward, new Vector3(directionMultiplier, directionMultiplier, directionVector.z));
		}
		else
			cmotor.inputMoveDirection = Vector3.Scale(Camera.main.transform.forward, Vector3.zero);
		
		float horizontalAxis = Input.GetAxis (axisNameX);
		if(Mathf.Abs (horizontalAxis) > 0.4)
		{
			float axisSignal = (horizontalAxis / Mathf.Abs(horizontalAxis));
			float scaledAxis = (float)(horizontalAxis - (0.4 * axisSignal)) / 0.6f; 
			transform.Rotate(new Vector3(0, 100 * Time.deltaTime * scaledAxis * scaledAxis * axisSignal, 0));
		}
		
		//cmotor.inputJump = Input.GetButton("Jump");
		
		
		// If the player clicked the mouse button, select the object aimed with the crosshair.
		/*if (Input.GetButtonDown("selectionbutton")) 
		{
			// Casts a ray forward from the camera's position.
			Vector3 fwd = Camera.main.transform.forward;
			RaycastHit myHit = new RaycastHit();

			// If the ray hits an "interactable" object, change it's color to mark it as selected.
			if(Physics.Raycast(Camera.main.transform.position, fwd, out myHit, 1000.0f) 
			   && myHit.transform.tag == "Interactable")
			{
				// Stores a reference to the object hit by the ray.
				objectHit = myHit.transform.gameObject;

				// If the object's material has the color propriety...
				if(objectHit.renderer.material.HasProperty("_Color"))
				{			
					if(objectHit.renderer.material.color != Color.red)
					{
						// Save the previous color...
						oldColor = objectHit.renderer.material.GetColor("_Color");
						// ...and change the current color, to identify that the object is currently selected.
						objectHit.renderer.material.color = Color.red;
						
						//if(object.tag == "Riscos")
						//audio.Play();		
						//else
						//audio.Play();
					}
					else if(objectHit.renderer.material.color == Color.red)
					{
						// If the object was already selected, unselects the object.
						objectHit.renderer.material.color = oldColor;
					}
				}
			}
		}*/
	}
	
}