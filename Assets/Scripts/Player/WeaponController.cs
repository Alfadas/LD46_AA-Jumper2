using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
	{
	[SerializeField] private Vector3 aimPosition = Vector3.zero;
	[SerializeField] private Vector3 muzzle = Vector3.zero;
	[Tooltip("Maximum Deviation from Point of Aim in cm at a Target Distance of 100m")]
	[SerializeField] private float spread = 50.0f;
	[Tooltip("Angle by which the Gun is rotated at most per Shot in Degrees upwards")]
	[SerializeField] private float verticalRecoil = 0.4f;
	[Tooltip("Angle by which the Gun is rotated at most per Shot in Degrees to a random Side")]
	[SerializeField] private float horizontalRecoil = 0.2f;
	[Tooltip("The Fraction of the current accumulated Recoil which is recovered per Second")]
	[SerializeField] private float recoilResetFactor = 1.0f;
	[SerializeField] private int roundsPerMinute = 600;
	[SerializeField] private int magazineCapacity = 1;
	[SerializeField] private float reloadTime = 2.0f;
	[SerializeField] private float muzzleVelocity = 40.0f;
	[Tooltip("Available Burst Counts, the first Element is default, 0 means full-auto")]
	[SerializeField] private int[] fireModes = { 0, 3, 1 };
	[SerializeField] private GameObject bulletPrefab = null;
	[SerializeField] private AudioClip fireSound = null;
	private Vector3 hipPosition = Vector3.zero;
	private Quaternion originalRotation = Quaternion.identity;
	private float timePerRound = 1.0f;
	private float lastShot = 0.0f;
	private int shotCount = 0;
	private float reloadStarted = 0.0f;
	private Text bulletCounter = null;
	private AudioSource audioSource = null;
	private bool fire = false;
	private int fireMode = 0;
	private int shotsFired = 0;
	public bool safety = false;

	private void Start()
		{
		originalRotation = transform.localRotation;
		timePerRound = 1.0f / (roundsPerMinute / 60.0f);
		hipPosition = transform.localPosition;
		audioSource = gameObject.GetComponent<AudioSource>();
		bulletCounter = GameObject.Find("BulletCounter").GetComponent<Text>();
		}

	private void Update()
		{
		// Auto Reload
		if(shotCount <= 0 && reloadStarted < 0)
			{
			reloadStarted = Time.time;
			}

		// Finish Reload after reloadTime
		if(reloadStarted >= 0 && Time.time - reloadStarted >= reloadTime)
			{
			reloadStarted = -1;
			shotCount = magazineCapacity;
			}

		// Fire Weapon
		if(fire && (fireModes[fireMode] == 0 || shotsFired < fireModes[fireMode]) && !safety && (Time.time - lastShot) >= timePerRound && shotCount > 0)
			{
			lastShot = Time.time;

			--shotCount;
			++shotsFired;

			GameObject bullet = GameObject.Instantiate(bulletPrefab, transform.position + transform.rotation * Vector3.Scale(muzzle, transform.localScale), transform.rotation);
			Vector3 deviation = (Random.insideUnitSphere * spread) / 10000.0f;
			bullet.GetComponent<Rigidbody>().AddForce((bullet.transform.forward + deviation) * muzzleVelocity, ForceMode.VelocityChange);

			transform.localRotation *= Quaternion.AngleAxis(verticalRecoil * Random.Range(0.5f, 1.0f), Vector3.left);
			transform.localRotation *= Quaternion.AngleAxis(horizontalRecoil * Random.Range(-1.0f, 1.0f), Vector3.up);

			audioSource.clip = fireSound;
			audioSource.Play();
			}

		// Recenter Weapon
		float recoilAngle = Quaternion.Angle(transform.localRotation, originalRotation);
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalRotation, recoilAngle * recoilResetFactor * Time.deltaTime);

		// Update Bullet Counter
		if(reloadStarted < 0)
			{
			bulletCounter.text = shotCount + "/" + magazineCapacity;
			bulletCounter.alignment = TextAnchor.LowerRight;
			}
		else
			{
			string text = "Reload";
			int lettercount = (int)(((Time.time - reloadStarted) / reloadTime) * (text.Length + 1));   // Add a virtual Letter to let the full Word appear for longer than 1 Frame
			text = text.Substring(0, Mathf.Clamp(lettercount, 0, text.Length));                         // Subtract virtual Letter

			bulletCounter.text = text;
			bulletCounter.alignment = TextAnchor.LowerLeft;
			}
		}

	// TODO: Remove and replace by actual Animation
	public void aim()
		{
		transform.localPosition = aimPosition;
		}
	public void unaim()
		{
		transform.localPosition = hipPosition;
		}

	public void pullTrigger()
		{
		fire = true;
		shotsFired = 0;
		}

	public void releaseTrigger()
		{
		fire = false;
		}

	public void reload()
		{
		if(reloadStarted < 0)
			{
			reloadStarted = Time.time;
			shotCount = 0;
			}
		}

	public void switchFireMode(int fireMode = -1)
		{
		if(fireMode >= 0)
			{
			this.fireMode = fireMode;
			}
		else
			{
			this.fireMode = (this.fireMode + 1) % fireModes.Length;
			}
		}

	public void toggleSafety(bool safety)
		{
		this.safety = safety;
		}
	}
