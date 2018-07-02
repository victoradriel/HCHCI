using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GestureManager : MonoBehaviour {

	private float 				Threshold;
	private int					LastGestureID=-1; 	
	private GestureListener 	Listener = null;
	private List<Gesture> 		GestureList = new List<Gesture>();
	private List<int> 			GestureIDList = new List<int>();

	public void addGesture(Gesture ngesture, int ID){
		GestureList.Add (ngesture);
		GestureIDList.Add (ID);
	}

	public void Clear(){
		GestureList.Clear ();
		GestureIDList.Clear ();
	}

	public void SetThreshold(float pThreshold)	{
		Threshold = pThreshold;
	}

	public void SetGestureListener(GestureListener pListener)	{
		Listener = pListener;
	}

	private int FindGesture(Gesture ngesture){
		int i;
		for(i=0;i<GestureList.Count;i++)
		{
			if(GestureList[i].IsEqual(ngesture,Threshold))
				return GestureIDList[i];

		}
		return -1;
	}

	public abstract Gesture CaptureGesture();

	// Use this for initialization
	void Start () {
		LastGestureID = -1;
	}


	// Update is called once per frame
	void Update () {
		Gesture currentGesture;
		int gestureID;
		currentGesture = CaptureGesture ();

		gestureID = FindGesture (currentGesture);
		if (gestureID >= 0) { 
			if (Listener != null) {
				if(gestureID != LastGestureID)
				{
					if(LastGestureID>=0)
						if(Listener!=null)
							Listener.OnGestureRelease();

					Listener.OnGestureRecognition (gestureID);
					LastGestureID = gestureID;
				}
			}
		} else {
			if(LastGestureID>=0)
			{
				if(Listener!=null)
					Listener.OnGestureRelease();
				LastGestureID = -1;
			}
		}
	}
}
