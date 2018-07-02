#pragma strict


var toggle : int;
public var hit : RaycastHit;
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
}   open = 0;

function Update () {

		
		var object: GameObject;
				
					
			
			if (Physics.Raycast (transform.position, Camera.main.transform.forward, hit))
			{
				
										
				if(Input.GetButton("selectionbutton") || Input.touchCount > 0)
				{
				object = hit.collider.gameObject;
				
					if(toggle == 0 && open == 0)
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
		
		