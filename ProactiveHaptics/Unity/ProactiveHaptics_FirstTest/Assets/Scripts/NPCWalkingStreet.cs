using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NPCWalkingStreet : MonoBehaviour {

	UnityEngine.AI.NavMeshAgent navAgent;
	Animator animator;
	int lastTarget = 100;
	
	IEnumerator RoamRandomly()
	{
		while( true )
		{
			FindNextTarget ();
			animator.SetBool("Caminhando", true);
			yield return StartCoroutine(ReachTarget ());
			animator.SetBool ("Caminhando", false);
			yield return StartCoroutine(NPCWaitIdle(6f));
		}
	}

	IEnumerator NPCWaitIdle(float time)
	{
		float waitedTime = 0f;
		while(waitedTime < time)
		{
			waitedTime += 1f * Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator ReachTarget()
	{
		while(!(navAgent.hasPath && navAgent.remainingDistance <= navAgent.stoppingDistance + 0.1f))
		{
			yield return null;
		}
	}

	void FindNextTarget()
	{
		GameObject[] randomDests = GameObject.FindGameObjectsWithTag("Path Position");

		int targetIndex = lastTarget;
		while(targetIndex == lastTarget)
		{
			targetIndex = Random.Range(0, randomDests.Length - 1);
			if(randomDests.Length <= 1)
				break;
		}
		if(targetIndex != lastTarget) navAgent.SetDestination (randomDests[targetIndex].transform.position);

		lastTarget = targetIndex;
	}

	// Use this for initialization
	void Start () 
	{
		navAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
		animator = this.gameObject.GetComponent<Animator>();

		StartCoroutine (RoamRandomly ());
	}
}
