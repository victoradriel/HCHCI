#pragma strict
public var hit : RaycastHit;

function Start () {

}

function Update () {
		
		//var cam : Transform = Camera.main.transform.position;
		//var forward : Vector3 = transform.TransformDirection(Vector3.forward) * 10;
		//Debug.DrawRay (forward, forward, Color.green);
		
		//var forward : Vector3 = transform.TransformDirection(Vector3.forward) * 10;
		Debug.DrawRay (Camera.main.transform.position, Camera.main.transform.forward * 10000, Color.red);

}