using UnityEngine;
using System;
using System.Collections;
using System.IO;
using FDTGloveUltraCSharpWrapper;

public class Glove : MonoBehaviour 
{
	
	CfdGlove leftHandGuanteObj;
	CfdGlove rightHandGuanteObj;

	// Use this for initialization
	void Start () {

		int usbId = 0;
		int gloveCount = 0;

		for(usbId=0;usbId<16;usbId++)
		{
			String handPort1 = "USB"+usbId;

			CfdGlove tmpGlove = new CfdGlove();

			try{
				if(tmpGlove.Open (handPort1))
				{
					setCorrectGloveHand(tmpGlove);
					gloveCount++;
				}
			} catch { Debug.LogError ("Data Glove USB1 hand is not opened."); }

			if(gloveCount>1)
				break;
		}

		string strPath = Application.dataPath + Path.DirectorySeparatorChar;
						
		if (leftHandGuanteObj!=null)
		{
			if(leftHandGuanteObj.LoadCalibration(strPath+"Left.fd"))
				Debug.Log("Data Glove LEFT calibration file Loaded.");
			else
				Debug.LogWarning("Data Glove LEFT calibration file not found.");
		}
		else
			Debug.LogError ("Data Glove LEFT not found.");

		if (rightHandGuanteObj!=null)
		{
			if(rightHandGuanteObj.LoadCalibration(strPath+"Right.fd"))
				Debug.Log("Data Glove RIGHT calibration file Loaded.");
			else
				Debug.LogWarning ("Data Glove RIGHT calibration file not found.");
		}
		else 
			Debug.LogError ("Data Glove RIGHT not found.");
	}

	private void setCorrectGloveHand(CfdGlove tmpGlove)
	{
		int thandId = tmpGlove.GetGloveHand ();
		MoveFingers.Hand_Type handId = (MoveFingers.Hand_Type)thandId;
		if(handId ==  MoveFingers.Hand_Type.LEFT_HAND)
			leftHandGuanteObj = tmpGlove;
		else if(handId ==  MoveFingers.Hand_Type.RIGHT_HAND)
			rightHandGuanteObj = tmpGlove;
	}

	public float GetFingerValue(MoveFingers.Hand_Type hand, MoveFingers.Finger_Type finger)
	{
		if (hand == MoveFingers.Hand_Type.LEFT_HAND) {
			if (leftHandGuanteObj!=null)
			{
				if(leftHandGuanteObj.IsOpen())
				{
					return leftHandGuanteObj.GetSensorScaled((int)finger);
				}
			}
		}
		
		if (hand == MoveFingers.Hand_Type.RIGHT_HAND) {
			if (rightHandGuanteObj!=null)
			{
			    if(rightHandGuanteObj.IsOpen())
				{
					return rightHandGuanteObj.GetSensorScaled((int)finger);
				}
			}
		}

		return 1f;
	}

	public void GetFingerData(MoveFingers.Hand_Type hand, float[] finger)
	{
		if (hand == MoveFingers.Hand_Type.LEFT_HAND) {
			if(leftHandGuanteObj!=null)
			{
				if (leftHandGuanteObj.IsOpen()) {
					leftHandGuanteObj.GetSensorScaledAll(ref finger);
				}
			}
		}
		
		if (hand == MoveFingers.Hand_Type.RIGHT_HAND) {
			if(rightHandGuanteObj!=null)
			{
				if (rightHandGuanteObj.IsOpen()) {
					rightHandGuanteObj.GetSensorScaledAll(ref finger);
				}
			}
		}
		
		return;
	}

	public int GetGesture(MoveFingers.Hand_Type hand)
	{
		if (hand == MoveFingers.Hand_Type.LEFT_HAND) {
			if(leftHandGuanteObj!=null)
			{
				if (leftHandGuanteObj.IsOpen()) {
					return leftHandGuanteObj.GetGesture();
				}
			}
		}
		
		if (hand == MoveFingers.Hand_Type.RIGHT_HAND) {
			if (rightHandGuanteObj!=null) 
			{
				if (rightHandGuanteObj.IsOpen()) {
					return rightHandGuanteObj.GetGesture();
				}
			}
		}
		
		return -1;
	}
}
