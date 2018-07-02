using UnityEngine;
using System.Collections;

public class AppClose : MonoBehaviour {
	
	public void OnClick () {
		closeApp();
	}
	
	void closeApp() {
		Application.Quit();
	}
}
