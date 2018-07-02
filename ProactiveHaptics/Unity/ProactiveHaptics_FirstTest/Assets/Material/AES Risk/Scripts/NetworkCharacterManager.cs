using UnityEngine;
using System.Collections;

public class NetworkCharacterManager : MonoBehaviour {

	public FPSInputController localControler;

	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		// This is my own player
		if(GetComponent<NetworkView>().isMine)
		{
			FPSInputController gmControler = GetComponent<FPSInputController>();
			gmControler.enabled = true;
			localControler = gmControler;

			if(this.tag != "FPSControllerKinect")
				GetComponent<objectSelector>().enabled = true;

			gmControler.GetComponentInChildren<MouseLook>().enabled = true;

			Camera[] allCamChilds = gmControler.GetComponentsInChildren<Camera>();
			foreach(Camera child in allCamChilds)
			{
				Debug.Log("name" + child.gameObject.name);

				child.enabled = true;
				child.nearClipPlane = 0.001f;
				child.gameObject.SetActive(true);
			}
		}
		else // This is just some remote controlled player
		{
			name += "Remote";

			FPSInputController gmControler = GetComponent<FPSInputController>();
			gmControler.enabled = false;

			if(this.tag != "FPSControllerKinect")  
				GetComponent<objectSelector>().enabled = false;

			gmControler.GetComponentInChildren<MouseLook>().enabled = false;

			Camera[] allCamChilds = gmControler.GetComponentsInChildren<Camera>();
			foreach(Camera child in allCamChilds)
			{
				Debug.Log("name" + child.gameObject.name);
				
				child.enabled = false;
				child.gameObject.SetActive(false);
			}
		}
	}
}
