using UnityEngine;
using System.Collections;

public class InputDeviceManager : MonoBehaviour {

	// Prefabs that will be instantiated at the scene.
	public GameObject FPSControllerKinect;
	public GameObject KinectPrefab;
	public GameObject FPSController;
	public Transform FPSControllerPosition;
	private GameObject controller;

	private GyroCam gyrocam;

	public bool useKinect = false;
	public bool useHMD = false;
	public bool useGlove = false;
	public bool useJoystick = true;
	public bool useMouseKeyboard = true;
	
	// Use this for initialization
	void Awake () 
	{
		GameObject menuInfo = GameObject.FindGameObjectWithTag("menuInfo");
		MenuInfo info = null;
		if (menuInfo != null) {
			info = GameObject.FindGameObjectWithTag("menuInfo").GetComponent<MenuInfo>();
		}

		if (info != null) {
			this.useKinect = info.devices[DeviceType.KINECT];
			this.useHMD = info.devices[DeviceType.HMD];
			this.useGlove = info.devices[DeviceType.DATA_GLOVE];
			this.useJoystick = info.devices[DeviceType.JOYSTICK];
			this.useMouseKeyboard = info.devices[DeviceType.KEYBOARD];
		}

	}
	
	public void InstantiatePlayer()
	{
		// INSTANTIATE THE PLAYER
		if (useKinect) 
		{
			// If using kinect, instantiate the necessary prefabs.
			GameObject kinectPrefab = Instantiate(KinectPrefab) as GameObject;

			if(GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isGameMultiplayer == false)
			{
				controller = Instantiate(FPSControllerKinect) as GameObject;
				controller.transform.position = FPSControllerPosition.position;
				controller.transform.localRotation = FPSControllerPosition.localRotation;

				controller.GetComponentInChildren<KinectModelControllerV2>().sw = kinectPrefab.GetComponent<SkeletonWrapper>();
			}
			else // In a network game, the instantiate method is different
			{
				Debug.Log ("teste");
				controller = Network.Instantiate(FPSControllerKinect, FPSControllerPosition.position, FPSControllerPosition.localRotation, 0) as GameObject;

				if(controller.GetComponent<NetworkView>().isMine)
					controller.GetComponentInChildren<KinectModelControllerV2>().sw = kinectPrefab.GetComponent<SkeletonWrapper>();
			}
		}
		else
		{
			if(GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isGameMultiplayer == false)
			{
				controller = Instantiate (FPSController) as GameObject;
				controller.transform.position = FPSControllerPosition.position;
				controller.transform.localRotation = FPSControllerPosition.localRotation;
			}
			else // In a network game, the instantiate method is different
				controller = Network.Instantiate(FPSController, FPSControllerPosition.position, FPSControllerPosition.localRotation, 0) as GameObject;
		}


		if(GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isGameMultiplayer == true)
		{
			if(controller.GetComponent<NetworkView>().isMine) // This test is needed to only control MY character during network play! (to not control OTHER players's characters)
			{
				SetUsedDevices();
			}
		}
		else
			SetUsedDevices ();

	}


	void SetUsedDevices()
	{
		// SET THE CURRENTLY USED DEVICES IN THE CHARACTER CONTROLLER
		// If we're not going to use the HMD, disable the camera's component that initializes the HMD.
		if (!useHMD) 
		{
			gyrocam = controller.GetComponentInChildren<GyroCam>();
			if (gyrocam != null)
				gyrocam.enabled = false;
		} 
		
		
		if (useJoystick) {
			//Debug.Log ("Testando joystick");
			string axisNameX = "HorizontalJoy";
			string axisNameY = "VerticalJoy";
			for (int i=0; i<Input.GetJoystickNames().Length; i++) {
				if (Input.GetJoystickNames()[i].Equals("Controller (XBOX 360 For Windows)")) {
					Debug.Log ("Controle encontrado na porta " + (i+1).ToString());
					axisNameX += (i+1).ToString();
					axisNameY += (i+1).ToString();
					break;
				}
			}
			controller.GetComponent<FPSInputController>().axisNameX = axisNameX;
			controller.GetComponent<FPSInputController>().axisNameY = axisNameY;
		}
		
		if (useMouseKeyboard) {
			controller.GetComponent<FPSInputController>().axisNameX = "Mouse X";
			controller.GetComponent<FPSInputController>().axisNameY = "Forward";
			Screen.lockCursor = true;
		}
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
