using UnityEngine;
using System.Collections;
using System.Timers;

public class puzzleRules : MonoBehaviour {
	
	private float minSound = 0.1f;
	private float maxSound = 0.8f;
	
	private GameObject 		PlayerController 	= null;
	private GameObject 		PlayerCamera		= null;
	private AudioSource 	ambientalAudio 		= null;
	private GameObject 		currentErrorPiece	= null;

	private KinectModelControllerV2 KinectControler = null;

	private bool[,] 		AssembledFlag	 	= null;
	private GameObject[,] 	TempleatePieces	 	= null;
	private GameObject[] 	AssembledPieces 	= null;
	private Vector3[] 		T0AssembledPieces   = null;
	private Quaternion[] 	R0AssembledPieces	= null;
	
	private bool soundChanged = false;
	private bool startGame = false;
	private bool endGame = false;
	
	private int asembledPiecesCount = 0;
	private int TemplatesByUser 	= 0;
	private int InteractPieces 		= 0;
	private int currentAssemblyID 	= 1;
	private int otherAssemblyID 	= 1;
	private int currentUserID 		= 0;
	private int otherUserID 		= 0;
	
	private float lastFlutterTime		= 0.0f;
	private float lastIncreseSounfTime  = 0.0f;
	private float lastErrorTime			= 0.0f;
	
	private float 	TimeGame		= 0.0f;
	private float 	TimeGame0		= 0.0f;
	private int 	GoodAssembled	= 0;
	private int 	BadAssembled	= 0;
	//private int 	GoodSwapping	= 0;
	//private int 	GoodGrabed		= 0;
	//private int 	BadSwapping		= 0;
	//private int 	BadGrabed		= 0;			
	
	
	private GUIText TimeText;
	private GUIText GoodAText;
	private GUIText BadAText;

	//private GUIText GoodSText;
	//private GUIText GoodGText;
	//private GUIText BadSText;
	//private GUIText BadGText;			
	
