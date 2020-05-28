using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private int damage = 10;
	[SerializeField] private float fragmentCountModifier = 1.0f;
	[SerializeField] private float fragmentSpeed = 4.0f;
	[SerializeField] private LineRenderer tracer = null;
	[SerializeField] private float tracerDuration = 0.4f;
	[SerializeField] private GameObject fragmentPrefab = null;
	[SerializeField] private AudioClip[] hitSounds = null;
	private float bulletFired = 0.0f;
	private Vector3 lastPosition = Vector3.zero;
	private Vector3 lastTravelledSegment = Vector3.zero;
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

		if(tracer != null)
		{
			tracer.SetPosition(0, transform.position);
			tracer.SetPosition(1, transform.position);
			StartCoroutine(removeTracer());
		}
	}

	private void FixedUpdate()
	{
		if(!destroyed)
		{
			lastTravelledSegment = transform.position - lastPosition;
			RaycastHit hit;
			// TODO: Check for Tag here, too
			if(Physics.Raycast(lastPosition, lastTravelledSegment, out hit, lastTravelledSegment.magnitude) && !hit.collider.isTrigger)
			{
				// Retrieve Rigidbody
				SimpleRigidbody rigidbody = gameObject.GetComponent<SimpleRigidbody>();

				// Calculate Damage
				int impactDamage = Mathf.CeilToInt(rigidbody.Mass * rigidbody.Velocity.magnitude * damage * DamageMod);

				// Delete Rigidbody
				Object.Destroy(rigidbody);

				// Apply Damage
				Hittable target = hit.collider.GetComponent<Hittable>();
				if(target != null)
				{
					target.GetDamage(impactDamage);
				}

				// Play Hit Sound
				AudioSource audioSource = hit.collider.GetComponent<AudioSource>();
				if(audioSource != null && hitSounds != null && hitSounds.Length > 0)
				{
					audioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length - 1)]);
				}

				// Change Bullet Position to Impact Point
				transform.position = hit.point;

				// Destroy Bullets and spawn Fragments at Impact Point
				DestroyBullet(impactDamage, -lastTravelledSegment.normalized);
			}

			if(tracer != null)
			{
				tracer.SetPosition(1, transform.position);
			}

			lastPosition = transform.position;
		}
	}

	private void DestroyBullet(float fragmentationDamage = 0.0f, Vector3? fragmentationDirection = null)
	{
		destroyed = true;

		if(fragmentationDamage > 0.0f && fragmentPrefab != null)
		{
			if(fragmentationDirection == null)
			{
				fragmentationDirection = -transform.forward;
			}

			int fragmentCount = Mathf.Max(Mathf.FloorToInt(fragmentationDamage * fragmentCountModifier), 1);
			for(int i = 0; i < fragmentCount; ++i)
			{
				// Spawn Fragment
				GameObject fragment = Object.Instantiate(fragmentPrefab, transform.position, transform.rotation);
				fragment.GetComponent<SimpleRigidbody>().Velocity = (((Vector3) fragmentationDirection) + Random.insideUnitSphere) * fragmentSpeed;
			}
		}

		Object.Destroy(gameObject, 0.2f);
	}

	private IEnumerator removeTracer()
	{
		while(!destroyed)
		{
			yield return new WaitForSeconds(tracerDuration);
			tracer.SetPosition(0, tracer.GetPosition(0) + lastTravelledSegment);
		}

		if(destroyed)
		{
			tracer.SetPosition(0, Vector3.zero);
			tracer.SetPosition(1, Vector3.zero);
		}
	}
}
