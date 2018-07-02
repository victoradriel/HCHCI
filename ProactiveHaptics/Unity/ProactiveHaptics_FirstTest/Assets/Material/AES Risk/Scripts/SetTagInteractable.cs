using UnityEngine;
using System.Collections;

public class SetTagInteractable : MonoBehaviour {

	// Use this for initialization
	void Awake () 
	{
		// Set the tags of this object's children to "Interactable".
		AddTagRecursively (transform, "Interactable");
	}


	void AddTagRecursively(Transform trans, string tag)
	{
		trans.gameObject.tag = tag;
		if(trans.GetChildCount() > 0)
			foreach(Transform t in trans)
				AddTagRecursively(t, tag);
	}
}
