using UnityEngine;
using System.Collections;

public class cube_follow_path : MonoBehaviour {

	public iTweenPath cubepath;
	public float speed;
	private float pathpercentage = 0;

	// Use this for initialization
	void Start () {
		pathpercentage = 0f;
		iTween.PutOnPath(gameObject, iTweenPath.GetPath(cubepath.pathName), pathpercentage);
		//iTween.
	}

	void LateUpdate() {
	}

	// Update is called once per frame
	void Update () {
		if(pathpercentage < 1f)
		{
			pathpercentage += speed * Time.deltaTime;
			iTween.PutOnPath(gameObject, iTweenPath.GetPath(cubepath.pathName), pathpercentage);
		}
	}
}
