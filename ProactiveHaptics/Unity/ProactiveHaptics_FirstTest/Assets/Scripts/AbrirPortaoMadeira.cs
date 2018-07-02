using UnityEngine;
using System.Collections;

public class AbrirPortaoMadeira : MonoBehaviour {

	public Transform gateLeft;
	public Transform gateRight;

	public Transform openGateLeft;
	public Transform openGateRight;

	public float openingTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator OpenGate()
	{
		Debug.Log ("opengate foi chamada.");

		float openSpeed;
		if(openingTime == 0f)
			openSpeed = 1f;
		else 
			openSpeed = 1f/openingTime;

		bool leftRotated = false;
		bool rightRotated = false;
		while(!leftRotated || !rightRotated)
		{
			leftRotated = (Quaternion.Angle (gateLeft.rotation, openGateLeft.rotation) < 5f);
			rightRotated = (Quaternion.Angle (gateRight.rotation, openGateRight.rotation) < 5f);

			if(!leftRotated) 
				gateLeft.rotation = Quaternion.Lerp (gateLeft.rotation, openGateLeft.rotation, 1f * Time.deltaTime);
			if(!rightRotated) 
				gateRight.rotation = Quaternion.Lerp (gateRight.rotation, openGateRight.rotation, 1f * Time.deltaTime);
			
			yield return null;
		}

	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			StartCoroutine (OpenGate ());
		}
	}
}
