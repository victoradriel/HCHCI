using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GloveGestureManager : GestureManager
{
	private float[] MinFingersLValues;
	private float[] MinFingersRValues;
	private float[] MaxFingersLValues;
	private float[] MaxFingersRValues;
	private Glove 	TheGlove;

	public GloveGestureManager (){
		MinFingersLValues = new float[5];
		MinFingersRValues = new float[5];
		MaxFingersLValues = new float[5];
		MaxFingersRValues = new float[5];
	}

	public void SetGlove(Glove pglove)
	{ 
		TheGlove = pglove;
	}

	public override Gesture CaptureGesture ()
	{
		float[] fingersLData = new float[5];
		float[] fingersRData = new float[5];
		float[] fingersGestureData = new float[10];
		TheGlove.GetFingerData (MoveFingers.Hand_Type.LEFT_HAND,fingersLData);
		TheGlove.GetFingerData (MoveFingers.Hand_Type.RIGHT_HAND,fingersRData);

		int i = 0;

		for(i=0;i<5;i++){
			fingersGestureData[i] = (fingersLData[i] - MinFingersLValues[i])/(MaxFingersLValues[i]-MinFingersLValues[i]);
			fingersGestureData[i+5] = (fingersRData[i] - MinFingersRValues[i])/(MaxFingersRValues[i]-MinFingersRValues[i]);
		}

		for(i=0;i<10;i++){
			if(fingersGestureData[i]>1)
				fingersGestureData[i] = 1;
			if(fingersGestureData[i]<0)
				fingersGestureData[i] = 0;
		}

		Gesture capturedGesture = new Gesture(fingersGestureData);
		return capturedGesture;
	}

	public void CaptureClosePose()
	{
		TheGlove.GetFingerData (MoveFingers.Hand_Type.LEFT_HAND,MinFingersLValues);
		TheGlove.GetFingerData (MoveFingers.Hand_Type.RIGHT_HAND,MinFingersRValues);
	}

	public void CaptureOpenPose()
	{
		TheGlove.GetFingerData (MoveFingers.Hand_Type.LEFT_HAND,MaxFingersLValues);
		TheGlove.GetFingerData (MoveFingers.Hand_Type.RIGHT_HAND,MaxFingersRValues);
	}

	// Use this for initialization
	void Start () {
		//base.Start ();
	}
	
	
	// Update is called once per frame
	void Update () {
		//base.Update ();
	}
}


