using UnityEngine;
using System.Collections;

public class SimpleRisk : GenericRisk {
	public GameObject riskObj;


	// riskEnabled
	public override void EnableRisk() {
		// Debug.Log ("simple Risk: " + riskName + " enabled.");
		if (riskObj == null) riskObj = this.gameObject;
		riskObj.SetActive(true);
	}
		
	// riskDisabled
	public override void DisableRisk() {
		// Debug.Log ("simple Risk: " + riskName + " disabled.");
		if (riskObj == null) riskObj = this.gameObject;
		riskObj.SetActive(false);
	}
		
	public override void setRiskFound (bool value) {
		//Debug.Log ("Risk Found! " + riskName);
		riskFound = value;
		riskTime = Time.timeSinceLevelLoad;
	}

	void Start() {
		if (riskObj == null) riskObj = this.gameObject;
	}
}
