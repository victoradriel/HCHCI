using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingAt : MonoBehaviour {
    RaycastHit hit;
    Vector3 pointingnorth = new Vector3(0, 0, 10);
    public GameObject pointnow = null;
    public bool islooking = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Physics.Raycast(transform.position, transform.TransformDirection(pointingnorth), out hit)) {
            islooking = true;
            pointnow = GameObject.Find(hit.collider.name);
        }
        else {
            islooking = false;
        }
                
	}
}
