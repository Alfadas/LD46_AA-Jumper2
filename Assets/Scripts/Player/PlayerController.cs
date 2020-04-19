using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
	{
	[SerializeField] private float movementSpeed = 20.0f;
	[SerializeField] private float rotationSpeed = 400.0f;
	[Tooltip("Maximum look down Angle, Front is 0/360 Degrees, straight down 90/360 Degrees")]
	[SerializeField] private float maxLookDown = 90.0f;
	[Tooltip("Maximum look up Angle, Front is 0/360 Degrees, straight up 270/360 Degrees")]
	[SerializeField] private float maxLookUp = 270.0f;
	[SerializeField] private float jumpStrength = 40.0f;
	[Tooltip("The Height of the GameObjects Pivot Point above the Ground")]
	[SerializeField] private float distanceToGround = 0.0f;
	[Tooltip("Minimum Time between 2 Jump Attempts")]
	[SerializeField] private float jumpTime = 0.2f;
	[SerializeField] private GameObject head = null;
	private bool grounded = false;
	private Rigidbody rigidbody = null;
	private Vector3 movement = Vector3.zero;
	private float lastJump = 0.0f;
	private float jumpCharge = 0.0f;
	private bool mouseVisible = false;

	private void Start()
		{
		rigidbody = gameObject.GetComponent<Rigidbody>();
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
				head.transform.rotation = Quaternion.Euler(new Vector3(rotation.x, 0.0f, 0.0f));
				transform.rotation = Quaternion.Euler(new Vector3(0.0f, rotation.y, 0.0f));
				}
			else
				{
				transform.rotation = Quaternion.Euler(rotation);
				}
			}

		// Movement
		// Movement is only possible when having Ground Contact, else the last Input is applied again
		if(grounded)
			{
			// Movement
			movement = transform.rotation * new Vector3(Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime, 0.0f, Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime);
			}
		else
			{
			// Jump Movement
			movement *= 1.0f - (rigidbody.drag * Time.deltaTime);
			}
		transform.Translate(movement, Space.World);

		// Jumping
		if(Input.GetButton("Jump"))
			{
			if((Time.time - lastJump) >= jumpTime && grounded)
				{
				lastJump = Time.time;
				jumpCharge = 0.0f;
				rigidbody.AddForce(transform.up * jumpStrength * Time.deltaTime, ForceMode.Impulse);
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
		return Physics.Raycast(transform.position + Vector3.up * 0.02f, Vector3.down, distanceToGround + 0.04f);
		}

	public void setMouseVisible(bool mouseVisible)
		{
		this.mouseVisible = mouseVisible;
		}
	}
