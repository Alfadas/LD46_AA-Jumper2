using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
	[Tooltip("Base Damage of this Weapon")]
	[SerializeField] private float damage = 1.0f;
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
	[SerializeField] private Vector3 aimPosition = Vector3.zero;
	[Tooltip("A GameObject which has its Center at the Muzzle Point of the Weapon to determine where Bullets will be spawned")]
	[SerializeField] private Transform muzzle = null;
	[Tooltip("Available Burst Counts, the first Element is default, 0 means full-auto")]
	[SerializeField] private int[] fireModes = { 0, 3, 1 };
	[SerializeField] private GameObject bulletPrefab = null;
	[SerializeField] private Text magazineIndicator = null;
	[SerializeField] private Text firemodeIndicator = null;
	[SerializeField] private AudioClip fireSound = null;
	private Vector3 hipPosition = Vector3.zero;
	private Quaternion originalRotation = Quaternion.identity;
	private float roundsPerMinuteMod = 1.0f;
	private float timePerRound = 1.0f;
	private float lastShot = 0.0f;
	private int shotCount = 0;
	private float reloadStarted = 0.0f;
	private AudioSource audioSource = null;
	private bool fire = false;
	private int fireMode = 0;
	private int shotsFired = 0;
	private bool safety = false;
	private bool toggleSafety = false;

	public float Damage
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
	public float Spread
	{
		get
		{
			return spread;
		}
		private set
		{
			spread = value;
		}
	}
	public float VerticalRecoil
	{
		get
		{
			return verticalRecoil;
		}
		private set
		{
			verticalRecoil = value;
		}
	}
	public float HorizontalRecoil
	{
		get
		{
			return horizontalRecoil;
		}
		private set
		{
			horizontalRecoil = value;
		}
	}
	public int RoundsPerMinute
	{
		get
		{
			return roundsPerMinute;
		}
		private set
		{
			roundsPerMinute = value;
			timePerRound = 1.0f / ((RoundsPerMinute * RoundsPerMinuteMod) / 60.0f);
		}
	}
	public int MagazineCapacity
	{
		get
		{
			return magazineCapacity;
		}
		private set
		{
			magazineCapacity = value;
		}
	}
	public float ReloadTime
	{
		get
		{
			return reloadTime;
		}
		private set
		{
			reloadTime = value;
		}
	}
	public float MuzzleVelocity
	{
		get
		{
			return muzzleVelocity;
		}
		private set
		{
			muzzleVelocity = value;
		}
	}
	public float DamageMod { get; set; } = 1.0f;
	public float SpreadMod { get; set; } = 1.0f;
	public float RecoilMod { get; set; } = 1.0f;
	public float RoundsPerMinuteMod
	{
		get
		{
			return roundsPerMinuteMod;
		}
		set
		{
			roundsPerMinuteMod = value;
			timePerRound = 1.0f / ((RoundsPerMinute * RoundsPerMinuteMod) / 60.0f);
		}
	}
	public float MagazineCapacityMod { get; set; } = 1.0f;
	public float ReloadTimeMod { get; set; } = 1.0f;
	public float MuzzleVelocityMod { get; set; } = 1.0f;
	public bool ReadyToFire { get; private set; } = false;
	public bool Safety
	{
		get
		{
			return safety;
		}

		set
		{
			toggleSafety = value;
		}
	}

	private void Start()
	{
		originalRotation = transform.localRotation;
		timePerRound = 1.0f / ((RoundsPerMinute * RoundsPerMinuteMod) / 60.0f);
		hipPosition = transform.localPosition;
		audioSource = gameObject.GetComponent<AudioSource>();
	}

	private void Update()
	{
		// Auto Reload
		if(shotCount <= 0 && reloadStarted < 0)
		{
			reloadStarted = Time.time;
		}

		// Finish Reload after reloadTime
		if(reloadStarted >= 0 && Time.time - reloadStarted >= (ReloadTime * ReloadTimeMod))
		{
			reloadStarted = -1;
			shotCount = Mathf.RoundToInt(MagazineCapacity * MagazineCapacityMod);
		}

		if(!safety && reloadStarted < 0 && (Time.time - lastShot) >= timePerRound && shotCount > 0)
		{
			ReadyToFire = true;
		}
		else
		{
			ReadyToFire = false;
		}

		// Fire Weapon
		if(fire && ReadyToFire)
		{
			lastShot = Time.time;

			--shotCount;
			++shotsFired;

			GameObject bullet = GameObject.Instantiate(bulletPrefab, muzzle.position, transform.rotation);
			bullet.GetComponent<Bullet>().DamageMod = Damage * DamageMod;
			Vector3 deviation = (Random.insideUnitSphere * Spread * SpreadMod) / 10000.0f;
			bullet.GetComponent<Rigidbody>().AddForce((bullet.transform.forward + deviation) * MuzzleVelocity * MuzzleVelocityMod, ForceMode.VelocityChange);

			transform.localRotation *= Quaternion.AngleAxis(VerticalRecoil * RecoilMod * Random.Range(0.5f, 1.0f), Vector3.left);
			transform.localRotation *= Quaternion.AngleAxis(HorizontalRecoil * RecoilMod * Random.Range(-1.0f, 1.0f), Vector3.up);

			if(fireModes[fireMode] != 0 && shotsFired >= fireModes[fireMode])
			{
				fire = false;
			}

			if(fireSound != null)
			{
				audioSource.clip = fireSound;
				audioSource.Play();
			}	
		}

		// Recenter Weapon
		float recoilAngle = Quaternion.Angle(transform.localRotation, originalRotation);
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalRotation, recoilAngle * recoilResetFactor * Time.deltaTime);

		// Toggle Safety
		safety = toggleSafety;

		// Update Bullet Counter
		if(magazineIndicator != null)
		{
			if(reloadStarted < 0)
			{
				magazineIndicator.text = shotCount + "/" + (MagazineCapacity * MagazineCapacityMod);
				magazineIndicator.alignment = TextAnchor.LowerRight;
			}
			else
			{
				string text = "Reload";
				int lettercount = (int)(((Time.time - reloadStarted) / (ReloadTime * ReloadTimeMod)) * (text.Length + 1));	// Add a virtual Letter to let the full Word appear for longer than 1 Frame
				text = text.Substring(0, Mathf.Clamp(lettercount, 0, text.Length));											// Subtract virtual Letter

				magazineIndicator.text = text;
				magazineIndicator.alignment = TextAnchor.LowerLeft;
			}
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

		if(firemodeIndicator != null)
		{
			if(fireModes[this.fireMode] == 0)
			{
				firemodeIndicator.text = "Auto";
			}
			else if(fireModes[this.fireMode] == 1)
			{
				firemodeIndicator.text = "Semi";
			}
			else
			{
				firemodeIndicator.text = fireModes[this.fireMode] + "-Burst";
			}
		}
	}
}
