using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {

	public static int levelToLoad = 1;

	public GameObject loadingScreen;
	public UISlider progressBar;

	public void OnClick (){

		StartCoroutine (LoadingScreen());

	}

	public void SetLevelToLoad(UIPopupList list){
		levelToLoad = UIPopupList.current.items.IndexOf (UIPopupList.current.value) + 1;
	}	

	IEnumerator LoadingScreen(){

		loadingScreen.SetActive (true);

		AsyncOperation async = Application.LoadLevelAsync (levelToLoad);

		while(!async.isDone){

			progressBar.value = async.progress;
			Debug.Log (async.progress);

			yield return null;

		}



	}
}