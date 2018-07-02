using UnityEngine;
using System.Collections;

public class DoNothingRisk : GenericRisk {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// riskEnabled
	public override void EnableRisk() {
		// do nothing
	}
	
	// riskDisabled
	public override void DisableRisk() {
		// do nothing
	}

	public override void setRiskFound (bool value) {
		//Debug.Log ("Risk Found! " + riskName);
		riskFound = value;
		riskTime = Time.timeSinceLevelLoad;
	}
}
