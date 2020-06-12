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
	[SerializeField] private float movementSpeed = 20.0f;
	[Tooltip("Factor by which Sprinting is faster than walking")]
	[SerializeField] private float sprintFactor = 2.0f;
	[SerializeField] private float jumpStrength = 40.0f;
	[Tooltip("Minimum Time between 2 Jump Attempts")]
	[SerializeField] private float jumpTime = 0.2f;
	[Tooltip("Movement Speed Modifier when the Player is neither grounded nor grappled")]
	[SerializeField] private float floatingMovementFactor = 0.002f;
	[Tooltip("Movement Speed Modifier when the Player is grappled, but not grounded")]
	[SerializeField] private float grappledMovementFactor = 0.5f;
	[SerializeField] private GameObject head = null;
	[SerializeField] private GrapplingHook grapplingHook = null;
	[SerializeField] private Collider feet = null;
	private new Rigidbody rigidbody = null;
	private Vector3 movement = Vector3.zero;
	private List<ContactPoint> contactList = null;
	private bool grounded = false;
	private float lastJump = 0.0f;
	private float jumpCharge = 0.0f;
	private bool mouseVisible = false;

	private void Start()
	{
		rigidbody = gameObject.GetComponent<Rigidbody>();
		contactList = new List<ContactPoint>(64);
	}

	private void Update()
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

		// Movement
		movement = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
		if(movement.sqrMagnitude > 1)
		{
			movement = Vector3.Normalize(movement);
		}
		movement *= movementSpeed * Time.deltaTime;
		if(Input.GetButton("Sprint") && Vector3.Angle(transform.forward, movement) <= 45.0f)
		{
			movement *= sprintFactor;
		}
		// Apply Movement
		rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, new Vector3(movement.x, rigidbody.velocity.y, movement.z), grounded ?
			1.0f : ((grapplingHook != null && grapplingHook.Hooked) ? grappledMovementFactor : floatingMovementFactor));

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
					rigidbody.AddForce(Vector3.up * jumpStrength * Time.deltaTime, ForceMode.Impulse);
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
			for(int i = 0; i < contactCount; ++i)
			{
				if(!contactList[i].otherCollider.gameObject.Equals(gameObject))
				{
					if(contactList[i].thisCollider.Equals(feet))
					{
						grounded = true;
						break;
					}
				}
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		grounded = false;
	}

	public void setMouseVisible(bool mouseVisible)
	{
		this.mouseVisible = mouseVisible;
	}
}

