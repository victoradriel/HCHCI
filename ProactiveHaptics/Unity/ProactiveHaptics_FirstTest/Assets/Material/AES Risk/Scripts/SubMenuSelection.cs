using UnityEngine;
using System.Collections;

public class SubMenuSelection : MonoBehaviour {

	[System.Serializable]
	public class SubMenuInfo {
		public string name;
		public GameObject obj;
	};

	public SubMenuInfo[] subMenus;

	public GameObject[] objectsToHide;

	public void Start() {
		openAllSubMenus();
		closeAllSubMenus();
		subMenus[0].obj.SetActive(true);
	}

	public void Update() {
		hideButtons();
	}

	public void showSubmenu(GameObject btn) {
		closeAllSubMenus();
		foreach (SubMenuInfo m in subMenus) {
			if (m.name == btn.name) {
				m.obj.SetActive(true);
			}
		}
	}

	public void closeAllSubMenus() {
		foreach (SubMenuInfo m in subMenus) {
			m.obj.SetActive(false);
		}
	}

	public void openAllSubMenus() {
		foreach (SubMenuInfo m in subMenus) {
			m.obj.SetActive(true);
		}
	}


	public void hideButtons() {
		foreach (GameObject g in objectsToHide) {
			g.SetActive(false);
		}
	}
}
