using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleObject : MonoBehaviour {

	[System.Serializable]
	public class ActionRisk {
		public RiskType risk;
		public bool mustExecute;
		public bool toggle = true;
		public GameObject obj;
		public bool keepRotation = true;
		public Vector3 newRotation;
		public Transform changeObjectParent;
		public Material changeMaterial;
		public float inSeconds;
	};

	[System.Serializable]
	public class Action {
		public bool toggle = true;
		public GameObject obj;
		public Material changeMaterial;
		public bool keepRotation = true;
		public Vector3 newRotation;
		public Transform changeObjectParent;
		public float inSeconds;
	};

	public float delayAnimation = 0;

	public Action[] listOfActions;
	public ActionRisk[] listOfRiskActions;

	// Use this for initialization
	void Start () {
		foreach (Action a in listOfActions) {
			StartCoroutine(enableDisableObject(a));
		}
		foreach (ActionRisk a in listOfRiskActions) {
			StartCoroutine(enableDisableObject(a));
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (Time.timeSinceLevelLoad);
	}

	IEnumerator enableDisableObject(Action obj) {
		yield return new WaitForSeconds(obj.inSeconds + delayAnimation);
		if (obj.toggle) {
			if(obj.obj.GetComponent<MeshRenderer>() != null) obj.obj.GetComponent<MeshRenderer>().enabled = !obj.obj.GetComponent<MeshRenderer>().enabled;
			if(obj.obj.GetComponent<Collider>() != null) obj.obj.GetComponent<Collider>().enabled = !obj.obj.GetComponent<Collider>().enabled;
		}
		changeMaterial(obj.obj, obj.changeMaterial);
		changeParent(obj.obj, obj.changeObjectParent, obj.keepRotation, obj.newRotation);
	}

	IEnumerator enableDisableObject(ActionRisk obj) {
		yield return new WaitForSeconds(obj.inSeconds + delayAnimation);
		if (obj.toggle) {
			if(obj.obj.GetComponent<MeshRenderer>() != null) obj.obj.GetComponent<MeshRenderer>().enabled = !obj.obj.GetComponent<MeshRenderer>().enabled;
			if(obj.obj.GetComponent<Collider>() != null) obj.obj.GetComponent<Collider>().enabled = !obj.obj.GetComponent<Collider>().enabled;
		}

		changeMaterial(obj.obj, obj.changeMaterial);
		changeParent(obj.obj, obj.changeObjectParent, obj.keepRotation, obj.newRotation);
	}

	public void changeMaterial(GameObject obj, Material mat) {
		if (mat != null) {
			MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
			if (renderer != null) {
				renderer.material = mat;
			}
			SkinnedMeshRenderer skinnedRenderer = obj.GetComponent<SkinnedMeshRenderer>();
			if (skinnedRenderer != null) {
				skinnedRenderer.material = mat;
			}
		}
	}

	public void changeParent(GameObject obj, Transform newParent, bool rotation, Vector3 newRot) {
		if (newParent != null) {
			obj.transform.parent = newParent;
			if (!rotation) obj.transform.localRotation = Quaternion.Euler (newRot);
			obj.transform.localPosition = new Vector3(0,0,0);
		}
	}

}
