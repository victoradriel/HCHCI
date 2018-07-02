using UnityEngine;
using System.Collections;

public class Report : MonoBehaviour {
	public UILabel time;
	public UILabel risksC;
	public UILabel risksW;
	public UILabel miss;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


		int minutes = (int)Time.timeSinceLevelLoad / 60;
		int secs = (int)Time.timeSinceLevelLoad % 60;

		time.text = minutes + ":" + secs;

	}
}
