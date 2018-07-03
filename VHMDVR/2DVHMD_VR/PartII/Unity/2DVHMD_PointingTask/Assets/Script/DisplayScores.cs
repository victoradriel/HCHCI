using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayScores : MonoBehaviour {
    GameObject Score1 = null;
    GameObject Score2 = null;
    GameObject Score3 = null;
    Text tScore1 = null;
    Text tScore2 = null;
    Text tScore3 = null;
    int nSc1 = 0;
    int nSc2 = 0;
    int nSc3 = 0;


    // Use this for initialization
    void Start () {
        Score1 = GameObject.Find("Score1");
        Score2 = GameObject.Find("Score2");
        Score3 = GameObject.Find("Score3");

        tScore1 = Score1.GetComponent<Text>();
        tScore2 = Score2.GetComponent<Text>();
        tScore3 = Score3.GetComponent<Text>();

        nSc1 = PlayerPrefs.GetInt("Score1");
        nSc2 = PlayerPrefs.GetInt("Score2");
        nSc3 = PlayerPrefs.GetInt("Score3");

        nSc1 = (nSc1 * 100) / 15;
        nSc2 = (nSc2 * 100) / 15;
        nSc3 = (nSc3 * 100) / 15;
    }
	
	// Update is called once per frame
	void Update () {
        
        tScore1.text = "Score 1: " + nSc1 +"%";
        tScore2.text = "Score 2: " + nSc2 + "%";
        tScore3.text = "Score 3: " + nSc3 + "%";
    }
}
