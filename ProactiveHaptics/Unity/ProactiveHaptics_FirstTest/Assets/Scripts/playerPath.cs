using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class playerPath : MonoBehaviour {

	public GameObject playerBody;
	public float speed;
	private float pathpercentage = 0;

	Transform[] path;

	// Use this for initialization
	void Start () {
		GameObject[] objects = GameObject.FindGameObjectsWithTag("Path Position").OrderBy( go => go.name ).OrderBy( go => go.name.Length ).ToArray();
		List<Transform> positions = new List<Transform>();
		foreach(GameObject obj in objects)
		{
			positions.Add(obj.transform);
		}
		path = positions.ToArray();

		pathpercentage = 0f;
		iTween.PutOnPath(gameObject, path, pathpercentage);
	}
	
	void LateUpdate() {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey ("w"))
		{
			if(pathpercentage < 1f)
			{
				Vector3 currentPoint = iTween.PointOnPath(path, pathpercentage);
				Vector3 nextPoint = iTween.PointOnPath(path, pathpercentage + 0.01f);

				float distance = Vector3.Distance(currentPoint, nextPoint);
				//Debug.Log (distance);

				pathpercentage += speed * (1f / distance) * Time.deltaTime;
				iTween.PutOnPath(gameObject, path, pathpercentage);
				iTween.LookUpdate(playerBody, nextPoint, 0f);
			}
		}
		else if(Input.GetKey ("s"))
		{
			if(pathpercentage > 0f)
			{
				Vector3 currentPoint = iTween.PointOnPath(path, pathpercentage);
				Vector3 nextPoint = iTween.PointOnPath(path, pathpercentage + 0.01f);
				
				float distance = Vector3.Distance(currentPoint, nextPoint);
				//Debug.Log (distance);
				
				pathpercentage -= speed * (1f / distance) * Time.deltaTime;
				iTween.PutOnPath(gameObject, path, pathpercentage);
				iTween.LookUpdate(playerBody, nextPoint, 0f);
			}
		}
	}
}
