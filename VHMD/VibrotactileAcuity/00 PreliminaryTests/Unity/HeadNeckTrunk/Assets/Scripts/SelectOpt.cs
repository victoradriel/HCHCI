using UnityEngine;
using System.Collections;

public class SelectOpt : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void DesableOther(string option){
		GameObject other;
		switch(option){
		case "TrialOneFirst":
			other = GameObject.Find("TrialOneSecond");
			other.guiText.material.color = Color.white;
			break;
		case "TrialOneSecond":
			other = GameObject.Find("TrialOneFirst");
			other.guiText.material.color = Color.white;
			break;
		case "TrialTwoFirst":
			other = GameObject.Find("TrialTwoSecond");
			other.guiText.material.color = Color.white;
			break;
		case "TrialTwoSecond":
			other = GameObject.Find("TrialTwoFirst");
			other.guiText.material.color = Color.white;
			break;
		case "TrialThreeFirst":
			other = GameObject.Find("TrialThreeSecond");
			other.guiText.material.color = Color.white;
			break;
		case "TrialThreeSecond":
			other = GameObject.Find("TrialThreeFirst");
			other.guiText.material.color = Color.white;
			break;
		}
	}

	void OnMouseDown() {

		if(this.guiText.material.color == Color.green){
			this.guiText.material.color = Color.white;
		}else{
			this.guiText.material.color = Color.green;
			DesableOther(this.name);
		}
	}
}
