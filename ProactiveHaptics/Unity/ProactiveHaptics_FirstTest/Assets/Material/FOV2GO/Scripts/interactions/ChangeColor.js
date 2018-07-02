#pragma strict
var tapType : int = 3; // 1 = short tap 2 = long tap 3 = any tap
var oldColor; //: Color = Color.red;
var originalTexture;
private var motor : CharacterMotor;
@script RequireComponent( Rigidbody )
@script RequireComponent(s3dInteractor);


function Start(){

oldColor = gameObject.GetComponent.<Renderer>().material.GetColor("_Color");
//originalTexture = gameObject.renderer.material.GetTexture("_BumpMap");

}

function NewTap(params: TapParams) {
	//oldColor = gameObject.renderer.material.GetColor("_Color");
	 
	if (params.tap == tapType || tapType == 3) {
		//if(gameObject.renderer.material.texture == 'nossatextura')
		if(gameObject.GetComponent.<Renderer>().material.color == Color.red)
			//gameObject.renderer.material.SetTexture = 
			gameObject.GetComponent.<Renderer>().material.color = oldColor;
			else
			gameObject.GetComponent.<Renderer>().material.color = Color.red;
			
	}
}

function Update(){

	//motor.inputNewTap = Input.GetButton("Jump");



}