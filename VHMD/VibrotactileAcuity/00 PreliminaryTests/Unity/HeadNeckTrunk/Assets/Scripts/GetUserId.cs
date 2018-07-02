using UnityEngine;
using System.Collections;

public class GetUserId : MonoBehaviour {   
	
	void Start (){
		PlayerPrefs.SetInt("Play", 0);
	}
	
	void Update() {
		string text = "";
		int id = 0;
		//print ("update");
		foreach(char c in Input.inputString) {
			if(c == "\b"[0]){
				EmEdicao();
				if (guiText.text.Length != 0) guiText.text = guiText.text.Substring(0, guiText.text.Length - 1);
			} else{
				if(c == "\n"[0] || c == "\r"[0]){
					try{
						text = guiText.text;
						string[] playerid = text.Split(':');					
						id = int.Parse(playerid[1]);
						if(id > 0){
							SetModifierMode(id);
							SetLangMode();
							guiText.material.color = Color.green;
							PlayerPrefs.SetInt("Play", 1);
							PlayerPrefs.SetString("User", playerid[0]);
							PlayerPrefs.SetInt("Id", id);
							PlayerPrefs.Save();
						}
					}catch{
						guiText.material.color = Color.red;
						PlayerPrefs.SetInt("Play", 0);
					}					
				} else{
					EmEdicao();
					guiText.text += c;					
				}
			}
		}	
	}
	
	void EmEdicao(){
		guiText.material.color = Color.white;
		PlayerPrefs.SetInt("Play", 0);
	}
	
	void SetModifierMode(int userId){
		if((userId < 5) || (userId > 9 && userId < 15) || (userId > 19 && userId < 25) 
		   || (userId > 29 && userId < 35) || (userId > 39 && userId < 45) || (userId > 49 && userId < 55)){
			// Sem modificador
			PlayerPrefs.SetInt("Modifier", 0);
		}else{
			// Com modificador
			PlayerPrefs.SetInt("Modifier", 1);
		}
	}
	
	void SetLangMode(){
		PlayerPrefs.SetInt("d", 0);
		PlayerPrefs.SetInt("f", 0);
		PlayerPrefs.SetInt("i", 0);
	}
}