	public enum Score
	{
		ScGoodAssembled,
		ScGoodSwapping,
		ScGoodGrabed,
		ScBadAssembled,
		ScBadSwapping,
		ScBadGrabed,
	};
	// Use this for initialization
	void Start () {
		
		// Load puzzle pieces
		GameObject[] 	query;
		GameObject 		obj;
		int 			i;
		int 			iobj;
		int 			iuser;
		
		query = GameObject.FindGameObjectsWithTag("Template"); 
		
		TemplatesByUser = query.Length/2;
		TempleatePieces  	= new GameObject[2,TemplatesByUser];
		AssembledFlag 		= new bool[2, TemplatesByUser];
		
		iuser = 0;
		iobj = 0;
		for(i=0;i<query.Length;i++)
		{
			obj = query[i];
			GetPieceId(obj,out iuser,out iobj);
			
			SetVisibility(obj,true);
			TempleatePieces[iuser-1,iobj-1] = obj;
			AssembledFlag[iuser-1,iobj-1] = false;
			loadTest.Log ("<####> ADD_TEMPLATE,"+obj.name+","+Time.fixedTime);					
		}
		
		query = GameObject.FindGameObjectsWithTag("CanAssembled"); 
		InteractPieces = query.Length;
		
		AssembledPieces 	= new GameObject[InteractPieces];
		T0AssembledPieces 	= new Vector3[InteractPieces];
		R0AssembledPieces 	= new Quaternion[InteractPieces];
		
		for (i=0; i<InteractPieces; i++)
		{
			obj = query [i];
			obj.transform.parent = null;
			AssembledPieces[i] =  obj;
			
			loadTest.Log ("<####> ADD_INTERACTOBJECT,"+obj.name+","+Time.fixedTime);	
		}
		
		// Sort form left to right
		System.Array.Sort (AssembledPieces, delegate (GameObject o1, GameObject o2){
			float z1 = -(o1.transform.position.z);
			float z2 = -(o2.transform.position.z);
			
			z1 = z1*100;
			z2 = z2*100;
			return (int)(z1-z2);
		});
		
		for (i=0; i<InteractPieces; i++)
		{
			obj = AssembledPieces [i];
			T0AssembledPieces[i] = AssembledPieces[i].transform.position;
			R0AssembledPieces[i] = AssembledPieces[i].transform.rotation;
			
			loadTest.Log ("<####> INTERACTOBJECT_T0,"+obj.name+","+T0AssembledPieces[i].x+","+T0AssembledPieces[i].y+","+T0AssembledPieces[i].z+","+Time.fixedTime);	
			loadTest.Log ("<####> INTERACTOBJECT_R0,"+obj.name+","+R0AssembledPieces[i].x+","+R0AssembledPieces[i].y+","+R0AssembledPieces[i].z+","+R0AssembledPieces[i].w+","+Time.fixedTime);	
		}
		
		// END - Load puzzle pieces
		
		
		// Move HUD
		if(GetComponent<NetworkView>().isMine)
		{
			Screen.SetResolution(1280,512,true);

			Rect rect;
			Rect rect0;
			Rect Drect;
			GUITexture TexBG = (GUITexture)(GameObject.Find("TextureBG")).GetComponent<GUITexture>(); 
			
			rect = new Rect(TexBG.pixelInset);
			rect0 = rect;
			rect.x = ((Screen.width/4)	-(rect.width+10));
			rect.y = ((Screen.height/2)- (rect.height+130));
			
			TexBG.pixelInset = rect;
			Vector3 dPos = TexBG.transform.position;
			
			dPos.x = rect.x - rect0.x;
			dPos.y = rect.y - rect0.y;
			
			TimeText 	= (GUIText)(GameObject.Find("TimeText")).GetComponent<GUIText>(); 
			GoodAText 	= (GUIText)(GameObject.Find("GoodAText")).GetComponent<GUIText>(); 
			BadAText 	= (GUIText)(GameObject.Find("BadAText")).GetComponent<GUIText>(); 

			//GoodGText	= (GUIText)(GameObject.Find("GoodGText")).guiText; 
			//GoodSText	= (GUIText)(GameObject.Find("GoodSText")).guiText; 
			//BadGText	= (GUIText)(GameObject.Find("BadGText")).guiText; 
			//BadSText	= (GUIText)(GameObject.Find("BadSText")).guiText; 
			
			TimeText.pixelOffset 	= TimeText.pixelOffset 	+ new Vector2(dPos.x,dPos.y);
			GoodAText.pixelOffset 	= GoodAText.pixelOffset	+ new Vector2(dPos.x,dPos.y);
			BadAText.pixelOffset 	= BadAText.pixelOffset 	+ new Vector2(dPos.x,dPos.y);

			//GoodGText.pixelOffset	= GoodGText.pixelOffset	+ new Vector2(dPos.x,dPos.y);
			//GoodSText.pixelOffset 	= GoodSText.pixelOffset	+ new Vector2(dPos.x,dPos.y);
			//BadGText.pixelOffset 	= BadGText.pixelOffset 	+ new Vector2(dPos.x,dPos.y);
			//BadSText.pixelOffset 	= BadSText.pixelOffset 	+ new Vector2(dPos.x,dPos.y);
		}
		// END - Move HUD

		// Time Log
		InvokeRepeating ("TimerLog",5.0f, 0.25f);
	}

	void TimerLog()
	{
		if (PlayerCamera != null)
		{
			if(startGame)
			{
				loadTest.Log ("<####> CameraPosition,"+currentUserID+","+
				              PlayerCamera.transform.position.x+","+
				              PlayerCamera.transform.position.y+","+
				              PlayerCamera.transform.position.z+","+
				              Time.fixedTime);

				loadTest.Log ("<####> CameraRotation,"+currentUserID+","+
				              PlayerCamera.transform.rotation.x+","+
				              PlayerCamera.transform.rotation.y+","+
				              PlayerCamera.transform.rotation.z+","+
				              PlayerCamera.transform.rotation.w+","+
				              Time.fixedTime);
			}
		}
	}

