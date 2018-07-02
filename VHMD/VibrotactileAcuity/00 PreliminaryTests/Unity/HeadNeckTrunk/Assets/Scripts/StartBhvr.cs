using UnityEngine;
using System.Collections;

public class StartBhvr : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnMouseDown() {
		int playAllow = PlayerPrefs.GetInt("Play");
		if(playAllow == 1){
			Application.LoadLevel(1);
		} else{
			GameObject TextId = GameObject.Find("TextId");
			TextId.guiText.material.color = Color.red;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
