using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveFingers : MonoBehaviour {
	public enum Hand_Type {
		LEFT_HAND, RIGHT_HAND
	}

	public enum Finger_Type {
		THUMB_1,
		THUMB_2,
		THUMB_3,
		INDEX_1,
		INDEX_2,
		INDEX_3,
		MIDDLE_1,
		MIDDLE_2,
		MIDDLE_3,
		RING_1,
		RING_2,
		RING_3,
		PINKY_1,
		PINKY_2,
		PINKY_3
	}
	
	private bool bCloseRightHand;
	private bool bCloseLeftHand;
	
public class Finger {

		public Finger_Type type;
		public Vector3 rotationAxis;
		public GameObject boneObj;
		public Quaternion startBoneRotation;
		public float minRotation;
		public float maxRotation;



		public Finger(Finger_Type ft, GameObject bone, float minR, float maxR, Vector3 axis) {
			this.type = ft;
			this.boneObj = bone;
			this.minRotation = minR;
			this.maxRotation = maxR;
			this.rotationAxis = axis;
			this.startBoneRotation = bone.transform.localRotation;
		}

		public void SetFingerRotation(float value, bool inverted) {
			float valueClampped = 0;

			if (inverted) {
				valueClampped = Mathf.Clamp(-value, -maxRotation, -minRotation);
			} else {
				valueClampped = Mathf.Clamp(value, minRotation , maxRotation);
			}

			boneObj.transform.localRotation = Quaternion.AngleAxis(valueClampped, rotationAxis) * startBoneRotation;
		}
	};

	public Hand_Type hand;
	private bool invertRotation;

	// Left hand bones
	public Transform ThumbDummy;
	public GameObject Thumb1;
	public GameObject Thumb2;
	public GameObject Thumb3;
	
	public Transform IndexDummy;
	public GameObject Index1;
	public GameObject Index2;
	public GameObject Index3;
	
	public Transform MiddleDummy;
	public GameObject Middle1;
	public GameObject Middle2;
	public GameObject Middle3;
	
	public Transform RingDummy;
	public GameObject Ring1;
	public GameObject Ring2;
	public GameObject Ring3;
	
	public Transform PinkyDummy;
	public GameObject Pinky1;
	public GameObject Pinky2;
	public GameObject Pinky3;
	


	// List Containing all fingers
	private Dictionary<Finger_Type, Finger> fingers;

	private Glove motionGlove;
	
	// Use this for initialization
	void Start () {
		motionGlove = GetComponent<Glove>();

		Vector3 thumbAxis = ThumbDummy? ThumbDummy.up : -Vector3.up;
		Vector3 indexAxis = IndexDummy? IndexDummy.forward : Vector3.forward;
		Vector3 middleAxis = MiddleDummy? MiddleDummy.forward : Vector3.forward;
		Vector3 ringAxis = RingDummy? RingDummy.forward : Vector3.forward;
		Vector3 pinkyAxis = PinkyDummy? PinkyDummy.forward : Vector3.forward;

		fingers = new Dictionary<Finger_Type, Finger>();

		// LEFT HAND FINGERS
		fingers.Add(Finger_Type.THUMB_1, new 
		            Finger(Finger_Type.THUMB_1, Thumb1, 0, 40, thumbAxis)); 
		
		fingers.Add(Finger_Type.THUMB_2, new 
		            Finger(Finger_Type.THUMB_2, Thumb2, 0, 90, thumbAxis));
		
		fingers.Add(Finger_Type.THUMB_3, new 
		            Finger(Finger_Type.THUMB_3, Thumb3, 0, 90, thumbAxis));

		fingers.Add(Finger_Type.INDEX_1, new 
		            Finger(Finger_Type.INDEX_1, Index1, 0, 100, indexAxis)); 

		fingers.Add(Finger_Type.INDEX_2, new 
		            Finger(Finger_Type.INDEX_2, Index2, 0, 120, indexAxis));

		fingers.Add(Finger_Type.INDEX_3, new 
		            Finger(Finger_Type.INDEX_3, Index3, 0, 90, indexAxis));

		fingers.Add(Finger_Type.MIDDLE_1, 
		            new Finger(Finger_Type.MIDDLE_1, Middle1, 0, 100, middleAxis));

		fingers.Add(Finger_Type.MIDDLE_2, 
		            new Finger(Finger_Type.MIDDLE_2, Middle2, 0, 120, middleAxis));

		fingers.Add(Finger_Type.MIDDLE_3, new 
		            Finger(Finger_Type.MIDDLE_3, Middle3, 0, 90, middleAxis));

		fingers.Add(Finger_Type.RING_1, 
		            new Finger(Finger_Type.RING_1, Ring1, 0, 100, ringAxis));
		
		fingers.Add(Finger_Type.RING_2, 
		            new Finger(Finger_Type.RING_2, Ring2, 0, 120, ringAxis));
		
		fingers.Add(Finger_Type.RING_3, new 
		            Finger(Finger_Type.RING_3, Ring3, 0, 90, ringAxis));

		fingers.Add(Finger_Type.PINKY_1, 
		            new Finger(Finger_Type.PINKY_1, Pinky1, 0, 100, pinkyAxis));
		
		fingers.Add(Finger_Type.PINKY_2, 
		            new Finger(Finger_Type.PINKY_2, Pinky2, 0, 120, pinkyAxis));
		
		fingers.Add(Finger_Type.PINKY_3, new 
		            Finger(Finger_Type.PINKY_3, Pinky3, 0, 90, pinkyAxis));

	}

	void Update()
	{
		if(GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isGameMultiplayer == true)
		{
			if(GetComponent<NetworkView>().isMine) // This test is needed to only control MY character during network play! (to not control OTHER players's characters)
			{
				updateFingers();
			}
		}
		else
			updateFingers();
	}
	
	void updateFingers()
	{
		foreach (KeyValuePair<Finger_Type, Finger> f in fingers) {
			/*f.Value.SetFingerRotation(motionGlove.GetFingerValue(hand, f.Key) * f.Value.maxRotation, 
		                          (hand == Hand_Type.RIGHT_HAND)? true : false);*/
			f.Value.SetFingerRotation(motionGlove.GetFingerValue(hand, f.Key) * f.Value.maxRotation, true);
		}
		
		int gR = motionGlove.GetGesture (Hand_Type.RIGHT_HAND);
		int gL = motionGlove.GetGesture (Hand_Type.LEFT_HAND);
		
		if(gR==0)
		{
			bCloseRightHand = true;
			//Debug.Log ("<####> CLOSE_RIGHT_HAND, "+bCloseRightHand+","+Time.fixedTime);	
		}
		else
		{
			bCloseRightHand = false;
		}
		
		if(gL==0)
		{
			bCloseLeftHand = true;
			//Debug.Log ("<####> CLOSE_LEFT_HAND, "+bCloseLeftHand+","+Time.fixedTime);	
		}
		else
		{
			bCloseLeftHand = false;
		}

	}

	public bool IsLocal()
	{
		if(GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isGameMultiplayer == true)
		{
			if(GetComponent<NetworkView>().isMine) // This test is needed to only control MY character during network play! (to not control OTHER players's characters)
			{
				return true;
			}
			return false;
		}
		else
			return true;
	}

	public bool isCloseRightHand()
	{
		return bCloseRightHand;
	}

	public bool isCloseLeftHand()
	{
		return bCloseLeftHand;
	}
	
}
