using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class objectSelector : MonoBehaviour {

	public Camera referenceCamera;
	List<GameObject> selectedObjects;
	public GameObject selectionLightPrefab;
	public Shader glowingShader;
	public Shader transparentGlowingShader;

	// Use this for initialization
	void Start () {
		selectedObjects = new List<GameObject>();
	}
	
	void SelectObject(GameObject obj)
	{
		for(int i=0; i<obj.transform.childCount; i++)
		{
			SelectObject(obj.transform.GetChild(i).gameObject);
		}

		if(obj.GetComponent<Renderer>() != null)
		{
			List<Material> objmats = new List<Material>(obj.GetComponent<Renderer>().materials.ToList());

			List<Material> objSelectionMaterials = new List<Material>();

			foreach(Material mat in objmats)
			{
				Material selectionMaterial = new Material(mat);

				if(mat.shader.name == "Transparent/Bumped Diffuse" || mat.shader.name == "Transparent/Diffuse" || mat.shader.name == "Nature/Tree Soft Occlusion Leaves" ||
				   mat.shader.name == "Custom/TropicalPlantsAOWaving")
				{
					selectionMaterial.shader = transparentGlowingShader;
					selectionMaterial.SetFloat("_RimStrength", 1f);
				}
				else
				{
					selectionMaterial.shader = glowingShader;
					selectionMaterial.SetFloat("_RimStrength", 2f);
				}

				selectionMaterial.SetColor("_ColorTint", new Color(1.0f, 0.5f, 0.5f, mat.color.a));
				selectionMaterial.SetColor("_RimColor", Color.red);

				objSelectionMaterials.Add (selectionMaterial);
			}

			obj.GetComponent<Renderer>().materials = objSelectionMaterials.ToArray();
		}
		
		return;
	}
	
	void DeselectObject(GameObject obj)
	{
		for(int i=0; i<obj.transform.childCount; i++)
		{
			DeselectObject(obj.transform.GetChild(i).gameObject);
		}

		if(obj.GetComponent<Renderer>() != null)
		{
			List<Material> objmats = obj.GetComponent<Renderer>().materials.ToList();

			objmats = obj.GetComponent<DefaultInformation>().defaultMaterialList;

			obj.GetComponent<Renderer>().materials = objmats.ToArray();
		}
	}

	[RPC]
	void SelectObjectNetID(NetworkViewID id)
	{
		SelectObject(NetworkView.Find(id).gameObject);
	}

	[RPC]
	void DeselectObjectNetID(NetworkViewID id)
	{
		DeselectObject(NetworkView.Find(id).gameObject);
	}

	
	// Update is called once per frame
	void Update () {
		if(GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isGameMultiplayer == true)
		{
			if(GetComponent<NetworkView>().isMine) // This test is needed to only control MY character during network play! (to not control OTHER players's characters)
			{
				ProcessObjSelectorInput();
			}
		}
		else
			ProcessObjSelectorInput ();
	}


	void ProcessObjSelectorInput()
	{
		Ray ray = new Ray(referenceCamera.transform.position, referenceCamera.transform.forward);
		if(Input.GetButtonUp("Fire1"))
		{
			RaycastHit hitinfo = new RaycastHit();
			Physics.Raycast(ray, out hitinfo);
			if(hitinfo.collider != null)
			{
				GameObject hitobj = hitinfo.collider.gameObject;
				if(hitobj != null)
				{
					GenericRisk hitobjRisk = hitobj.GetComponent<GenericRisk>();
					if(hitobj.tag == "Interactable" || hitobj.tag == "Riscos")
					{
						if(!selectedObjects.Contains (hitobj))
						{
							
							selectedObjects.Add (hitobj);
							if (hitobjRisk != null)
								hitobjRisk.setRiskFound(true);
							
							SelectObject (hitobj);

							if(!GetComponent<NetworkView>().isMine)
								return;

							GetComponent<NetworkView>().RPC("SelectObjectNetID", RPCMode.All, hitobj.GetComponent<NetworkView>().viewID);
						}
						else
						{
							selectedObjects.Remove (hitobj);
							if (hitobjRisk != null)
							{
								hitobjRisk.setRiskFound(false);
								hitobjRisk.riskTime = 0; // At the performance screen, if the risk is unmarked ("not found"), it musn't show any "time the player took to find the risk".
							}
							DeselectObject(hitobj);

							if(!GetComponent<NetworkView>().isMine)
								return;
							
							GetComponent<NetworkView>().RPC("DeselectObjectNetID", RPCMode.All, hitobj.GetComponent<NetworkView>().viewID);
						}
					}
				}
			}
		}
	}
}
