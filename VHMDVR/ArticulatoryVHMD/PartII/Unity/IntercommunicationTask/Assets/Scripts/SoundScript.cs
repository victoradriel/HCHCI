using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour {
    public AudioClip sound01;
    public AudioClip sound02;
    private AudioSource source;
    bool stopf = false;
    public bool setStart = false;
    public bool setStop = false;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
        source.loop = true;
        source.volume = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (source.isPlaying && !stopf && source.volume < 1) source.volume += 0.001f;
        if (setStart && !source.isPlaying) { source.clip = sound01; source.Play(); }

        if (setStop && source.isPlaying) stopf = true;
        if (source.isPlaying && stopf && source.volume > 0) source.volume -= 0.01f;
        if (stopf && source.volume <= 0) { source.Stop(); stopf = false; }
	}
}
