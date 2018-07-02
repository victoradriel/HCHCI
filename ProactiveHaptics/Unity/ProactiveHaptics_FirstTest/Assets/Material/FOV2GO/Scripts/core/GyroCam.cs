using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;



public class GyroCam : MonoBehaviour
	
{
	[DllImport ("SensicsHMD")]
	private static extern void closeSensicsHMD ();
	[DllImport ("SensicsHMD")]
	private static extern bool openSensicsHMD ();
	[DllImport ("SensicsHMD")]
	private static extern bool getDataSensicsHMD (float[] data);
	
	private bool isHMDopen = false;
	private bool calibrated = false;
	Quaternion inverseInitialRot = new Quaternion();
	//Quaternion CameraInitialRot = new Quaternion();
		
	InputDeviceManager idm;
	// public float adjAngle = -75;
	//Quaternion adj, adjY180, adjX90, adjY90; // Helper quaternions, for HMD rotation fix.
	
	bool isUsingPlatform = false;
	
	public Quaternion getSensicsQuat() {
		// Gets quaternion from HMD.
		float[] data = new float[4];
		getDataSensicsHMD (data);
		
		return new Quaternion(data[2], -data[3], data[1], data[0]);
	}
	
	public void Start () {
		//CameraInitialRot = transform.parent.localRotation;
		//transform.localRotation = Quaternion.identity;

		//MenuInfo info = GameObject.FindGameObjectWithTag ("menuInfo").GetComponent<MenuInfo> ();
		//info.devices.TryGetValue (DeviceType.PLATAFORMA_WIP, out isUsingPlatform);
		idm = GameObject.Find ("InputDeviceManager").GetComponent<InputDeviceManager>();
		
		inverseInitialRot = Quaternion.identity;
		isHMDopen = openSensicsHMD (); // Detects the HMD, if it's connected.
		if(isHMDopen)
		{
			Invoke ("Calibrate", 0.25f);
		}

	}
	
	public void Update () 
	{
		if (Input.GetKeyUp ("r")) {
			Calibrate();
		}
		
		if (this.gameObject.activeSelf && isHMDopen) 
		{
			// Get the current quaternion from the HMD.
			Quaternion qt = getSensicsQuat();

			transform.localRotation = (inverseInitialRot * qt); 
			calibrated = false;
		}
	}
	
	public void Calibrate () {
		inverseInitialRot = Quaternion.Inverse(getSensicsQuat ());

		calibrated = true;
	}
	
}