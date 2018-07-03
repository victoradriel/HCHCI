using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dontdoit : MonoBehaviour {

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
