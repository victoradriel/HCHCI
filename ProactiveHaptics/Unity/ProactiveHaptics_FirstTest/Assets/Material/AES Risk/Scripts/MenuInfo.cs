using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuInfo : MonoBehaviour {

	public class RiskInfo {
		public bool active;
		public bool found;
		public float time;
		public RiskType type;

		public RiskInfo (RiskType type, bool active, float time) {
			this.type = type;			            
			this.active = active;
			this.time = time;
			this.found = false;
		}
	};

	public string playerName;	
	public Dictionary<DeviceType, bool> devices;
	public Dictionary<RiskType, RiskInfo> riskOptions;
	public float timePlayableLevelLoaded;
	bool runMethod = true;

	void Awake() {

	
		for (int i=0; i<Input.GetJoystickNames().Length; i++) {
			Debug.Log (Input.GetJoystickNames()[i]);
		}
		DontDestroyOnLoad(this);

		devices = new Dictionary<DeviceType, bool>();		
		devices.Add(DeviceType.KINECT, true);
		devices.Add(DeviceType.DATA_GLOVE, true);
		devices.Add(DeviceType.HMD, true);
		devices.Add(DeviceType.JOYSTICK, true);
		devices.Add(DeviceType.KEYBOARD, true);

		riskOptions = new Dictionary<RiskType, RiskInfo> ();
	}

	public void onRiskOptionChange(UIToggle option, OptionRiskType riskType) {
		if (riskOptions.ContainsKey(riskType.riskType)) {
			riskOptions[riskType.riskType].active = option.value;
		} else {
			riskOptions.Add (riskType.riskType, new RiskInfo(riskType.riskType, option.value, 0f));
		}
	}

	public void onDeviceChange(UIToggle option, OptionDevice optDevice){
		devices[optDevice.device] = option.value;
	}
	
	public void onInputNameChange(UIInput textInput) {
		playerName = textInput.label.text;
	}

	public void copyRisksInfo() {
		ManageRisks info = GameObject.FindGameObjectWithTag("RiskManager").GetComponent<ManageRisks>();
		if (info != null) {
			foreach (GenericRisk gr in info.risks) {
				Debug.Log (gr.type + " " + gr.transform.name);
				riskOptions[gr.type].found = gr.riskFound;
				riskOptions[gr.type].time = gr.riskTime;
				//Debug.Log (gr.type.ToString() + " --- " + riskOptions[gr.type].found.ToString());
			}
		} else {
			Debug.LogError ("menu info not found!");
		}
	}

	// Returns the current time when the level actually loaded.
	// This is needed by the script "FillPerformanceScreen", to set the correct time the player took to play the level.
	void setTimePlayableLevelLoaded()
	{
		timePlayableLevelLoaded = Time.realtimeSinceStartup;
	}

	void Update()
	{
		// OTIMIZAR ESTE CODIGO!!! ESSE UPDATE SO EH NECESSARIO APENAS UMA VEZ!!!
		if(runMethod == true)
		{
			if(Application.loadedLevel == 1 || Application.loadedLevel == 2 ||
			   Application.loadedLevel == 3 || Application.loadedLevel == 4 )
			{
				setTimePlayableLevelLoaded();
				runMethod = false;
			}
		}
	}

}