	// Update is called once per frame
	void Update () {

		if(startGame)
		{
			TimeText.text = "" + ((int)TimeGame) + " s";
			TimeGame = Time.fixedTime - TimeGame0;
		}
		
		// Key events
		if (Input.GetKeyDown(KeyCode.N))
		{
			CallNextPuzzle();
		}
		
		if (Input.GetKeyDown(KeyCode.S))
		{
			GetComponent<NetworkView>().RPC ("StartPuzzle", RPCMode.All);
		}
		// End  - Key events
		
		// Game user configuration
		if(currentUserID == 0)
		{
			if(PlayerController ==  null)
			{
				PlayerController = GameObject.Find ("FirstPersonControllerKinect(Clone)");
				PlayerCamera = GameObject.FindWithTag("MainCamera");
				KinectControler = PlayerController.GetComponent<KinectModelControllerV2>();
		

				if(PlayerController)
				{
					SkinnedMeshRenderer sMR = PlayerController.GetComponentInChildren<SkinnedMeshRenderer>();
					sMR.updateWhenOffscreen = true;
				}
			}
			
			if(Network.peerType != NetworkPeerType.Disconnected &&
			   Network.peerType != NetworkPeerType.Connecting)
			{
				if (Network.isServer)
				{
					if(loadTest.GetCurrentServerUserLeftSide())
					{
						currentUserID = 1;
						otherUserID = 2;
					}
					else
					{							
						currentUserID = 2;
						otherUserID = 1;
					}
					loadTest.Log ("<####> Is Server"+","+Time.fixedTime);	
				}
				else
				{
					if(loadTest.GetCurrentServerUserLeftSide())
					{
						currentUserID = 2;
						otherUserID = 1;
					}
					else
					{							
						currentUserID = 1;
						otherUserID = 2;
					}
					
					loadTest.Log ("<####> Is Client"+","+Time.fixedTime);	
				}
				
				loadTest.Log ("<####> Current_User,"+currentUserID+","+Time.fixedTime);
				
				Vector3 usrPosition;
				
				if (PlayerController != null)
				{
					
					usrPosition = PlayerController.transform.position;
					
					usrPosition.y =  4.5f; //3.9f;
					usrPosition.x = -16.20f;
					
					if (currentUserID == 1)
					{
						usrPosition.z = -7.105118f;
					}
					else if (currentUserID == 2)
					{
						usrPosition.z = -8.892f;
					}
					
					lastFlutterTime = Time.fixedTime;
					
					Move(PlayerController,usrPosition);
					usrPosition = PlayerController.transform.position;
					loadTest.Log ("<####> USR_POSITION,"+currentUserID+","+usrPosition.x+","+usrPosition.y+","+usrPosition.z+","+Time.fixedTime);
				}
			}			
		}
		// END -  Game user configuration
		
		
		// Start sound
		if(startGame)
		{
			if(ambientalAudio == null)
			{
				GameObject key = GameObject.Find("KeyPlantVase");
				if(key != null)
				{
					ambientalAudio = key.GetComponent<AudioSource>();
					
					if(loadTest.GetCurrentIncreaseSound())
						ambientalAudio.volume = minSound;
					else
						ambientalAudio.volume = maxSound;
					
					ambientalAudio.loop = true;
					ambientalAudio.mute = false;
					loadTest.Log ("<####> INITIAL_SOUND_LEVEL,"+currentUserID+"," + ambientalAudio.volume + "," + Time.fixedTime);
					//ambientalAudio.Play ();
				}
			}
		}
		// END Star sound
		
		// Change Sound
		if (asembledPiecesCount > (TemplatesByUser-1) && !soundChanged)
		{
			if (GetComponent<NetworkView>().isMine)
			{
				if( lastIncreseSounfTime == 0)
					lastIncreseSounfTime = Time.fixedTime;
				
				if ((Time.fixedTime - lastIncreseSounfTime) > 0.5)
				{
					float endSoundLevel;
					if(loadTest.GetCurrentIncreaseSound())
					{
						endSoundLevel = maxSound;

						if(ambientalAudio.volume < endSoundLevel)
						{
							ambientalAudio.volume = (ambientalAudio.volume + 0.1f);

							loadTest.Log ("<####> CHANGE_SOUND_LEVEL,"+currentUserID+"," + ambientalAudio.volume + "," + Time.fixedTime);
							lastIncreseSounfTime = Time.fixedTime;
						}
						else
							soundChanged = true;
					}
					else
					{
						endSoundLevel = minSound;

						if(ambientalAudio.volume > endSoundLevel)
						{
							ambientalAudio.volume = (ambientalAudio.volume - 0.1f);
							
							loadTest.Log ("<####> CHANGE_SOUND_LEVEL,"+currentUserID+"," + ambientalAudio.volume + "," + Time.fixedTime);
							lastIncreseSounfTime = Time.fixedTime;
						}
						else
							soundChanged = true;
					}
				}
			}
		}
		// END - Change Sound
		
		// Puzzle cue 
		if(TempleatePieces != null && startGame)
		{
			if ((Time.fixedTime - lastFlutterTime) > 0.5)
			{
				if(currentUserID == 0)
					return;
				
				if(GetComponent<NetworkView>().isMine)
				{
					if(TempleatePieces [currentUserID-1,currentAssemblyID-1].tag == "Template")
					{
						ToggleVisibility (TempleatePieces [currentUserID-1,currentAssemblyID-1]);
					}
				}
				
				lastFlutterTime = Time.fixedTime;
			}
		}
		// END Puzzle cue 
		
		
		// Error piece 
		if ((Time.fixedTime - lastErrorTime) > 2) {
			if(currentErrorPiece != null)
			{
				if(GetComponent<NetworkView>().isMine)
				{
						loadTest.Log ("<####> BACK_PIECE," + currentUserID + "," + currentAssemblyID + "," + currentErrorPiece.name + "," + Time.fixedTime);
					GetComponent<NetworkView>().RPC ("BackPieceToOriginalPlace", RPCMode.All,currentErrorPiece.name);
				}
				currentErrorPiece = null;
			}
			
			lastErrorTime = Time.fixedTime;
		}// END Error piece 
		
		
		// Check if Other user assembed piece
		if(otherUserID>0)
		{
			if(TempleatePieces!=null && AssembledFlag!=null)
			{
				if(otherUserID>0)
				{
					if(TempleatePieces [otherUserID-1,otherAssemblyID-1].tag == "Assembled")
					{
						if(!AssembledFlag[otherUserID-1, otherAssemblyID-1])
						{
							AssembledFlag [otherUserID-1, otherAssemblyID-1] = true;
							loadTest.Log ("<####> OTHER_ASSEMBLE," + currentUserID + "," + otherAssemblyID + "," + Time.fixedTime);
							
							asembledPiecesCount++;
							otherAssemblyID++;
							
							if(otherAssemblyID>TemplatesByUser)
								otherAssemblyID=(TemplatesByUser-1);
						}
					}
				}
			}
		}

		//Check score
		if (GetComponent<NetworkView>().isMine)
		{
			//GoodAssembled = asembledPiecesCount;
			//GoodGrabed  = asembledPiecesCount;

			GoodAText.text = ""+GoodAssembled;
			BadAText.text = ""+BadAssembled;
			//GoodGText.text = ""+GoodGrabed;
		}

		// END - Check if Other user assembed piece
				
		// Check end Game
		if(AssembledFlag != null)
		{				
			if(	AssembledFlag [0, TemplatesByUser-1] && AssembledFlag [1, TemplatesByUser-1])
			{
				if(!endGame)
				{
					loadTest.Log ("<####> END_ASSEMBLE,"+ currentUserID + "," + currentErrorPiece.name + "," + Time.fixedTime);
					Invoke("CallNextPuzzle",2.0f);
					endGame = true;
				}
			}
		}
	}
	
