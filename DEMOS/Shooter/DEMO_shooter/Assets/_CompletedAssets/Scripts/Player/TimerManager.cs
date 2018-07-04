using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour {
    Text text;
    float timeLeft = 30.0f;
    public GameObject score;
    Text scoretext;

    // Use this for initialization
    void Start () {
        text = GetComponent<Text>();

        score = GameObject.Find("ScoreText");
        scoretext = score.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        timeLeft -= Time.deltaTime;
        text.text = "Time: " + Math.Round(timeLeft,0);
        if (timeLeft <= 0){
            text.text = "Time is Up!";
            if (timeLeft <= -3){
                text.color = Color.red;
            }
        }
        

    }
}
