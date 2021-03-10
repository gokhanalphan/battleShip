using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testMove : MonoBehaviour
{
	public float moveSpeed = 10f;
	Transform _transform;
	bool movementInProgress = false;
	float distanceThreshold = 0.01f;

	void Awake()
	{
		_transform = GetComponent<Transform>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0) && !movementInProgress)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				if (hit.collider.tag == "Tile")
				{
					StartCoroutine(RectMove(hit.point));
				}
			}
		}
	}

	IEnumerator RectMove(Vector3 destination)
	{
		// this will be true in the following Updates, until this whole coroutine finishes
		movementInProgress = true;

		// 0. the movement vector
		Vector3 moveVector = destination - _transform.position;
		Vector3 target;

		// 1. move on X
		target = _transform.position + new Vector3(moveVector.x, 0, 0);
		yield return StartCoroutine(MoveTo(target));

		// 2. when finished, move on Z
		target = _transform.position + new Vector3(0, 0, moveVector.z);
		yield return StartCoroutine(MoveTo(target));

		movementInProgress = false;
	}

	IEnumerator MoveTo(Vector3 destination)
	{
		while (true)
		{
			_transform.position = Vector3.MoveTowards(_transform.position, destination,
													  moveSpeed * Time.deltaTime);
			if (Vector3.Distance(_transform.position, destination) < distanceThreshold)
				yield break;
			else
				yield return null;
		}
	}
}
