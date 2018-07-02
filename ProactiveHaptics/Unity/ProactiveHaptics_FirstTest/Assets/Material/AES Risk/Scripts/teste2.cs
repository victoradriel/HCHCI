using UnityEngine;
using System.Collections;

public class teste2 : MonoBehaviour {

	int score = 0 ;
	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnControllerColliderHit(ControllerColliderHit obj){

		if(obj.gameObject.name == "Coin"){
			//AudioSource.PlayClipAtPoint(coinSound, transform.position);
			score++;			
			Destroy(obj.gameObject);
			Debug.Log(score);
		}


		
	}
}
