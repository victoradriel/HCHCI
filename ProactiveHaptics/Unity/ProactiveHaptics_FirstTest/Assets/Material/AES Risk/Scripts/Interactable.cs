using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour {

	void OnTriggerEnter( Collider other ){

		if (other.tag == "Hand") {
			Debug.Log ("Entering Tag If");

			gameObject.GetComponent<Renderer>().material.color = Color.red;

		}

	}
}
