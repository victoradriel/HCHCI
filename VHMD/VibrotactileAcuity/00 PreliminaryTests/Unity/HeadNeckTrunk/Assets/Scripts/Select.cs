using UnityEngine;
using System.Collections;

public class Select : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		if(this.guiText.material.color == Color.green){
			EnableAll();
			EsconderAvatar();
		}else{
			DesableAll();
			this.guiText.material.color = Color.green;
			
			DesativaTodos();
			
			switch(this.guiText.name){
				case "OpCurso":
					EsconderAvatar();
					MostrarAvatar("Fluxo");
					break;
				case "OpIntin":
					EsconderAvatar();
					MostrarAvatar("Intinerario");
					break;
				case "OpDestino":
				case "OpObst":
					EsconderAvatar();
					MostrarAvatar("Posicao");
					break;
				case "OpAlerta":
				case "OpNon":
					EsconderAvatar();
					break;
			}
		}
    }
	
	public static void DesableAll(){
		string[] opcoes = {"OpAlerta", "OpCurso", "OpDestino", "OpIntin", "OpObst", "OpNon"};
		GameObject opcao;
		
		for (int i=0; i < 6; i++) {
			opcao = GameObject.Find(opcoes[i]);
			opcao.guiText.material.color = Color.gray;
		}
	} 
	
	public static void EnableAll(){
		string[] opcoes = {"OpAlerta", "OpCurso", "OpDestino", "OpIntin", "OpObst", "OpNon"};
		GameObject opcao;
		
		for (int i=0; i < 6; i++) {
			opcao = GameObject.Find(opcoes[i]);
			opcao.guiText.material.color = Color.white;
		}
	}
	
	public static void EsconderAvatar(){
		GameObject plane = GameObject.Find("Plane");
		GameObject setas = GameObject.Find("Setas");
		GameObject rosaVentos = GameObject.Find("RosaVentos");
		GameObject course = GameObject.Find("Fluxo");
		GameObject personagem = GameObject.Find("Personagem");
		GameObject title = GameObject.Find("Posicao");
		
		EsconderMotores();
		plane.renderer.enabled = false;
		personagem.renderer.enabled = false;
		title.guiText.enabled = false;
		setas.renderer.enabled = false;
		course.renderer.enabled = false;
		rosaVentos.renderer.enabled = false;
	}
	
	public static void EsconderMotores(){
		string[] motors = {"Sphere1", "Sphere2", "Sphere3", "Sphere4", "Sphere5", "Sphere6", "Sphere7", "Sphere8"};
		string[] lines = {"Cylinder1", "Cylinder2", "Cylinder3", "Cylinder4", "Cylinder5", "Cylinder6", "Cylinder7", "Cylinder8"};
		
		for(int i=0;i<8;i++){
			GameObject esfera = GameObject.Find(motors[i]);
			esfera.renderer.enabled = false;
			GameObject line = GameObject.Find(lines[i]);
			line.renderer.enabled = false;
		}
	}
	
	public static void DesativaTodos() {
		string[] motors = {"Sphere1", "Sphere2", "Sphere3", "Sphere4", "Sphere5", "Sphere6", "Sphere7", "Sphere8"};
		for(int i=0;i<8;i++){
			GameObject esfera = GameObject.Find(motors[i]);
			esfera.renderer.material.color = Color.white;
		}
	}
	
	public static void MostrarAvatar(string opcao){
		GameObject plane = GameObject.Find("Plane");
		GameObject personagem = GameObject.Find("Personagem");
		GameObject title = GameObject.Find("Posicao");
		string[] motors = {"Sphere1", "Sphere2", "Sphere3", "Sphere4", "Sphere5", "Sphere6", "Sphere7", "Sphere8"};
		string[] lines = {"Cylinder1", "Cylinder2", "Cylinder3", "Cylinder4", "Cylinder5", "Cylinder6", "Cylinder7", "Cylinder8"};
		GameObject setas = GameObject.Find("Setas");
		GameObject rose = GameObject.Find("RosaVentos");
		GameObject course = GameObject.Find("Fluxo");
		
		GameObject esfera;
		GameObject line;
		
		plane.renderer.enabled = true;
		personagem.renderer.enabled = true;
		title.guiText.enabled = true;
		
		switch(opcao){
			case "Intinerario":
				for(int i=0;i<8;i+=2){
					esfera = GameObject.Find(motors[i]);
					esfera.renderer.enabled = true;
					line = GameObject.Find(lines[i]);
					line.renderer.enabled = true;
				}				
				setas.renderer.enabled = true;
				title.guiText.text = "Direção";
				break;
			
			case "Fluxo":
				for(int i=3;i<6;i+=2){
					esfera = GameObject.Find(motors[i]);
					esfera.renderer.enabled = true;
					line = GameObject.Find(lines[i]);
					line.renderer.enabled = true;
				}				
				course.renderer.enabled = true;
				title.guiText.text = "Posição";
				break;
			
			case "Posicao":
				for(int i=0;i<8;i++){
					esfera = GameObject.Find(motors[i]);
					esfera.renderer.enabled = true;
					line = GameObject.Find(lines[i]);
					line.renderer.enabled = true;
				}				
				rose.renderer.enabled = true;
				title.guiText.text = "Posição";
				break;
		}
	}
}
