using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
	{
	private const int PLAYER_GROUNDED = 0;
	private const int PLAYER_TOUCHING = 1;
	private const int PLAYER_FLOATING = 2;

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
	[SerializeField] private GameObject head = null;
	[SerializeField] private Collider feet = null;
	[Tooltip("Float Array of Length 3 with Factors applied to Movement when Character is grounded, touches something or is completely in Air")]
	[SerializeField] private float[] movementFactors = {1.0f, 0.02f, 0.002f};
	private Rigidbody rigidbody = null;
	private Vector3 movement = Vector3.zero;
	private List<ContactPoint> contactList = null;
	private int groundingState = PLAYER_FLOATING;
	private float lastJump = 0.0f;
	private float jumpCharge = 0.0f;
	private bool mouseVisible = false;

	private void Start()
		{
		rigidbody = gameObject.GetComponent<Rigidbody>();
		contactList = new List<ContactPoint>(4);
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

		
		// TODO: Remove
		// Check for Ground Contact
		//Debug.Log(groundingState);

		// Movement
		movement = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
		if(movement.sqrMagnitude > 1)
			{
			movement = Vector3.Normalize(movement);
			}
		movement *= movementSpeed * Time.deltaTime;
        if (Input.GetButton("Sprint") && Vector3.Angle(transform.forward, movement) <= 45.0f)
			{
            movement *= sprintFactor;
			}
		// Apply Movement
		rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, new Vector3(movement.x, rigidbody.velocity.y, movement.z), movementFactors[groundingState]);
				
        // Jumping
        if (Input.GetButton("Jump"))
			{
			if((Time.time - lastJump) >= jumpTime && groundingState == PLAYER_GROUNDED)
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
		if(groundingState == PLAYER_FLOATING || groundingState == PLAYER_TOUCHING)
			{
			//Debug.Log("Bef Clear " + contactList.Count);
			contactList.Clear();
			//Debug.Log("Aft Clear " + contactList.Count);
			if(collision.GetContacts(contactList) > 0)
				{
				//Debug.Log("Bef Gen " + contactList.Count);
				foreach(ContactPoint contact in contactList)
					{
					//Debug.Log("Collider:");
					//Debug.Log(contact.thisCollider + " " + contact.otherCollider + " " + contact.point);
					if(contact.otherCollider != null && contact.otherCollider.gameObject != null && contact.thisCollider != null	 // Check every Instance cz of Unity Bugs *sigh*
						&& !contact.otherCollider.gameObject.Equals(gameObject))
						{
						//Debug.Log(contact.thisCollider + " collides with " + contact.otherCollider);
						if(contact.thisCollider.Equals(feet))
							{
							//Debug.Log("Seems Grounded");
							groundingState = PLAYER_GROUNDED;
							break;
							}
						else
							{
							//Debug.Log("Seems Touching");
							groundingState = PLAYER_TOUCHING;
							}
						}
					}
				}
			}
		}

	private void OnCollisionExit(Collision collision)
		{
		groundingState = PLAYER_FLOATING;
		}

	public void setMouseVisible(bool mouseVisible)
		{
		this.mouseVisible = mouseVisible;
		}
	}

