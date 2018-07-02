using UnityEngine;
using System;
using System.Data;
using System.Collections;
using System.ComponentModel.Design;
using MySql.Data;
using MySql.Data.MySqlClient;

public class ConnectToMySQL : MonoBehaviour {

	public string serverAdress;
	public int port = 88;
	public string databaseName;
	public string user = "admin";
	public string password = "mudar123";
	public bool pooling = true;

	// connection object
	private MySqlConnection con = null;
	// command object
	private MySqlCommand cmd = null;
	// reader object
	private MySqlDataReader rdr = null;


	public static string USER_TABLE_NAME = "perfilusuario";


	void Awake () {
		string databaseConnectionStr = 
				"Server="+serverAdress+";"+
				"Database="+databaseName+";"+
				"User ID="+user+";"+
				"Password="+password+";"+
				"Pooling="+pooling.ToString()+";";

		try
		{
			// setup the connection element
			Debug.Log (databaseConnectionStr);
			con = new MySqlConnection(databaseConnectionStr);
			
			// lets see if we can open the connection
			con.Open();
			Debug.Log("Connection State: " + con.State);
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
	}


	void insertEntry(string table, string[] fields) {

	}

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnApplicationQuit()
	{
		Debug.Log("killing con");
		if (con != null)
		{
			if (con.State.ToString() != "Closed")
				con.Close();
			con.Dispose();
		}
	}
}
