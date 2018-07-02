	#pragma strict
	var oldColor; 				//: Color = Color.red;
	//var originalTexture;
	//var success: AudioClip; 	// drag the sound 1 here
	//var error: AudioClip; 	// drag the sound 2 here
	var MyAudio : AudioClip;
	public var hit : RaycastHit;
	private var motor : CharacterMotor;
//	@script RequireComponent( Rigidbody )
	
	
	
	function Start () 
	{
		/*
		//AudioSource.PlayClipAtPoint(MyAudio, transform.position);
		
		while (true) 
		{
	 		if(Input.GetButton("helpbuttons")) //APERTA START NO GAMEPAD -> FALA TUTORIAL -> ATIVAR APENAS NA VERSAO COM GAMEPAD!!
	 		{
	        	AudioSource.PlayClipAtPoint(MyAudio, transform.position);
	        	yield WaitForSeconds(35.0);
	 
	        }
            yield;
        }
		*/
		
	}
	
	function Update () 
	{
		var object: GameObject;
		
		// The player pressed the selection button.
		if(Input.GetButtonDown("selectionbutton") || Input.GetButtonDown("selectionbutton2") || Input.GetButtonDown("selectionbutton3") || Input.touchCount == 3)
		{		
			if (Physics.Raycast (transform.position, Camera.main.transform.forward, hit))
			{
				object = hit.collider.gameObject;
						
				// If the object's material has the color propriety...
				if(object.GetComponent.<Renderer>().material.HasProperty("_Color"))
				{			
					if(object.GetComponent.<Renderer>().material.color != Color.red)
					{
						// Save the previous color...
						oldColor = object.GetComponent.<Renderer>().material.GetColor("_Color");
						// ...and change the current color, to identify that the object is currently selected.
						object.GetComponent.<Renderer>().material.color = Color.red;

						//if(object.tag == "Riscos")
							//audio.Play();		
						//else
							//audio.Play();
						
					}
					else if(object.GetComponent.<Renderer>().material.color == Color.red)
					{
						// If the object was already selected, unselects the object.
						object.GetComponent.<Renderer>().material.color = oldColor;
					}
				}
			}
		}
		
	}				