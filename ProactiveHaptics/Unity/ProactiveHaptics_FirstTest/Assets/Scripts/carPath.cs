using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class carPath : MonoBehaviour {

	public float speed;
	private float pathpercentage = 0;

	bool moving = false;
	int actorCount = 0;

	public List<GameObject> turningTires;
	public List<GameObject> tires;

	Transform[] path;

	void StartMoving()
	{
		moving = true;

		/*foreach(GameObject tire in tires)
		{
			tire.GetComponent<Animator>().SetBool ("Carro Andando", true);
		}*/
	}

	void StopMoving()
	{
		moving = false;

		/*foreach(GameObject tire in tires)
		{
			tire.GetComponent<Animator>().SetBool ("Carro Andando", false);
		}*/
	}

	// Use this for initialization
	void Start () {
		GameObject[] objects = GameObject.FindGameObjectsWithTag("Car Path Position").OrderBy( go => go.name ).OrderBy( go => go.name.Length ).ToArray();
		List<Transform> positions = new List<Transform>();
		foreach(GameObject obj in objects)
		{
			positions.Add(obj.transform);
		}
		path = positions.ToArray();

		pathpercentage = 0f;
		iTween.PutOnPath(gameObject, path, pathpercentage);

		actorCount = 0;
		StartMoving ();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			actorCount ++;
			StopMoving ();
		}
		else
		{
			if(other.gameObject.GetComponent<NPCWalkingStreet>() != null)
			{
				actorCount ++;
				StopMoving ();
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		bool actorExited = false;

		if(other.gameObject.tag == "Player")
		{
			actorExited = true;
		}
		else
		{
			if(other.gameObject.GetComponent<NPCWalkingStreet>() != null)
			{
				actorExited = true;
			}
		}

		if(actorExited)
		{
			actorCount --;

			if(actorCount == 0)
			{
				StartMoving ();
			}
		}
	}

	void Update () 
	{
		if(moving)
		{
			if(pathpercentage < 1f)
			{
				Vector3 currentPoint = iTween.PointOnPath(path, pathpercentage);
				Vector3 nextPoint = iTween.PointOnPath(path, pathpercentage + 0.01f);

				float distance = Vector3.Distance (currentPoint, nextPoint);
				
				pathpercentage += speed * Time.deltaTime;
				iTween.PutOnPath(gameObject, path, pathpercentage);

				float initialAngle = transform.rotation.y;
				iTween.LookUpdate(gameObject, nextPoint, 0f);
				transform.Rotate(Vector3.up, -90f);
				float finalAngle = transform.rotation.y;

				float tireRotation = finalAngle - initialAngle;

				foreach(GameObject tire in tires)
				{
					tire.transform.Rotate(new Vector3(7.5f * distance, 0f, 0f));
					Debug.Log (tire.transform.localRotation);
				}

				foreach(GameObject tire in turningTires)
				{
					Quaternion tireRot = tire.transform.localRotation;
					Quaternion tireNextRot = new Quaternion(tireRot.x, tireRotation + 90f, tireRot.z, tireRot.w);
				}
			}
			else
			{
				pathpercentage = 0f;
				iTween.PutOnPath(gameObject, path, pathpercentage);
			}
		}
		else
		{
		}
	}
}
