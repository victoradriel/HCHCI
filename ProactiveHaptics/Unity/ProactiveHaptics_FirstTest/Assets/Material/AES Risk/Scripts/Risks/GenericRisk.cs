using UnityEngine;
using System.Collections;

// This is the generic Risk 
public abstract class GenericRisk : MonoBehaviour {
	public string riskName;
	public bool riskFound;
	public RiskType type;

	public float riskTime = 0f;
	//public float riskFoundSeconds = 0.0f;
	//public int riskFoundMinutes = 0;
	

	public GenericRisk() {
		riskFound = false;
	}

	public void SetEnabled(bool value) {
		if (value)
			EnableRisk();
		else 
			DisableRisk();
	}

	public abstract void EnableRisk();
	public abstract void DisableRisk();

	public abstract void setRiskFound (bool value);

	public void toggleRiskFound() {
		riskFound = !riskFound;
	}
};
