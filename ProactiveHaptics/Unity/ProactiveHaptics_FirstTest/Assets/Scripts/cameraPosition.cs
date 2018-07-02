using UnityEngine;
using System.Collections;

public class cameraPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		NetworkManager netMng = GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ();

		if(netMng.isGameMultiplayer == true)
		{
			if(GetComponent<NetworkView>().isMine) // This test is needed to only control MY character during network play! (to not control OTHER players's characters)
			{
				moveCamera();
			}
		}
		else
			moveCamera();
	}

	void moveCamera()
	{
		if (Camera.main)
		{
			Camera.main.transform.position = transform.position;

		}
	}
}
