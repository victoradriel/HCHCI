using UnityEngine;
using System.Collections;
using System;

public class MovingHead : MonoBehaviour {

    //private double test;
    public Vector3 teste;
    public Vector3 offset;
    public Vector3 bratrackOutput;
    public float translationFactor;

    // Use this for initialization
    void Start() {

        //test = BratrackUDPReceiver.TX;
        //teste = new Vector3(0,0,0);
        teste = new Vector3(0, 0, 0);
        offset = new Vector3(0, 0, 0);
        bratrackOutput = new Vector3(0, 0, 0);
        translationFactor = 0.001f;

    }

    // Update is called once per frame
    void Update() {
        bratrackOutput.Set((float)BratrackUDPReceiver.TX, (float)BratrackUDPReceiver.TY, (float)BratrackUDPReceiver.TZ);

        float tx = (float)BratrackUDPReceiver.TX;
        float ty = (float)BratrackUDPReceiver.TY;
        float tz = (float)BratrackUDPReceiver.TZ;

        // checar se não é extremamente baixo
        print("BRA: " + tx);

        if (Input.GetKey(KeyCode.A)) 
            offset.Set(tx, ty, tz);

        if (Input.GetKey(KeyCode.PageUp))
            translationFactor += 0.001f;

        if (Input.GetKey(KeyCode.PageDown))
            translationFactor -= 0.001f;

        float ttx = (offset.x - tx) * translationFactor;
        float tty = (offset.y - ty) * translationFactor;
        float ttz = (offset.z - tz) * translationFactor;

        teste.Set(0, tty, ttz);
        transform.localPosition = teste;
    }
}
