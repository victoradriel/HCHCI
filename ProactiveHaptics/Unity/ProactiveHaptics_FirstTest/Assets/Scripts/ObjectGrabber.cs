using UnityEngine;
using System.Collections;

public class ObjectGrabber : MonoBehaviour {
	
	// Este script deve ser colocado nas maos dos personagens, e serve para que o jogador possa
	// agarrar os objetos.
	
	// A mao podera apenas segurar um objeto de cada vez.
	private MoveFingers 	glove = null;
	private GameObject 		objectGrabbed = null;
	private GameObject      SwappingObject = null;
	private puzzleRules		logic = null;		
	private bool			inAssemblingZone = false;
	private bool			inSwappingZone = false;
	private bool			enabled2Take = false;
	private bool			enabled2Grabbe = false;
	private string			strHand;
	private bool			isRHand;
	private bool			isLocal;
	private int 			BadGrabedCount = 0;
	//GameObject MaoDireita;
	//GameObject MaoEsquerda;
	
	static private GameObject 	lastGrabbed = null;
	static private float 		lastTimeSwapping;
	
	void Start()
	{
		if(name=="Bip01-L-Hand")
		{
			strHand =  "LEFT";
			isRHand = false;
		}
		else if(name=="Bip01-R-Hand")
		{
			strHand =  "RIGHT";
			isRHand = true;
		}
		
		
		lastTimeSwapping = Time.fixedTime;

		/*MaoEsquerda = GameObject.Find("MaoEsquerda");
		MaoDireita = GameObject.Find("MaoDireita");*/
	}
	
