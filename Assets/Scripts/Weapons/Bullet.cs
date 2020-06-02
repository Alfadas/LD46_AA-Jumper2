using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolObject, IPoolManager
{
	[Tooltip("List of Layers this Bullet can collide with")]
	[SerializeField] private LayerMask targetMask = new LayerMask();
	[SerializeField] private int damage = 10;
	[SerializeField] private float fragmentCountModifier = 1.0f;
	[SerializeField] private float fragmentSpeed = 4.0f;
	[SerializeField] private LineRenderer tracer = null;
	[SerializeField] private float tracerLength = 20.0f;
	[SerializeField] private GameObject fragmentPrefab = null;
	[SerializeField] private AudioClip[] hitSounds = null;
	private Vector3 spawnPosition = Vector3.zero;
	private Vector3 lastPosition = Vector3.zero;
	private new SimpleRigidbody rigidbody = null;
	private bool drawTracer = false;
	private bool destroyed = false;
	private Stack<GameObject> fragmentPool = null;

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
	public IPoolManager PoolManager { get; set; } = null;

	private void Awake()
	{
		rigidbody = gameObject.GetComponent<SimpleRigidbody>();
		fragmentPool = new Stack<GameObject>();
		init();
	}

	public void init()
	{
		rigidbody.init();

		spawnPosition = transform.position;
		lastPosition = transform.position;

		if(tracer != null)
		{
			drawTracer = false;
			tracer.SetPosition(0, transform.position);
			tracer.SetPosition(1, transform.position);
		}

		gameObject.SetActive(true);
	}

	private void FixedUpdate()
	{
		if(!destroyed)
		{
			Vector3 travelledSegment = transform.position - lastPosition;
			RaycastHit hit;
			// TODO: Check for Tag here, too
			if(Physics.Raycast(lastPosition, travelledSegment, out hit, travelledSegment.magnitude, targetMask) && !hit.collider.isTrigger)
			{
				// Calculate Damage
				int impactDamage = Mathf.CeilToInt(rigidbody.Mass * rigidbody.Velocity.magnitude * damage * DamageMod);

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

				// Spawn Fragments at Impact Point
				travelledSegment = -travelledSegment.normalized;
				if(impactDamage > 0.0f && fragmentPrefab != null)
				{
					int fragmentCount = Mathf.Max(Mathf.FloorToInt(impactDamage * fragmentCountModifier), 1);
					for(int i = 0; i < fragmentCount; ++i)
					{
						// Spawn Fragment
						GameObject fragment;
						if(fragmentPool.Count > 0)
						{
							fragment = fragmentPool.Pop();
							fragment.transform.position = transform.position;
							fragment.transform.rotation = transform.rotation;
							fragment.GetComponent<SimpleRigidbody>().init();
						}
						else
						{
							fragment = GameObject.Instantiate(fragmentPrefab, transform.position, transform.rotation);
							fragment.GetComponent<SimpleRigidbody>().PoolManager = this;
						}
						fragment.GetComponent<SimpleRigidbody>().Velocity = (travelledSegment + Random.insideUnitSphere) * fragmentSpeed;
					}
				}

				// Destroy Bullet
				if(PoolManager != null)
				{
					gameObject.SetActive(false);
					PoolManager.returnPoolObject(gameObject);
				}
				else
				{
					destroyed = true;
					GameObject.Destroy(gameObject, 0.02f);
				}
			}

			lastPosition = transform.position;

			// Make sure, that Tracer Ends never appear inside or behind the Weapon
			if(tracer != null && (drawTracer || (transform.position - spawnPosition).sqrMagnitude > (rigidbody.Velocity.sqrMagnitude * (tracerLength * tracerLength) * (Time.deltaTime * Time.deltaTime))))
			{
				tracer.SetPosition(0, transform.position);
				tracer.SetPosition(1, transform.position - (rigidbody.Velocity * tracerLength * Time.deltaTime));
				drawTracer = true;
			}
		}
	}

	public void returnPoolObject(GameObject poolObject)
	{
		fragmentPool.Push(poolObject);
	}
}