	private void CallNextPuzzle()
	{
		GetComponent<NetworkView>().RPC ("NextPuzzle", RPCMode.All);
	}
	
	[RPC]
	private void NextPuzzle()
	{
		
		if(GetComponent<NetworkView>().isMine)
		{
			//Print game score
			loadTest.Log ("<####> LastScore,Time,"+ currentUserID + "," +TimeGame+"," +Time.fixedTime);
			loadTest.Log ("<####> LastScore,GoodAssembled,"+ currentUserID + "," +GoodAssembled+"," +Time.fixedTime);
			//loadTest.Log ("<####> LastScore,GoodSwapping," +GoodSwapping+"," +Time.fixedTime);
			//loadTest.Log ("<####> LastScore,GoodGrabed," +GoodGrabed+"," +Time.fixedTime);
			loadTest.Log ("<####> LastScore,BadAssembled," +currentUserID + ","+ BadAssembled+"," +Time.fixedTime);
			//loadTest.Log ("<####> LastScore,BadSwapping," +BadSwapping+"," +Time.fixedTime);
			//loadTest.Log ("<####> LastScore,BadGrabed," +BadGrabed+"," +Time.fixedTime);
			
			
			Camera[] allCamChilds = PlayerController.GetComponentsInChildren<Camera>();
			foreach(Camera child in allCamChilds)
			{
				child.enabled = false;
				child.gameObject.SetActive(false);
			}
			
			//GL.Clear(true,true,new Color(0.1f,0.1f,0.1f));
			loadTest.NextTest();
			Application.LoadLevel("preload");
		}
	}
	