	void Update()
	{
		int i;
		if(glove==null)
		{
			MoveFingers[] objs = FindObjectsOfType(typeof(MoveFingers)) as MoveFingers[];
			
			for(i=0;i<objs.Length;i++)
			{
				if(objs[i].IsLocal())
					glove = objs[i];
			}
		}
		if(logic==null)
		{
			puzzleRules[] objs = FindObjectsOfType(typeof(puzzleRules)) as puzzleRules[];
			
			for(i=0;i<objs.Length;i++)
			{
				if(objs[i].IsLocal())
					logic = objs[i];
			}
		}
		
		if(objectGrabbed != null)
		{
			if(GetComponent<NetworkView>().isMine)
			{							
				if(objectGrabbed.tag == "Taken")
				{
					if(objectGrabbed.transform.parent != transform)
						GetComponent<NetworkView>().RPC ("FreePiece", RPCMode.All,objectGrabbed.gameObject.name);
				}
				else
				{
					if(!inSwappingZone)
					{
						if(isReady2Release())
							ProcessRelease();
					}
				}
			}
		}
		
		if(SwappingObject==null)
			SwappingObject = GameObject.FindGameObjectWithTag("Swapping Zone");
		
		isLocal = GetComponent<NetworkView>().isMine;
		/*
		// Maos
		if (glove.isCloseLeftHand ()) {
			// esquerda fechada
			MaoEsquerda.guiText.text = "Esquerda: PEGOU A PEÇA";
		} else if (!glove.isCloseLeftHand ()) {
			// esquerda aberta
			MaoEsquerda.guiText.text = "Esquerda: soltou a peça";
		} 

		if (glove.isCloseRightHand ()) {
			// direita fechada
			MaoDireita.guiText.text = "Direita: PEGOU A PEÇA";
		} else if (!glove.isCloseRightHand ()) {
			// direita aberta
			MaoDireita.guiText.text = "Direita: soltou a peça";
		}*/
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Table")
		{
			inAssemblingZone = true;
			loadTest.Log ("<####> EnterAssemblingZone,"+logic.GetUserID()+","+strHand+","+inAssemblingZone+","+Time.fixedTime);
		}
		else if(other.gameObject.tag == "CanAssembled")
		{
			enabled2Grabbe = (objectGrabbed==null);
			loadTest.Log ("<####> EnterPieceCollition,"+logic.GetUserID()+","+strHand+","+Time.fixedTime);
		}
		else if (other.gameObject.tag == "Swapping Zone")
		{
			inSwappingZone = true;
			enabled2Take = (objectGrabbed==null);
			loadTest.Log ("<####> EnterSwapping Zone,"+logic.GetUserID()+","+strHand+","+enabled2Take+","+Time.fixedTime);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Swapping Zone")
		{
			inSwappingZone = false;
			enabled2Take = false;
			loadTest.Log ("<####> ExitSwapping Zone,"+logic.GetUserID()+","+strHand+","+Time.fixedTime);
		}
		
		if (other.gameObject.tag == "Table")
		{
			inAssemblingZone = false;
			loadTest.Log ("<####> ExitAssemblingZone,"+logic.GetUserID()+","+strHand+","+inAssemblingZone+","+Time.fixedTime);
		}
		
		if (other.gameObject.tag == "CanAssembled")
		{
			enabled2Grabbe = false;
			loadTest.Log ("<####> ExitPieceCollition,"+logic.GetUserID()+","+strHand+","+inAssemblingZone+","+Time.fixedTime);
		}
		
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Table")
		{
			inAssemblingZone = true;
			loadTest.Log ("<####> inAssemblingZone,"+logic.GetUserID()+","+strHand+","+inAssemblingZone+","+Time.fixedTime);
		}
		else if(other.gameObject.tag == "CanAssembled")
		{
			if(enabled2Grabbe)
			{
				loadTest.Log ("<####> inPieceCollition,"+logic.GetUserID()+","+strHand+","+Time.fixedTime);
				
				if(isReady2Grabbe())
				{
					if(other.gameObject.transform.parent == null)
					{
						if(GetComponent<NetworkView>().isMine)
						{			
							GetComponent<NetworkView>().RPC ("GrabbePiece", RPCMode.All,other.gameObject.name);
						}
					}
					else
					{
						if(isReady2SwappingHand(other.gameObject))
						{
							if(GetComponent<NetworkView>().isMine)
							{
								loadTest.Log ("<####> SwapingHands,"+logic.GetUserID()+","+strHand+","+Time.fixedTime);
								GetComponent<NetworkView>().RPC ("TakingPiece", RPCMode.All,other.gameObject.name);
								lastTimeSwapping = Time.fixedTime;
							}
						}
					}
				}
				
				enabled2Grabbe = false;
			}
		}
		else if (other.gameObject.tag == "Swapping Zone")
		{			 	
			loadTest.Log ("<####> inSwappingZone,"+logic.GetUserID()+","+strHand+","+Time.fixedTime);
			inSwappingZone = true;
			
			if(enabled2Take)
			{
				if(glove == null)
					return;
				
				bool ready2Grabbe = false;
				
				if(strHand ==  "LEFT")
					ready2Grabbe = glove.isCloseLeftHand();
				else if(strHand=="RIGHT")
					ready2Grabbe = glove.isCloseRightHand();
				
				if(ready2Grabbe)
				{
					if(logic == null)
						return;
					
					if(SwappingObject==null)
						return;
					
					GameObject[] query = logic.GetAssembledPieces(); 
					
					int i;
					for(i=0;i<query.Length;i++)
					{
						if(SwappingObject.GetComponent<Collider>().bounds.Contains(query[i].transform.position))
						{
							if(GetComponent<NetworkView>().isMine)
							{
								loadTest.Log ("<####> SwapingObj,"+logic.GetUserID()+","+strHand+","+Time.fixedTime);
								GetComponent<NetworkView>().RPC ("TakingPiece", RPCMode.All,query[i].name);
							}
							break;
						}
					}
				}
			}
		}
	}
	
	bool isReady2Grabbe()
	{
		if (objectGrabbed == null)
		{
			bool ready2Grabbe =  false;
			
			if(glove != null)
			{
				if(strHand ==  "LEFT")
					ready2Grabbe = glove.isCloseLeftHand();
				else if(strHand=="RIGHT")
					ready2Grabbe = glove.isCloseRightHand();
				
				if(ready2Grabbe)
					loadTest.Log ("<####> isReady2Grabbe,"+logic.GetUserID()+","+strHand+","+Time.fixedTime);
				
				return ready2Grabbe;
			}	
			else return false;	
		}
		else return false;
	}
	
	bool isReady2Release()
	{
		if (objectGrabbed != null) 
		{
			bool ready2Release =  false;
			
			if(glove != null)
			{
				if(strHand ==  "LEFT")
					ready2Release = !glove.isCloseLeftHand();
				else if(strHand=="RIGHT")				
					ready2Release = !glove.isCloseRightHand();					
				
				if(ready2Release)
					loadTest.Log ("<####> isReady2Release,"+logic.GetUserID()+","+strHand+","+Time.fixedTime);
				
				return ready2Release;
			}
			return false;
		}
		return false;
	}
	
	bool isReady2SwappingHand(GameObject toSwapp)
	{
		bool ready2swapping =  false;
		if(lastGrabbed == toSwapp)
		{
			if ((Time.fixedTime - lastTimeSwapping) > 5)
			{
				ready2swapping = (objectGrabbed==null);
			}
		}
		else
		{
			ready2swapping = (objectGrabbed==null);
		}
		
		if(ready2swapping)
			loadTest.Log ("<####> isReady2Swapping,"+logic.GetUserID()+","+strHand+","+Time.fixedTime);
		
		return ready2swapping;
	}
	
	void ProcessRelease()
	{	
		if (objectGrabbed!=null) 
		{
			
			if(objectGrabbed.transform.parent != transform)
				return;
			
			GameObject toAssembling = objectGrabbed;
			
			if(logic!=null)
			{
				int usrId 	= logic.GetUserID();
				int tableId = logic.GetPieceOrignalTable(toAssembling);
				
				if(inAssemblingZone)
				{
					if(usrId != tableId)
					{
						if(!logic.IsAvailableAssembling())
						{	
							if(GetComponent<NetworkView>().isMine)
							{
								GetComponent<NetworkView>().RPC ("ReleasePiece", RPCMode.All,objectGrabbed.name);
								logic.TryAssemble(toAssembling);
							}

							BadGrabedCount = 0;
							
							return;
						}
					}
					else
					{
						if(GetComponent<NetworkView>().isMine)
						{
							GetComponent<NetworkView>().RPC ("ReleasePiece", RPCMode.All,objectGrabbed.name);												
							GetComponent<NetworkView>().RPC ("BackPieceToOriginalPlace", RPCMode.All,toAssembling.name);
							BadGrabedCount++;
							return;
						}
					}
				}
				else
				{
					if(GetComponent<NetworkView>().isMine)
					{
						GetComponent<NetworkView>().RPC ("ReleasePiece", RPCMode.All,objectGrabbed.name);										
						GetComponent<NetworkView>().RPC ("BackPieceToOriginalPlace", RPCMode.All,toAssembling.name);
						return;
					}
				}
			}
			
		}
	}
	
	[RPC]
	void GrabbePiece(string nameObj)
	{
		GameObject obj = GameObject.Find (nameObj);
		
		// Verifica se o jogador que agarrou eh este jogador
		//if(obj.transform.root != this.transform.root)
		{
			//if(obj.transform.parent==null)
			{
				if(obj.GetComponent<Rigidbody>() != null)
					obj.GetComponent<Rigidbody>().useGravity = false;
				
				obj.GetComponent<Collider>().isTrigger = true;
				
				obj.transform.parent = null;
				Vector3 nScale =  new Vector3();
				nScale.x = 1.5f;
				nScale.y = 1.5f;
				nScale.z = 1.5f;
				
				obj.transform.localScale = nScale;
				
				obj.transform.parent = transform;
				
				Vector3 npos =  new Vector3();
				
				// Positon of collition sphere
				npos.x = -0.09f;
				npos.y =  0.02f;
				npos.z =  0.00f;
				
				obj.transform.localPosition = npos;
				
				loadTest.Log ("<####> GRABBE_OBJ,"+logic.GetUserID()+","+strHand+","+obj.name+","+Time.fixedTime);
				objectGrabbed=obj;
				lastGrabbed = obj;
				
				enabled2Grabbe = false;
			}
		}
	}
	
	[RPC]
	void ReleasePiece(string objName)
	{
		GameObject obj = GameObject.Find (objName);
		
		if (obj!=null) 
		{
			if (obj.GetComponent<Rigidbody>() != null)			
				obj.GetComponent<Rigidbody>().useGravity = true;
			
			GameObject toDrop = obj;
			
			obj.GetComponent<Collider>().isTrigger = false;
			obj.transform.parent = null;
			
			Vector3 nScale =  new Vector3();
			nScale.x = 1.0f;
			nScale.y = 1.0f;
			nScale.z = 1.0f;
			obj.transform.localScale = nScale;
			
			obj = null;
			
			loadTest.Log ("<####> RELEASE_OBJ,"+logic.GetUserID()+","+strHand+","+toDrop.name+","+Time.fixedTime);
			objectGrabbed=null;
			enabled2Grabbe = false;
		}
		
	}
	
	[RPC]
	void TakingPiece(string objName)
	{
		GameObject obj = GameObject.Find (objName);
		
		if(obj != null)
		{
			//if(obj.transform.root != this.transform.root)
			{
				if(obj.GetComponent<Rigidbody>() != null)
					obj.GetComponent<Rigidbody>().useGravity = false;
				
				obj.GetComponent<Collider>().isTrigger = true;
				
				obj.transform.parent = null;
				Vector3 nScale =  new Vector3();
				nScale.x = 1.5f;
				nScale.y = 1.5f;
				nScale.z = 1.5f;
				
				obj.transform.localScale = nScale;
				
				obj.transform.parent = transform;
				
				Vector3 npos =  new Vector3();
				
				// Positon of collition sphere
				npos.x = -0.09f;
				npos.y =  0.02f;
				npos.z =  0.00f;
				
				obj.transform.localPosition = npos;
				obj.tag = "Taken";
				
				loadTest.Log ("<####> TAKE_OBJ,"+logic.GetUserID()+","+strHand+","+obj.name+","+Time.fixedTime);
				objectGrabbed=obj;
				lastGrabbed = obj;
				
				enabled2Take = false;
			}
		}
	}
	
	[RPC]
	void FreePiece(string objName)
	{
		GameObject obj = GameObject.Find (objName);
		
		if(obj != null)
			obj.tag = "CanAssembled";
		objectGrabbed = null;
		
		loadTest.Log ("<####> FREE_OBJ,"+logic.GetUserID()+","+strHand+","+obj.name+","+Time.fixedTime);
	}
	
	[RPC]
	void BackPieceToOriginalPlace(string objName)
	{
		if(logic != null)		
			logic.BackPieceToOriginalPlace(objName);
		
		loadTest.Log ("<####> HOMEBACK_OBJ,"+logic.GetUserID()+","+strHand+","+objName+","+Time.fixedTime);
	}
	
}
