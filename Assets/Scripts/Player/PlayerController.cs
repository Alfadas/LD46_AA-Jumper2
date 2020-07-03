using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Tooltip("Maximum look down Angle, Front is 0/360 Degrees, straight down 90/360 Degrees")]
	[SerializeField] private float maxLookDown = 90.0f;
	[Tooltip("Maximum look up Angle, Front is 0/360 Degrees, straight up 270/360 Degrees")]
	[SerializeField] private float maxLookUp = 270.0f;
	[SerializeField] private float rotationSpeed = 400.0f;
	[SerializeField] private float movementSpeed = 10.0f;
	[SerializeField] private float movementAcceleration = 20.0f;
	[Tooltip("Factor by which Sprinting is faster than Walking")]
	[SerializeField] private float sprintFactor = 2.0f;
	[SerializeField] private float jumpStrength = 40.0f;
	[Tooltip("Minimum Time between 2 Jump Attempts")]
	[SerializeField] private float jumpTime = 0.2f;
	[Tooltip("Movement Speed Modifier when the Player is neither grounded nor grappled")]
	[SerializeField] private float floatingMovementFactor = 0.002f;
	[Tooltip("Movement Speed Modifier when the Player is grappled, but not grounded")]
	[SerializeField] private float grappledMovementFactor = 0.5f;
	[Tooltip("The maximum Acceleration, that can be applied by standing on another moving Rigidbody")]
	[SerializeField] private float tractionAcceleration = 20.0f;
	[SerializeField] private GameObject head = null;
	[SerializeField] private GrapplingHook grapplingHook = null;
	[SerializeField] private Collider feet = null;
	private new Rigidbody rigidbody = null;
	private Rigidbody parentRigidbody = null;
	private List<ContactPoint> contactList = null;
	private bool grounded = false;
	private float lastJump = 0.0f;
	private float jumpCharge = 0.0f;
	private bool mouseVisible = false;

	private void Start()
	{
		rigidbody = gameObject.GetComponent<Rigidbody>();
		contactList = new List<ContactPoint>(64);

		Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
		foreach(Collider collider1 in colliders)
		{
			foreach(Collider collider2 in colliders)
			{
				if(collider1 != collider2)
				{
					Physics.IgnoreCollision(collider1, collider2);
				}
			}
		}
	}

	// Remember: Instantanious Input like GetKeyDown is processed by the first Update() after Key-Press, so it could be missed in FixedUpdate().
	//		Therefore only use GetAxis() and GetButton() in FixedUpdate() and buffer everything else in an Update() Call. 
	private void FixedUpdate()
	{
		if(!mouseVisible)
		{
			Cursor.lockState = CursorLockMode.Locked;

			// Rotation
			Vector3 rotation = transform.rotation.eulerAngles;

			if(head != null)
			{
				rotation.x = head.transform.rotation.eulerAngles.x;
			}

			rotation.x += -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
			rotation.y += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
			rotation.z = 0.0f;

			if(rotation.x < 180 && rotation.x > maxLookDown)
			{
				rotation.x = maxLookDown;
			}
			else if(rotation.x > 180 && rotation.x < maxLookUp)
			{
				rotation.x = maxLookUp;
			}

			if(head != null)
			{
				head.transform.localRotation = Quaternion.Euler(new Vector3(rotation.x, 0.0f, 0.0f));
				transform.localRotation = Quaternion.Euler(new Vector3(0.0f, rotation.y, 0.0f));
			}
			else
			{
				transform.localRotation = Quaternion.Euler(rotation);
			}
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
		}

		// Calculate Movement
		Vector3 direction = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
		Vector3 tractionVelocityChange = Vector3.zero;
		Vector3 movementVelocityChange = Vector3.zero;
		if(direction != Vector3.zero || (grounded && parentRigidbody == null))
		{
			// Cap Direction Length to 1
			if(direction.sqrMagnitude > 1.0f)
			{
				direction.Normalize();
			}

			// Calculate Speed
			float speed = movementSpeed;
			// Sprint Bonus
			if(Input.GetButton("Sprint") && Vector3.Angle(transform.forward, direction) <= 45.0f)
			{
				speed *= sprintFactor;
			}
			// Avoid unwanted Deceleration
			// TODO: Could get problematic if slow Effects should trigger in Motion
			float sqrRigidbodySpeed = rigidbody.velocity.sqrMagnitude;
			if(sqrRigidbodySpeed > (speed * speed) && Vector3.Angle(rigidbody.velocity, direction) <= 45.0f)
			{
				speed = Mathf.Sqrt(sqrRigidbodySpeed);
			}

			// Calculate Acceleration
			float acceleration = movementAcceleration;
			// Ground and Grappling Bonus
			if(!grounded)
			{
				if(grapplingHook != null && grapplingHook.Hooked)
				{
					acceleration *= grappledMovementFactor;
				}
				else
				{
					acceleration *= floatingMovementFactor;
				}
			}

			// Calculate Acceleration
			movementVelocityChange = calculateAcceleration(((direction * speed) - rigidbody.velocity), acceleration);
		}
		// Calculate Traction
		if(parentRigidbody != null)
		{
			tractionVelocityChange = calculateAcceleration(parentRigidbody.velocity - rigidbody.velocity, tractionAcceleration);
		}
		//Apply Movement
		rigidbody.AddForce(movementVelocityChange + tractionVelocityChange, ForceMode.VelocityChange);

		// Jumping
		if(Input.GetButton("Jump"))
		{
			if((Time.time - lastJump) >= jumpTime && grounded)
			{
				lastJump = Time.time;
				jumpCharge = 0.0f;
				rigidbody.AddForce(Vector3.up * jumpStrength * Time.deltaTime, ForceMode.Impulse);
			}
			else
			{
				if(jumpCharge < jumpTime)
				{
					jumpCharge += Time.deltaTime;
					rigidbody.AddForce(transform.up * jumpStrength * Time.deltaTime, ForceMode.Impulse);
				}
			}
		}
		else
		{
			jumpCharge = jumpTime;
		}
	}

	// Only get grounded, when you stay longer than 1 Frame on a Collider
	private void OnCollisionStay(Collision collision)
	{
		if(!grounded)
		{
			int contactCount = collision.GetContacts(contactList);
			float maxMass = 0.0f;
			for(int i = 0; i < contactCount; ++i)
			{
				if(contactList[i].thisCollider.Equals(feet))
				{
					grounded = true;
					if((parentRigidbody = contactList[i].otherCollider.attachedRigidbody) != null && parentRigidbody.mass > maxMass)
					{
						maxMass = parentRigidbody.mass;
					}
				}
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		grounded = false;
		if(collision.rigidbody == parentRigidbody)
		{
			parentRigidbody = null;
		}
	}

	private Vector3 calculateAcceleration(Vector3 targetVelocity, float acceleration)
	{
		float changeSqrMagnitude = targetVelocity.sqrMagnitude;
		if(changeSqrMagnitude > Mathf.Pow(acceleration * Time.deltaTime, 2.0f))
		{
			targetVelocity = targetVelocity.normalized * acceleration * Time.deltaTime;
		}

		return targetVelocity;
	}

	// TODO: Change to Property
	public void setMouseVisible(bool mouseVisible)
	{
		this.mouseVisible = mouseVisible;
	}
}

