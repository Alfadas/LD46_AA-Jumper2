using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Fragment : MonoBehaviour
{
	[SerializeField] private float drag = 0.2f;
	[SerializeField] private float lifetime = 4.0f;

	private Vector3 startScale = Vector3.one;
	private float spawnTime = 0.0f;

	public Vector3 Velocity { get; set; } = Vector3.zero;

	private void Start()
	{
		spawnTime = Time.time;
	}

	private void FixedUpdate()
	{
		float age = Time.time - spawnTime;
		if(age < lifetime)
		{
			// Update Position
			transform.position += Velocity * Time.deltaTime;

			// Apply Drag
			Velocity *= 1 - (drag  * Time.deltaTime);

			// Apply Gravity
			Velocity += new Vector3(0.0f, -9.81f * Time.deltaTime, 0.0f);
		}
		else
		{
			Object.Destroy(gameObject, 0.2f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Velocity = (transform.position - other.transform.position).normalized * Velocity.magnitude * 0.6f;
	}
}
