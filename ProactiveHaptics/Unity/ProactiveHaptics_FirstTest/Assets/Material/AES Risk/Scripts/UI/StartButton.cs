using UnityEngine;
using System.Collections;

public class StartButton: MonoBehaviour {

	public GameObject MainMenu;
	//public int selectedLevel;
	public GameObject riskADM;
	public GameObject riskParaRaios;
	public GameObject riskMedicao;
	public GameObject riskSubestacao;
	public MenuInfo MenuInfoObj;

	public GameObject startGameButton;
	public GameObject backToMenuButton;
	
	void OnClick () {

		NGUITools.SetActive (MainMenu,false);

		if (LoadLevel.levelToLoad == 1)
			NGUITools.SetActive (riskADM,true);

		if (LoadLevel.levelToLoad == 2)
			NGUITools.SetActive (riskParaRaios,true);

		if (LoadLevel.levelToLoad == 3)
			NGUITools.SetActive (riskMedicao,true);

		if (LoadLevel.levelToLoad == 4)
			NGUITools.SetActive (riskSubestacao,true);

		startGameButton.SetActive (true);
		backToMenuButton.SetActive (true);
	}
}
