using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
	{
	[SerializeField] private float movementSpeed = 20.0f;
    [SerializeField] private float sprintMulti = 2f;
	[SerializeField] private float rotationSpeed = 400.0f;
	[Tooltip("Maximum look down Angle, Front is 0/360 Degrees, straight down 90/360 Degrees")]
	[SerializeField] private float maxLookDown = 90.0f;
	[Tooltip("Maximum look up Angle, Front is 0/360 Degrees, straight up 270/360 Degrees")]
	[SerializeField] private float maxLookUp = 270.0f;
	[SerializeField] private float jumpStrength = 40.0f;
	[Tooltip("Minimum Time between 2 Jump Attempts")]
	[SerializeField] private float jumpTime = 0.2f;
	[SerializeField] private GameObject head = null;
	private Rigidbody rigidbody = null;
	private Vector3 movement = Vector3.zero;
	private float groundDistance = 0.0f;
	private float lastJump = 0.0f;
	private float jumpCharge = 0.0f;
	private bool mouseVisible = false;

	private void Start()
		{
		rigidbody = gameObject.GetComponent<Rigidbody>();
		updateGroundDistance();
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
		else
			{
			Cursor.lockState = CursorLockMode.None;
			}

		// Movement
		// Movement is only possible when having Ground Contact, else the last Input is applied again
		bool grounded = Physics.Raycast(transform.position + Vector3.up * 0.02f, Vector3.down, groundDistance + 0.04f);
		if(grounded)
			{
			// Movement
			movement = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
			if(movement.sqrMagnitude > 1)
				{
				movement = Vector3.Normalize(movement);
				}
            if (Input.GetButton("Sprint"))
            {
                movement *= movementSpeed * Time.deltaTime * sprintMulti;
            }
            else
            {
                movement *= movementSpeed * Time.deltaTime;
            }
            rigidbody.velocity = rigidbody.velocity = new Vector3(movement.x, rigidbody.velocity.y, movement.z);
        }

        // Jumping
        if (Input.GetButton("Jump"))
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

	public void updateGroundDistance()
		{
		MeshRenderer[] meshes = gameObject.GetComponentsInChildren<MeshRenderer>();
		float miny = transform.position.y;
		foreach(MeshRenderer mesh in meshes)
			{
			miny = Mathf.Min(mesh.bounds.center.y - mesh.bounds.extents.y, miny);
			}
		groundDistance = transform.position.y - miny;
		}

	public void setMouseVisible(bool mouseVisible)
		{
		this.mouseVisible = mouseVisible;
		}
	}

