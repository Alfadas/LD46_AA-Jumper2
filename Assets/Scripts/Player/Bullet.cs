using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private float maxFlightTime = 6.0f;
	[SerializeField] private float scanAheadDistance = 10.0f;
	[SerializeField] private int damage = 10;
	[SerializeField] private float fragmentCountModifier = 1.0f;
	[SerializeField] private float fragmentSpeed = 4.0f;
	[SerializeField] private GameObject fragmentPrefab = null;
	private float bulletFired = 0.0f;
	private Vector3 lastPosition = Vector3.zero;
	private bool destroyed = false;

	public int Damage
	{
		get
		{
			return damage;
		}

		private set
		{
			damage = value;
		}
	}
	public float DamageMod { get; set; } = 1.0f;

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

		if(!destroyed && (transform.position - lastPosition).magnitude > scanAheadDistance)
		{
			RaycastHit hit;
			if(Physics.Raycast(lastPosition, transform.forward, out hit, scanAheadDistance) && !hit.collider.isTrigger)
			{
				Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();

				int impactDamage = Mathf.CeilToInt(rigidbody.mass * rigidbody.velocity.magnitude * damage * DamageMod);

				AirshipHitCollector target = hit.collider.GetComponent<AirshipHitCollector>();
				target?.GetDamage(impactDamage);

				transform.position = hit.point;

				DestroyBullet(impactDamage);
			}

			lastPosition = transform.position;
		}
	}

	private void DestroyBullet(float fragmentationDamage = 0.0f)
	{
		destroyed = true;

		if(fragmentationDamage > 0.0f)
		{
			int fragmentCount = Mathf.Max(Mathf.FloorToInt(fragmentationDamage * fragmentCountModifier), 1);
			for(int i = 0; i < fragmentCount; ++i)
			{
				// Spawn Fragment
				GameObject fragment = Object.Instantiate(fragmentPrefab, transform.position, transform.rotation);
				// Give Fragment a random Velocity in general Back-Direction of the Bullet
				fragment.GetComponent<Fragment>().Velocity = -transform.forward + Random.insideUnitSphere * fragmentSpeed;
			}
		}

		Object.Destroy(gameObject, 0.2f);
	}
}
