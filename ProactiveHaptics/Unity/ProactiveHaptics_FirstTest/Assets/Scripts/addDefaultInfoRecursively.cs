using UnityEngine;
using System.Collections;

public class addDefaultInfoRecursively : MonoBehaviour {
	

	// Use this for initialization
	void Start () {
		addDefaultInfo(transform);
	}

	void addDefaultInfo(Transform trans)
	{
		trans.gameObject.AddComponent<DefaultInformation>();

		if(trans.GetChildCount() > 0)
			foreach(Transform t in trans)
				addDefaultInfo(t);
	}

	// Update is called once per frame
	void Update() {
	
	}
}
