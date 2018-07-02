using UnityEngine;
using System.Collections;

public class SelectOptQuestions : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {
		
		if(this.renderer.material.color == Color.green){
			this.renderer.material.color = Color.white;
		}else{
			this.renderer.material.color = Color.green;
		}
	}
}
