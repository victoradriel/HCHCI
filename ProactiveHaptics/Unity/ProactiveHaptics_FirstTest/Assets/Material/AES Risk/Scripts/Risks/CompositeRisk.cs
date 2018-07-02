using UnityEngine;
using System.Collections;

// switch between two objects enabled/disabled (both cannot be enabled or disabled at the same time).
public class CompositeRisk : GenericRisk {
	public GameObject riskObject;
	public GameObject safeObject;

	//public TimerScript timer;
	//public ManageRisks riskManager;

	// riskEnabled
	public override void EnableRisk() {
		riskObject.SetActive(true);
		safeObject.SetActive(false);
		//Debug.Log ("simple Risk: " + riskName + " enabled.");
	}
	
	// riskDisabled
	public override void DisableRisk() {
		riskObject.SetActive(false);
		safeObject.SetActive(true);
		//Debug.Log ("simple Risk: " + riskName + " disabled.");
	}

	public override void setRiskFound (bool value) {
		Debug.Log ("Risk Found! " + riskName);
		riskFound = value;

		riskTime = Time.timeSinceLevelLoad;
		
		// Stores in a dictionary the time taken to mark this risk.
		//riskManager.StoreRiskTime (this);
	}

}
