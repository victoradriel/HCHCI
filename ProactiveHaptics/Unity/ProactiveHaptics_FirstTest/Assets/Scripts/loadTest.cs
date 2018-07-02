using UnityEngine;
using System.Collections;
using System.IO;

public class loadTest : MonoBehaviour {
	static StreamWriter logFile;
	static StreamReader logFileR;
	static private string strTestID = "";
	static private string strError	= "";
	public bool testGui = true;

	private class testConfig
	{
		public string 	scene;
		public bool		increaseSound;
		public bool		serverUserLeftSide;
	};
	
	public static int				testID = 0;
	public static int				subtest = 0;
	private static testConfig[,] 	TestConfig;
	
	// Use this for initialization
	void Start () {
		ReadTestConfiguration ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.X))
		{
			Debug.Log ("<####> Exit,"+Time.fixedTime);	
			Application.Quit();
		}
	}

	static public void NextTest()
	{
			loadTest.Log ("NextTesti,"+GetCurrenDescription()+","+Time.fixedTime);

			subtest++;
			if(subtest>4)
			{
				subtest = 0;
				testID++;
				
				if(testID>47)
					testID =0;
			}

			loadTest.Log ("NextTeste,"+GetCurrenDescription()+","+Time.fixedTime);
	}

	static public void LoadScene()
	{
		if(TestConfig==null)
			Debug.Log ("<####> ERROR TestConfig NULL,"+Time.fixedTime);
		else
		{

			if(logFile!=null)			
				logFile.Close();

			if(logFileR!=null)
				logFileR.Close();

			string fileName = "";
			
			if(Application.isEditor)
			{
				if(Application.platform == RuntimePlatform.WindowsEditor)
					fileName =  "D:\\";
				else if(Application.platform == RuntimePlatform.OSXEditor)
					fileName = "/Users/wjsarmiento/";
			}
			else			
				fileName+= 	Application.dataPath+Path.DirectorySeparatorChar;

			fileName += GetCurrenDescription()+"_"+System.DateTime.Now.ToString("yyMMddHHmmss")+".log";

			FileStream fw = new FileStream(fileName,FileMode.Append,FileAccess.Write,FileShare.ReadWrite);
			logFile = new StreamWriter(fw);

			//FileStream fr = new FileStream(fileName,FileMode.Open,FileAccess.Read,FileShare.Read);
			//logFileR = new StreamReader(fr);

			Application.LoadLevel(TestConfig[testID,subtest].scene);
		}

	}

	static public void Log(string line)
	{
		logFile.WriteLine(line);
		logFile.Flush ();

		//logFileR.ReadLine ();
	}

	static public bool GetCurrentIncreaseSound()
	{
		if(TestConfig==null)
			Debug.Log ("<####> ERROR TestConfig NULL,"+Time.fixedTime);
		else
			return TestConfig [testID, subtest].increaseSound;

		return false;
	}

	static public bool GetCurrentServerUserLeftSide()
	{
		if(TestConfig==null)
			Debug.Log ("<####> ERROR TestConfig NULL,"+Time.fixedTime);
		else
			return TestConfig [testID, subtest].serverUserLeftSide;

		return false;
	}

	static public string GetCurrenScene()
	{
		return TestConfig [testID, subtest].scene;
	}

	static public string GetCurrenDescription()
	{
		string description = "Test-ID" + (testID + 1);

		if(subtest>0)
			description+="_Test"+subtest;
		else
			description+="_Prestest";

		return description;
	}

	static public void ReadTestConfiguration()
	{
		if (TestConfig != null)
			return;
		
		TestConfig = new testConfig[48, 5]; 
		
		char[] 		charsep = new char[] {','};
		string 		line;
		string[] 	tokens;

		string fileName = "";

		if(Application.isEditor)
		{
			if(Application.platform == RuntimePlatform.WindowsEditor)
				fileName =  "D:\\TesteProactiveHaptic.csv";
			else if(Application.platform == RuntimePlatform.OSXEditor)
				fileName = "/Users/wjsarmiento/Desktop/aes_multiplayer_project/TesteProactiveHaptic.csv";
		}
		else
		{
			fileName = 	Application.dataPath+
						Path.DirectorySeparatorChar+
						"TesteProactiveHaptic.csv";
		}

		StreamReader cnfFile = new StreamReader (fileName);


		if (cnfFile == null)
		{
			strError = "Config File not found";
			return;
		}

		line = cnfFile.ReadLine ();
		line = cnfFile.ReadLine ();
		
		int id;
		int test;
		int tki;
		
		id = 0;
		while(line != null)
		{
			tokens = line.Split(charsep);
			
			TestConfig[id,0] = new testConfig();
			
			TestConfig[id,0].scene 				= "pre-teste";
			TestConfig[id,0].increaseSound 		= true;
			TestConfig[id,0].serverUserLeftSide 	= true;
			
			tki = 4;
			for(test=1;test<5;test++,tki++)
			{
				TestConfig[id,test] = new testConfig();
				TestConfig[id,test].scene 				= tokens[tki];
				TestConfig[id,test].increaseSound		= (tokens[tki+4]=="Increase"); 
				TestConfig[id,test].serverUserLeftSide = (tokens[tki+8]=="Left"); 
			}
			
			line = cnfFile.ReadLine();
			id++;
		}

		Debug.Log ("<####> LoadConfigurationFile,"+Time.fixedTime);
		Debug.Log ("<####> DebugRegister,"+id+","+Time.fixedTime);
	}

	void OnGUI()
	{
		Debug.Log ("<####> Application.loadedLevelName,"+Application.loadedLevelName+","+Time.fixedTime);

		if (Application.loadedLevelName != "preload")
			return;

		if(testGui)
		{
			//private string strTestID;
			//private string strError;
			int id;
			if(strTestID.Length == 0)
				strTestID=""+(loadTest.testID+1);
			
			int selGridInt = loadTest.subtest;
			string[] menuGrid  = new string[] {"Pre test", "Test 1", "Test 2","Test 3","Test 4"};
			
			//GUISkin mySkin GUI.skin;
			
			GUI.Label(new Rect(100, 50, 500, 75),strError);
			GUI.Label(new Rect(100, 150, 100, 75),"ID Teste");
			strTestID = GUI.TextField(new Rect(200, 150, 100, 30),strTestID,2);
			GUI.Label(new Rect(100, 250, 100, 75),"Stage Teste");
			loadTest.subtest = GUI.SelectionGrid(new Rect(200, 200, 400, 75),selGridInt, menuGrid, 5);
			
			if(GUI.Button(new Rect(100, 300, 250, 75), "Continue"))
			{
				if(System.Int32.TryParse(strTestID,out id))
				{
					if(id >0 && id<49)
					{
						strTestID= "";
						strError = "";
						loadTest.testID = (id-1);
						testGui = false;
						//GL.Clear(true,true,new Color(0.1f,0.1f,0.1f));
						LoadScene ();
					}
					else						
						strError="ID Teste must be between 1 and 48";
				}
				else
					strError="ID Teste must be integer";
				
			}			
		}
	}
}

