using UnityEngine;
using System.Collections;

public class GrabTest : MonoBehaviour {
	private MoveFingers 	glove = null;
	GameObject DebugText;

	// Use this for initialization
	void Start () {
		DebugText = GameObject.Find("Output");
	
	}
	
	// Update is called once per frame
	void Update () {
		if (glove.isCloseLeftHand ()) {
			// esquerda fechada
			DebugText.GetComponent<GUIText>().text = "esquerda fechada";
		} else if (!glove.isCloseLeftHand ()) {
			// esquerda aberta
			DebugText.GetComponent<GUIText>().text = "esquerda aberta";
		} else if (glove.isCloseRightHand ()) {
			// direita fechada
			DebugText.GetComponent<GUIText>().text = "direita fechada";
		} else if (!glove.isCloseRightHand ()) {
			// direita aberta
			DebugText.GetComponent<GUIText>().text = "direita aberta";
		} else {
			DebugText.GetComponent<GUIText>().text = "fuck this shit!";
		}
	}
}
