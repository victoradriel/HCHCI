using UnityEngine;
using System.Collections;

public class Final : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Application.OpenURL("https://docs.google.com/forms/d/1inQHv0q383IKWtDakPxggCk-sTU8G5mXBaEZ04DE5YA/viewform");
	
	}

	void OnMouseDown() {
		Application.Quit();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Escape))
			Application.Quit();
		
		if(Input.GetKey(KeyCode.Space))
			OnMouseDown();
	
	}
}
