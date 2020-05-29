using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private int damage = 10;
	[SerializeField] private float fragmentCountModifier = 1.0f;
	[SerializeField] private float fragmentSpeed = 4.0f;
	[SerializeField] private LineRenderer tracer = null;
	[SerializeField] private float tracerLength = 20.0f;
	[SerializeField] private GameObject fragmentPrefab = null;
	[SerializeField] private AudioClip[] hitSounds = null;
	private float bulletFired = 0.0f;
	private Vector3 lastPosition = Vector3.zero;
	private Vector3 lastTravelledSegment = Vector3.zero;
	private SimpleRigidbody rigidbody = null;
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
			rigidbody = gameObject.GetComponent<SimpleRigidbody>();

			tracer.SetPosition(0, transform.position);
			tracer.SetPosition(1, transform.position);
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

			lastPosition = transform.position;
		}
	}

	private void Update()
	{
		if(tracer != null && (Time.time - bulletFired) > 0.02f)		// Small Delay to prevent Tracers being visible in the Weapon and to conceal, that they are way too big for the Weapon
		{
			tracer.SetPosition(0, transform.position);
			tracer.SetPosition(1, transform.position - (-rigidbody.Velocity * tracerLength * Time.deltaTime));
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
				fragment.GetComponent<SimpleRigidbody>().Velocity = (fragmentationDirection.Value + Random.insideUnitSphere) * fragmentSpeed;
			}
		}

		Object.Destroy(gameObject, 0.02f);
	}
}
