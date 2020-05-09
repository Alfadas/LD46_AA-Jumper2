using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private float maxFlightTime = 6.0f;
	[SerializeField] private float scanAheadDistance = 10.0f;
	[SerializeField] private int damage = 10;
	[SerializeField] private float explosionSize = 8.0f;
	[SerializeField] private float explosionDuration = 0.2f;
	[SerializeField] private Material explosionColor = null;
	private float bulletFired = 0.0f;
	private Vector3 lastPosition = Vector3.zero;
	private float impactTime = -1.0f;
	private Vector3 originalScale = Vector3.one;

	private void Start()
	{
		bulletFired = Time.time;
		lastPosition = transform.position;
	}

	private void Update()
	{
		if(Time.time - bulletFired >= maxFlightTime)
		{
			DestroyBullet();
		}

		if(impactTime >= 0)
		{
			transform.localScale = originalScale * (((Time.time - impactTime) / explosionDuration) * explosionSize);

			if(Time.time - impactTime >= explosionDuration)
			{
				impactTime = -1.0f;
				DestroyBullet();
			}
		}

		if((transform.position - lastPosition).magnitude > scanAheadDistance)
		{
			RaycastHit hit;
			if(Physics.Raycast(lastPosition, transform.forward, out hit, scanAheadDistance) && !hit.collider.isTrigger)
			{
				Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();

				HitCollector target = hit.collider.GetComponent<HitCollector>();
				target?.GetDamage(Mathf.CeilToInt(rigidbody.mass * rigidbody.velocity.magnitude * damage));

				transform.position = hit.point;
				Object.Destroy(rigidbody, 0.0f);
				transform.SetParent(hit.transform, true);

				gameObject.GetComponent<MeshRenderer>().material = explosionColor;
				originalScale = transform.localScale;
				impactTime = Time.time;
			}

			lastPosition = transform.position;
		}
	}

	private void DestroyBullet()
	{
		Object.Destroy(gameObject, 0.2f);
	}
}
