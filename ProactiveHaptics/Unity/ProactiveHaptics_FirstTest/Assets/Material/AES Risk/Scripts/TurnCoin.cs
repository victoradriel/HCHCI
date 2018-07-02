using UnityEngine;
using System.Collections;

public class TurnCoin : MonoBehaviour {

	int score =0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.Rotate(0, 0, 50 * Time.deltaTime);
	}


	void OnControllerColliderHit(ControllerColliderHit obj){
		print("srdrsdr");
		if(obj.gameObject.name == "Coin"){
			//AudioSource.PlayClipAtPoint(coinSound, transform.position);
			score++;			
			Destroy(obj.gameObject);
		}
	
	}
}
