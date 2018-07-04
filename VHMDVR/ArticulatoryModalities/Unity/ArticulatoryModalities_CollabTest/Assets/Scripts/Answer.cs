using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer : MonoBehaviour {
    public string overthecauldron = "";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        //print("Collision detected with trigger object " + other.name);
        overthecauldron = other.name;
    }
}
