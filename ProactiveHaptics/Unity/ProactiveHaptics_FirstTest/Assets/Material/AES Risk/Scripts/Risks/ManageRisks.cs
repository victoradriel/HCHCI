using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManageRisks : MonoBehaviour {
	
	public GameObject[] gameobjects;
	public GenericRisk[] risks;

	private MenuInfo info;
	//public bool[] menuOptions;


	void Awake () {
		GameObject menuInfo = GameObject.FindGameObjectWithTag("menuInfo") as GameObject;
		if (menuInfo != null) {
			info = menuInfo.GetComponent<MenuInfo>();
		}


		//menuOptions = info.riskOptions;


		// Stores in the Risk Manager all the existing risk objects in the scene.
		gameobjects = GameObject.FindGameObjectsWithTag("Riscos");
		risks = new GenericRisk[gameobjects.Length];

		for(int i = 0; i < gameobjects.Length; i++)
		{
			risks[i] = gameobjects[i].GetComponent<GenericRisk>();
		}
	}

	void Start () {
		if (info != null) {
			foreach (MenuInfo.RiskInfo r in info.riskOptions.Values) {
				foreach (GenericRisk risk in risks) {
					if (risk.type == r.type) {
						risk.SetEnabled(r.active);
					}
				}

			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
