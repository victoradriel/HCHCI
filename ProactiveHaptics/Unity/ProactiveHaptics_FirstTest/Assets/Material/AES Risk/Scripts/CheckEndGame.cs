using UnityEngine;
using System.Collections;

public class CheckEndGame : MonoBehaviour {
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Q)) {
			MenuInfo info = GameObject.FindGameObjectWithTag("menuInfo").GetComponent<MenuInfo>();
			info.copyRisksInfo();

			// Loads the current level's report screen
			if(Application.loadedLevelName == "adm_caminho1")
			{
				Application.LoadLevel("Performance-Administrativo");
			}
			else if(Application.loadedLevelName == "subestacao_trajeto")
			{
				Application.LoadLevel("Performance-Subestacao");
			}
			else if(Application.loadedLevelName == "isolador_trajeto")
			{
				Application.LoadLevel("Performance-ParaRaios");
			}
			else if(Application.loadedLevelName == "leitura_entrega")
			{
				Application.LoadLevel("Performance-LeituraEntrega");
			}
			
		}
	}
}
