using UnityEngine;
using System.Collections;

public class addColliderRecursively : MonoBehaviour {

	// Use this for initialization
	void Start () {
		addColliders(transform);
	}

	void addColliders(Transform trans)
	{
		if (trans.GetComponent<MeshFilter>() != null && 
		    trans.GetComponent<Collider>() == null && 
		    trans.tag != "DontRequireCollider") 
			trans.gameObject.AddComponent<BoxCollider>();
		if(trans.GetChildCount() > 0)
			foreach(Transform t in trans)
				addColliders(t);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