	[RPC]
	private void StartPuzzle()
	{
		if(GetComponent<NetworkView>().isMine)
		{
			lastFlutterTime = Time.fixedTime;
			startGame = true;
			loadTest.Log ("<####> START_GAMES," + currentUserID + "," + Time.fixedTime);
			TimeGame0 = Time.fixedTime;
			TimeGame = 0.0f;	
		}
	}
	
	public void SendUpdateScore(Score pScore, int ds)
	{
		GetComponent<NetworkView>().RPC ("UpdateScore", RPCMode.All,(int)pScore,ds);
	}
	
	/*[RPC]
	private void UpdateScore(int pScore, int ds)
	{
		switch((Score)pScore)
		{
		case Score.ScGoodAssembled:
			GoodAssembled+=ds;
			GoodAText.text = ""+GoodAssembled;
			loadTest.Log ("<####> UpdateScore,GoodAssembled," +GoodAssembled+"," +Time.fixedTime);
			break;
		case Score.ScGoodSwapping:
			GoodSwapping+=ds;
			GoodSText.text = ""+GoodSwapping;
			loadTest.Log ("<####> UpdateScore,GoodSwapping," +GoodSwapping+"," +Time.fixedTime);
			break;
		case Score.ScGoodGrabed:
			GoodGrabed+=ds;
			GoodGText.text = ""+GoodGrabed;
			loadTest.Log ("<####> UpdateScore,GoodGrabed," +GoodGrabed+"," +Time.fixedTime);
			break;
		case Score.ScBadAssembled:
			BadAssembled+=ds;
			BadAText.text = ""+BadAssembled;
			loadTest.Log ("<####> UpdateScore,BadAssembled," +BadAssembled+"," +Time.fixedTime);
			break;
		case Score.ScBadSwapping:
			BadSwapping+=ds;
			BadSText.text = ""+BadSwapping;
			loadTest.Log ("<####> UpdateScore,BadSwapping," +BadSwapping+"," +Time.fixedTime);
			break;
		case Score.ScBadGrabed:
			BadGrabed+=ds;
			BadAText.text = ""+BadGrabed;
			loadTest.Log ("<####> UpdateScore,BadGrabed," +BadGrabed+"," +Time.fixedTime);
			break;
		}
	}*/
	
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
	
	public int  GetUserID()
	{
		return currentUserID;
	}
	
	
	public GameObject[] GetAssembledPieces()
	{
		return AssembledPieces;
	}
	
	void GetPieceId(GameObject piece,out int  userId,out int pieceId)
	{
		int istr;
		int iobj;
		int iuser;
		
		string objName = piece.name;
		string token;
		
		istr = objName.IndexOf("U");
		
		if (istr < 0){
			userId  = -1;
			pieceId = -1;
			return;
		}
		token = objName.Substring(istr+1,1);
		System.Int32.TryParse(token,out userId);
		
		istr = objName.IndexOf("P");
		
		if (istr < 0){
			userId  = -1;
			pieceId = -1;
			return;
		}
		
		token = objName.Substring(istr+1,1);
		System.Int32.TryParse(token,out pieceId);
	}
	
