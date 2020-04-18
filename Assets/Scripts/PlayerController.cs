using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
	{
	[SerializeField] private float movementSpeed = 0.2f;
	[SerializeField] private float rotationSpeed = 1.0f;
	[Tooltip("Maximum look down Angle, Front is 0/360 Degrees, straight down 90/360 Degrees")]
	[SerializeField] private float maxLookDown = 90.0f;
	[Tooltip("Maximum look up Angle, Front is 0/360 Degrees, straight up 270/360 Degrees")]
	[SerializeField] private float maxLookUp = 270.0f;
	[SerializeField] private float jumpStrength = 1.0f;
	[Tooltip("The Height of the GameObjects Pivot Point above the Ground")]
	[SerializeField] private float distanceToGround = 0.0f;
	[SerializeField] private GameObject head = null;
	private bool grounded = true;
	private Rigidbody rigidbody = null;
	private Vector3 movement = Vector3.zero;

	private void Start()
		{
		rigidbody = gameObject.GetComponent<Rigidbody>();
		}

	private void FixedUpdate()
		{
		Cursor.lockState = CursorLockMode.Locked;

		// Rotation
		Vector3 rotation = transform.rotation.eulerAngles;

		if(head != null)
			{
			rotation.x = head.transform.rotation.eulerAngles.x;
			}

		rotation.x += -Input.GetAxis("Mouse Y") * rotationSpeed;
		rotation.y += Input.GetAxis("Mouse X") * rotationSpeed;
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
			head.transform.rotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y, 0.0f));
			transform.rotation = Quaternion.Euler(new Vector3(0.0f, rotation.y, 0.0f));
			}
		else
			{
			transform.rotation = Quaternion.Euler(rotation);
			}

		// Movement and jumping is only possible when having Ground Contact, else the last Input is applied again
		if(grounded)
			{
			// Movement
			movement = transform.rotation * new Vector3(Input.GetAxis("Horizontal") * movementSpeed, 0.0f, Input.GetAxis("Vertical") * movementSpeed);
			
			// Jumping
			if(Input.GetAxis("Jump") > 0)
				{
				rigidbody.AddForce(transform.up * jumpStrength, ForceMode.Impulse);
				}
			}
		else
			{
			// Jump Movement
			movement *= 1.0f - rigidbody.drag;
			}

		transform.Translate(movement, Space.World);
		}

	private void OnCollisionStay(Collision collision)
		{
		grounded = isGrounded();
		}

	private void OnCollisionExit(Collision collision)
		{
		grounded = isGrounded();
		}

	private bool isGrounded()
		{
		Debug.Log(Physics.Raycast(transform.position + (Vector3.up * (distanceToGround + 0.2f)), Vector3.down, distanceToGround + 0.4f));
		return Physics.Raycast(transform.position + (Vector3.up * (distanceToGround + 0.2f)), Vector3.down, distanceToGround + 0.4f);
		}
	}
