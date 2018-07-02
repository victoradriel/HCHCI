using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DefaultInformation : MonoBehaviour {

	public List<Material> defaultMaterialList;

	// Use this for initialization
	void Start () {
		if(this.gameObject.GetComponent<Renderer>() != null)
		{
			defaultMaterialList = new List<Material>();
			foreach(Material mat in gameObject.GetComponent<Renderer>().materials)
			{
				defaultMaterialList.Add(new Material(mat));
			}
		}

	}

	public void AddNetworkSync()
	{
		gameObject.AddComponent<NetworkView>();
		gameObject.GetComponent<NetworkView>().viewID = Network.AllocateViewID();
		gameObject.GetComponent<NetworkView>().stateSynchronization = NetworkStateSynchronization.Unreliable;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