	void ToggleVisibility(GameObject obj)
	{
		int i;
		
		Renderer[] lRenderers = obj.GetComponentsInChildren<Renderer>();
		for (i=0;i<lRenderers.Length;i++) {
			lRenderers[i].enabled = !lRenderers[i].enabled;
		}
	}
	
	void SetVisibility(GameObject obj, bool v)
	{
		int i;
		
		Renderer[] lRenderers = obj.GetComponentsInChildren<Renderer>();
		for (i=0;i<lRenderers.Length;i++) {
			lRenderers[i].enabled = v;
		}
	}
	
	void Move(GameObject obj,Vector3 t)
	{
		Transform lTransform = obj.GetComponent<Transform>();
		lTransform.position = t;
	}
	
	void CopyTranform(GameObject obj,GameObject org)
	{
		Transform lTransform = obj.GetComponent<Transform>();
		lTransform.position = org.transform.position;
		lTransform.rotation = org.transform.rotation;
	}
	
	void CopyTranform(GameObject obj,Transform t0)
	{
		Transform lTransform = obj.GetComponent<Transform>();
		lTransform.position = t0.position;
		lTransform.rotation = t0.rotation;
	}
	
	void CopyTranform(GameObject obj,Vector3 t,Quaternion r)
	{
		Transform lTransform = obj.GetComponent<Transform>();
		lTransform.position = t;
		lTransform.rotation = r;
	}
	
	int FindInteractPiece(GameObject piece)
	{
		int i;
		GameObject obj;
		for (i=0; i<InteractPieces; i++)
		{
			obj = AssembledPieces [i];
			if(obj==piece)
				return i;
		}
		
		return -1;
	}
	
	int FindCorrectAssemblePiece()
	{
		/// WARNING --- the correct assemble piece, is a piece that other user need assembling
		int userID;
		
		if (currentUserID == 1)
			userID = 2;
		else
			userID = 1;
		
		int i;
		int ui,pi;
		GameObject piece;
		for (i=0; i<InteractPieces; i++)
		{
			piece = AssembledPieces [i];
			GetPieceId(piece,out ui,out pi);
			
			if(ui == userID && pi==currentUserID)
				return i;
		}
		
		return -1;
	}
	
	public bool IsAvailableAssembling()
	{
		if(AssembledFlag [currentUserID-1, currentAssemblyID-1])
			return false;
		else
			return (currentErrorPiece != null );
	}
	
	public void TryAssemble(GameObject piece)
	{
		if (currentErrorPiece != null)
			return;
		
		int userId;
		int pieceId;
		
		GameObject gap;
		
		GetPieceId(piece,out userId, out pieceId);
		
		gap = TempleatePieces [currentUserID-1, currentAssemblyID-1];			
		loadTest.Log ("<####> TRY_ASSEMBLE," + userId + "," + pieceId + "," + piece.name + "," + "," + Time.fixedTime);
		
		bool Assembled = false;
		if(userId>0 && pieceId>0)
		{
			if (userId == currentUserID && pieceId == currentAssemblyID)
				Assembled = true;
			else
				Assembled = false;
		}
		else
			Assembled = false;
		
		
		if (Assembled){
			if(GetComponent<NetworkView>().isMine)
			{
				loadTest.Log ("<####> ASSEMBLED_OBJECT," + userId + "," + pieceId + "," + piece.name + "," + "," + Time.fixedTime);
				GetComponent<NetworkView>().RPC ("AssemblePiece", RPCMode.All,piece.name,currentUserID,currentAssemblyID);
			}
		}
		else
		{
			currentErrorPiece = piece;
			lastErrorTime = Time.fixedTime;
			
			if(GetComponent<NetworkView>().isMine)
			{
				loadTest.Log ("<####> ASSEMBLE_ERROR,"+userId+","+pieceId+ "," + piece.name + "," +","+Time.fixedTime);
				GetComponent<NetworkView>().RPC ("ShowBadAssemblePiece", RPCMode.All,piece.name,currentUserID,currentAssemblyID);
			}

			BadAssembled++;
			//SendUpdateScore(Score.ScBadAssembled,1);
		}
	}
	
