using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	private const string typeName = "AES-Risk";
	private const string gameName = "Proactive Haptic";
	private HostData[] hostList;
	
	public bool isGameMultiplayer = false;
	public bool deactivateGUI = false;

	int networkObjects = 0;

	public static void AddSyncToObject(GameObject obj)
	{
		if(obj.GetComponent<NetworkView>() == null)
			obj.AddComponent<NetworkView>();

		obj.GetComponent<NetworkView>().viewID = Network.AllocateViewID();

		if(obj.transform.childCount > 0)
		{
			foreach(Transform child in obj.transform)
				AddSyncToObject(child.gameObject);
		}
	}

	void Start()
	{
		List<Object> objects = GameObject.FindSceneObjectsOfType(typeof (GameObject)).ToList();
		List<GameObject> gameObjects = new List<GameObject>();

		foreach(Object obj in objects)
		{
			gameObjects.Add ((GameObject) obj);
		}

		foreach(GameObject obj in gameObjects)
		{
			int name_number_sufix = 1;
			foreach(GameObject obj2 in objects)
			{
				if(obj.name == obj2.name && obj != obj2)
				{
					name_number_sufix ++;
					obj2.name = obj2.name + " " + name_number_sufix;
				}
			}

			if(obj.tag == "Interactable" || obj.tag == "Riscos" || obj.tag == "Network Object" || obj.tag == "CanAssembled" || obj.tag == "Template")
			{
				AddNetworkTagToChildren(obj.transform);
			}
		}
	}

	private void AddNetworkTagToChildren(Transform trans)
	{
		if(trans.gameObject.tag != "Interactable" && trans.gameObject.tag != "Riscos" && trans.gameObject.tag != "CanAssembled" && trans.gameObject.tag != "Template")
			trans.gameObject.tag = "Network Object";
		
		if(trans.GetChildCount() > 0)
			foreach(Transform t in trans)
				AddNetworkTagToChildren(t);
	}

	// Server registration
	private void StartServer()
	{
		// Remove this lines if you want to use Unity's Master Server
		MasterServer.ipAddress = "143.54.13.238"; // The ip of the host. For some reason 127.0.0.1 does not work.
		MasterServer.port = 23466; // Port is defined on the master server
		// End remove lines

		Network.minimumAllocatableViewIDs = 100;
		Network.InitializeServer(2, 25000, !Network.HavePublicAddress());
		//MasterServer.ipAddress = "127.0.0.1"; // To run the MasterServer locally, instead of using the MasterServer run by Unity (it could be down due to maintenance).
		MasterServer.RegisterHost(typeName, gameName);
	}

	// This method is called when the server sucessfully initializes.
	void OnServerInitialized()
	{
		// Instantiate the player in multiplayer mode.
		GameObject.Find("InputDeviceManager").GetComponent<InputDeviceManager>().InstantiatePlayer();
		GetComponent<NetworkView>().stateSynchronization = NetworkStateSynchronization.ReliableDeltaCompressed;

		object[] obj = GameObject.FindSceneObjectsOfType(typeof (GameObject));
		foreach (object o in obj)
		{
			GameObject g = (GameObject) o;
			if(g.tag == "Interactable" || g.tag == "Riscos" || g.tag == "Network Object" || g.tag == "CanAssembled" || g.tag == "Template")
			{
				DefaultInformation dfInf = g.GetComponent<DefaultInformation>(); 
				if(dfInf != null) // wjs
					dfInf.AddNetworkSync();
			}
		}
	}

	// Search for a hostList (the data required to join a server) at the MasterServer
	private void RefreshHostList()
	{
		// Remove this lines if you want to use Unity's Master Server
		MasterServer.ipAddress = "143.54.13.238"; // must be the same as the host
		MasterServer.port = 23466; // Port is defined on the master server
		// End remove lines

		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
	
	private void JoinServer(HostData hostData)
	{
		Network.minimumAllocatableViewIDs = 100;
		Network.Connect(hostData);
	}

	[RPC]
	void RetrieveObjectSync(string objname)
	{
		Debug.Log ("Server receiving: " + objname);
		GameObject serverObj = GameObject.Find (objname);

		NetworkView netView = serverObj.GetComponent<NetworkView> ();

		if(netView != null) // wjs
		{
			NetworkViewID viewID = netView.viewID;
			GetComponent<NetworkView>().RPC ("ReceiveObjectSync", RPCMode.Others, objname, viewID, GetComponent<NetworkView>().owner);
		}
	}

	[RPC]
	void ReceiveObjectSync(string objname, NetworkViewID viewID, NetworkPlayer server)
	{
		Debug.Log ("Client receiving: " + objname);
		GameObject clientObj = GameObject.Find (objname);
		clientObj.AddComponent<NetworkView>();
		clientObj.GetComponent<NetworkView>().viewID = viewID;
		clientObj.GetComponent<NetworkView>().stateSynchronization = NetworkStateSynchronization.Unreliable;
	}

	// This method is called after we actually joined the server.
	void OnConnectedToServer()
	{
		// Instantiate the player in multiplayer mode.
		GameObject.Find("InputDeviceManager").GetComponent<InputDeviceManager>().InstantiatePlayer();
		GetComponent<NetworkView>().stateSynchronization = NetworkStateSynchronization.ReliableDeltaCompressed;

		object[] obj = GameObject.FindSceneObjectsOfType(typeof (GameObject));
		foreach (object o in obj)
		{
			GameObject g = (GameObject) o;
			if(g.tag == "Interactable" || g.tag == "Riscos" || g.tag == "Network Object" || g.tag == "CanAssembled" || g.tag == "Template")
			{
				GetComponent<NetworkView>().RPC("RetrieveObjectSync", RPCMode.Server, g.name);
			}
		}


	}

	// Just some GUI buttons to test the server
	void OnGUI()
	{
		if(!deactivateGUI)
		{
			if(!isGameMultiplayer)
			{
				/*if(GUI.Button(new Rect(100, 100, 250, 75), "Single Player"))
				{
					isGameMultiplayer = false;
					deactivateGUI = true; // The game started, deactivate the test GUI.

					// Instantiate the player in single player mode.
					GameObject.Find("InputDeviceManager").GetComponent<InputDeviceManager>().InstantiatePlayer();
				}*/
				if(GUI.Button(new Rect(100, 250, 250, 75), "Multiplayer"))
				{
					isGameMultiplayer = true;
				}
			}
			else // It's a multiplayer game
			{
				// If the player is disconnected... (it is not a client nor a server)
				//if(!Network.isClient && !Network.isServer)
				{
					// ...show these buttons.
					if(GUI.Button(new Rect(100, 100, 250, 75), "Start Server"))
					{
						StartServer();
						deactivateGUI = true; // The game started, deactivate the test GUI.
					}

					if(GUI.Button(new Rect(100, 250, 250, 75), "Refresh Hosts"))
						RefreshHostList();

					if(hostList != null)
					{
						for (int i = 0; i < hostList.Length; i++)
						{
							if(GUI.Button(new Rect (400, 100 + (110 * i), 300, 75), hostList[i].gameName+" IP  "+hostList[i].ip[0]))
							{
								JoinServer(hostList[i]);
								deactivateGUI = true; // The game started, deactivate the test GUI.
							}
						}

					}
				}
			}
		}
	}
}