using UnityEngine;
using System.Collections;

public class BlinkingLight : MonoBehaviour {

	public Light referenceLight;

	void TriggerLight()
	{
		referenceLight.enabled = !referenceLight.enabled;
	}

	// Use this for initialization
	void Start () {
		InvokeRepeating("TriggerLight", 0f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