	public int GetPieceOrignalTable(GameObject piece)
	{
		int index = FindInteractPiece (piece);
		
		if (index >= 0)
		{
			if (T0AssembledPieces [index].z > -8.0f)
				return 1;
			else
				return 2;
		}
		
		return -1;	
	}
	
	[RPC]
	public void BackPieceToOriginalPlace(string objName)
	{
		GameObject piece =  GameObject.Find (objName);
		if(piece!=null)
		{
			int index = FindInteractPiece (piece);
			piece.transform.rotation = R0AssembledPieces [index];
			piece.transform.position = T0AssembledPieces [index];
		}
	}
	
	[RPC]
	void AssemblePiece(string objName,int uID, int pId)
	{
		int userId;
		int pieceId;
		
		GameObject piece = GameObject.Find (objName);		
		
		if(piece != null)
		{		
			piece.transform.rotation = TempleatePieces [uID-1, pId-1].transform.rotation;
			piece.transform.position = TempleatePieces [uID-1, pId-1].transform.position;
		}
		HidePiece (TempleatePieces [uID - 1, pId - 1].name);
		TempleatePieces [uID-1, pId-1].tag = "Assembled";
		AssembledFlag [uID-1, currentAssemblyID-1] = true;
		asembledPiecesCount++;
		currentAssemblyID++;
		GoodAssembled ++;
		
		if(currentAssemblyID>TemplatesByUser)
			currentAssemblyID=TemplatesByUser;
	}
	
	[RPC]
	void ShowBadAssemblePiece(string objName,int uID, int pId)
	{
		int userId;
		int pieceId;
		
		GameObject piece = GameObject.Find (objName);		
		
		if(piece != null)
		{		
			piece.transform.rotation = TempleatePieces [uID-1, pId-1].transform.rotation;
			piece.transform.position = TempleatePieces [uID-1, pId-1].transform.position;
		}
	}
	
	[RPC]
	void HidePiece(string objName)
	{
		int i;
		GameObject obj =  GameObject.Find (objName);
		Renderer[] lRenderers = obj.GetComponentsInChildren<Renderer>();
		for (i=0;i<lRenderers.Length;i++) {
			lRenderers[i].enabled = false;
		}
	}
}
/*

int historyi = 0;
	Quaternion[] HistoryQ;
	Quaternion LastQ = new Quaternion();

HistoryQ = new Quaternion[5]; 
 * 
 * 
// by wjs
			Quaternion dQ = new Quaternion();
			dQ = Quaternion.Inverse(LastQ)*qt;

			if( Mathf.Abs(dQ.x)>0.1f ||
			    Mathf.Abs(dQ.y)>0.1f ||
			    Mathf.Abs(dQ.z)>0.1f ||
			    Mathf.Abs(dQ.w)>0.1f  ){
				LastQ = qt;
				HistoryQ[historyi] = LastQ;
			}

			if(historyi>4)
			{
				int i = 0;
				dQ.Set(0,0,0,0);
				for(i=0;i<4;i++)
				{
					dQ.x += HistoryQ[i].x;
					dQ.y += HistoryQ[i].y;
					dQ.z += HistoryQ[i].z;
					dQ.w += HistoryQ[i].w;
				}

				dQ = inverseInitialRot*dQ;

				if( Mathf.Abs(dQ.x)>0.1f ||
				   	Mathf.Abs(dQ.y)>0.1f ||
				  	Mathf.Abs(dQ.z)>0.1f ||
				   	Mathf.Abs(dQ.w)>0.1f  ){

					loadTest.Log ("!!!!Se gerou um erroo ten te recalibrar ......................,"+Time.fixedTime);					
				}
			}

			// end by wjs
***
*
*

LastQ = getSensicsQuat ();


*


*/
