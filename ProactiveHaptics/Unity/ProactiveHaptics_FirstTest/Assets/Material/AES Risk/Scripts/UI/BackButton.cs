using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {

	public GameObject riskADM;
	public GameObject riskParaRaios;
	public GameObject riskMedicao;
	public GameObject riskSubestacao;

	public GameObject MainMenu;
	
	void OnClick () {

		if (LoadLevel.levelToLoad == 1)
			NGUITools.SetActive (riskADM,false);
		
		if (LoadLevel.levelToLoad == 2)
			NGUITools.SetActive (riskParaRaios,false);
		
		if (LoadLevel.levelToLoad == 3)
			NGUITools.SetActive (riskMedicao,false);
		
		if (LoadLevel.levelToLoad == 4)
			NGUITools.SetActive (riskSubestacao,false);
		
		NGUITools.SetActive (MainMenu,true);
	
	}
}