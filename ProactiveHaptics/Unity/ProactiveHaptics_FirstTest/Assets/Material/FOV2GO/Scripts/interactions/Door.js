	#pragma strict
	var oldColor; //: Color = Color.red;
	//var originalTexture;
	var toggle : int;
	//var success: AudioClip; // drag the sound 1 here
	//var error: AudioClip; // drag the sound 2 here
	public var hit2 : RaycastHit;
	private var motor : CharacterMotor;
//	@script RequireComponent( Rigidbody )
	
	
var smooth = 0.0000005;
var DoorOpenAngle = 90.0;
private var open : int;
private var enter : boolean;
private var defaultRot : Vector3;
private var openRot : Vector3;
	
	
	function Start () 
	{
		toggle =0;
		defaultRot = transform.eulerAngles;
		openRot = new Vector3 (defaultRot.x, defaultRot.y - DoorOpenAngle, defaultRot.z);
		open = 0;
	}
	
	function Update () {
	
			
			var object: GameObject;
					
						
				
				if (Physics.Raycast (transform.position, Camera.main.transform.forward, hit2))
				{
					
											
					if(Input.GetButton("selectionbutton") || Input.touchCount > 0)
					{
					object = hit2.collider.gameObject;
					
						if(toggle == 0)
						{
							toggle = 1;
											
							if(open == 0){
							
							
							transform.eulerAngles = Vector3.Slerp(openRot, transform.eulerAngles,   smooth);
							print("era pra abrir");
							//print(transform.eulerAngles.x);
							open = 1;
							
						}else{
						
							transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaultRot,  smooth);
							print ("era pra fechar");
							open=0;
						}
						}
					}else
					{
						toggle = 0;
					}
			}
	}
		