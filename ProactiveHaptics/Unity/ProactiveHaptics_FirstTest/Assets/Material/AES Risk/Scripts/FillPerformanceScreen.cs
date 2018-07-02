using UnityEngine;
using System.Collections;

public class FillPerformanceScreen : MonoBehaviour {

	public UILabel playerNameLabel;
	public UILabel elapsedTime;
	public Transform riskOptions;
	public Transform[] allRiskOptionsChildren;
	private MenuInfo info; 

	void Awake () {
		info = GameObject.FindGameObjectWithTag("menuInfo").GetComponent<MenuInfo>();
	}

	// Use this for initialization
	void Start () {
		Screen.lockCursor = false;

		playerNameLabel.text = info.playerName;
		elapsedTime.text = formatTimer(Time.realtimeSinceStartup - info.timePlayableLevelLoaded);

		// Get ALL of riskOptions's children (all levels of the children hierarchy)
		allRiskOptionsChildren = riskOptions.GetComponentsInChildren<Transform>();

		foreach (MenuInfo.RiskInfo r in info.riskOptions.Values) {
			foreach (Transform t in allRiskOptionsChildren) 
			{
				// Only continue if the current child has the OptionRiskType script in its components.
				if(t.GetComponent<OptionRiskType>() != null)
				{
					UIToggle toggle = t.gameObject.GetComponent<UIToggle>();
					OptionRiskType type = t.gameObject.GetComponent<OptionRiskType>();
					type.optionLabel = t.gameObject.GetComponentInChildren<UILabel>();


					//Debug.Log ("type.risktype: " + type.riskType + "  r.type: " + r.type);

					if (type.riskType == r.type) 
					{
						toggle.value = r.found;
						type.optionLabel.text = type.optionLabel.text + "  -  " + formatTimer(r.time);
					}
				}
			}
		}
	}

	public string formatTimer(float time) {
		string mins = (Mathf.Floor(time / 60f)).ToString ("00");
		string secs = (time % 60).ToString ("00");

		return mins + ":" + secs;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}

