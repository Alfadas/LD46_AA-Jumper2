using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimpleRigidbody : MonoBehaviour
{
	[SerializeField] private float mass = 1.0f;
	[SerializeField] private float gravity = 9.81f;
	[SerializeField] private float drag = 0.2f;
	[Tooltip("How much Velocity is retained when this Object hits another Collider and the Threshold, at which it will just pass through")]
	[SerializeField] private float bounciness = 0.5f;
	[Tooltip("Time until from Spawn until automatic Destruction of this Object, 0 means unlimited")]
	[SerializeField] private float lifetime = 0.0f;

	private float spawnTime = 0.0f;

	public float Mass
	{
		get
		{
			return mass;
		}
		set
		{
			mass = value;
		}
	}
	public Vector3 Velocity { get; set; } = Vector3.zero;

	private void Start()
	{
		spawnTime = Time.time;
	}

	private void FixedUpdate()
	{
		if(lifetime <= 0 || (Time.time - spawnTime < lifetime))
		{
			// Update Position
			transform.position += Velocity * Time.deltaTime;

			// Apply Drag
			Velocity *= 1.0f - Mathf.Min((Velocity.sqrMagnitude * drag * Time.deltaTime), 1.0f);

			// Apply Gravity
			Velocity += new Vector3(0.0f, -gravity * Time.deltaTime, 0.0f);
		}
		else
		{
			Object.Destroy(gameObject, 0.02f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(Velocity.sqrMagnitude >= bounciness && (Time.time - spawnTime) > 0.2f)
		{
			Velocity = -Velocity * bounciness;
		}
	}

	public void applyImpulse(Vector3 impulse)
	{
		Velocity += impulse / mass;
	}
}
