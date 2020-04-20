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
    public bool shootingBlocked = false;

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
        if(shotCount <= 0 && reloadStarted < 0)
            {
            reloadStarted = Time.time;
            }

        if(reloadStarted >= 0 && Time.time - reloadStarted >= reloadTime)
            {
            reloadStarted = -1;
            shotCount = magazineCapacity;
            }

        if(Input.GetButton("Fire1") && !shootingBlocked && (Time.time - lastShot) >= timePerRound && shotCount > 0)
            {
            lastShot = Time.time;

            --shotCount;

            GameObject bullet = GameObject.Instantiate(bulletPrefab, transform.position + transform.rotation * Vector3.Scale(muzzle, transform.localScale), transform.rotation);
            Vector3 deviation = (Random.insideUnitSphere * spread) / 10000.0f;
            bullet.GetComponent<Rigidbody>().AddForce((bullet.transform.forward + deviation) * muzzleVelocity, ForceMode.VelocityChange);

            transform.localRotation *= Quaternion.AngleAxis(verticalRecoil * Random.Range(0.5f, 1.0f), Vector3.left);
            transform.localRotation *= Quaternion.AngleAxis(horizontalRecoil * Random.Range(-1.0f, 1.0f), Vector3.up);
            Debug.Log(horizontalRecoil * Random.Range(-1.0f, 1.0f));

            audioSource.clip = fireSound;
            audioSource.Play();
            }

        if(Input.GetButtonDown("Fire2"))
            {
            transform.localPosition = aimPosition;
            }

        if(Input.GetButtonUp("Fire2"))
            {
            transform.localPosition = hipPosition;
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
            string text = "Reloading ";

            int dotcount = (int) (((Time.time - reloadStarted) / reloadTime) / 0.25f);
            for(int i = 0; i < dotcount; ++i)
                {
                text += ".";
                }

            bulletCounter.text = text;
            bulletCounter.alignment = TextAnchor.LowerLeft;
            }
        }
    public void BlockShooting(bool stop)
    {
        shootingBlocked = stop;
    }
}
