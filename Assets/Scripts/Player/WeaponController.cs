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
    [SerializeField] private int roundsPerMinute = 600;
    [SerializeField] private int magazineCapacity = 1;
    [SerializeField] private float reloadTime = 2.0f;
    [SerializeField] private float muzzleVelocity = 40.0f;
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private AudioClip fireSound = null;
    [SerializeField] private Text bulletCounter = null;
    private Vector3 hipPosition = Vector3.zero;
    private AudioSource audioSource = null;
    private float timePerRound = 1.0f;
    private float lastShot = 0.0f;
    private int shotCount = 0;
    private float reloadStarted = 0.0f;

    private void Start()
        {
        timePerRound = 1.0f / (roundsPerMinute / 60.0f);
        hipPosition = transform.localPosition;
        audioSource = gameObject.GetComponent<AudioSource>();
        bulletCounter = GameObject.Find("BulletCounter").GetComponent<Text>();
        }

    private void Update()
        {
        // Debug.Log(shotCount + "/" + magazineCapacity + " " + (Time.time - reloadStarted) + "/" + reloadTime);

        if(shotCount <= 0 && reloadStarted < 0)
            {
            reloadStarted = Time.time;
            }

        if(reloadStarted >= 0 && Time.time - reloadStarted >= reloadTime)
            {
            reloadStarted = -1;
            shotCount = magazineCapacity;
            }

        if(Input.GetButton("Fire1") && (Time.time - lastShot) >= timePerRound && shotCount > 0)
            {
            lastShot = Time.time;

            --shotCount;

            GameObject bullet = GameObject.Instantiate(bulletPrefab, transform.position + transform.rotation * Vector3.Scale(muzzle, transform.localScale), transform.rotation);
            Vector3 deviation = (Random.insideUnitSphere * spread) / 10000.0f;
            bullet.GetComponent<Rigidbody>().AddForce((bullet.transform.forward + deviation) * muzzleVelocity, ForceMode.VelocityChange);

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
}
