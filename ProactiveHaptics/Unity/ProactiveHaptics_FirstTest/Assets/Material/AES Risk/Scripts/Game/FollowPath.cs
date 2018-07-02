using UnityEngine;
using System.Collections;

public class FollowPath : MonoBehaviour {

	public Transform mapSelected;
	public int mapIndex = 1;

	private Transform pathRoot;

	// Use this for initialization
	void Start () {
		if (mapSelected == null) {
			Debug.LogError("A map must be provided!");
		} else {
			pathRoot = mapSelected.FindChild("Path"+mapIndex);
			if (pathRoot == null) {
				Debug.LogError("Path with index " + mapIndex + " not found!");
			} else {
				foreach(Transform waypoint in pathRoot) {
					Debug.Log (waypoint.name);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
