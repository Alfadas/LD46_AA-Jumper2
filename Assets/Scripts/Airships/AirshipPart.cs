using System.Collections;
using UnityEngine;

public class AirshipPart : Hittable
{
    [SerializeField] bool essential;
    [SerializeField] GameObject explosionVfx;
    [SerializeField] float explosionSize;
    [SerializeField] int explosionDamage = 15;
    [SerializeField] int fragmentCount = 25;
    protected Airship airship; //airship this is attached to

    protected override void Start()
    {
        base.Start();
        airship = GetComponentInParent<Airship>(); //get attached airship
    }

    protected override void DestroyHittable()
    {
        if(explosionVfx && explosionSize > 0)
        {
            GameObject explosion = Instantiate(explosionVfx, transform.position, Quaternion.identity);
            float scale = explosion.transform.localScale.x * explosionSize;
            explosion.transform.localScale = new Vector3(scale, scale, scale);
            //change gravity to scale appropriately
            ParticleSystem.MainModule particleSystem = explosion.GetComponent<ParticleSystem>().main; //need to get it first to work
            particleSystem.gravityModifierMultiplier = scale;

            ThrowFragments();
            Object.Destroy(explosion, 2);
        }
        if (essential)
        {
            airship.DestroyAirship();
        }
        Object.Destroy(gameObject);
    }

    void ThrowFragments()
    {
        RaycastHit hit;
        Vector3 direction = Random.insideUnitSphere;
        for (int i = 0; i < fragmentCount; i++)
        {
            if (Physics.Raycast(transform.position, direction * explosionSize, out hit))
            {
                Hittable hittable = hit.collider.gameObject.GetComponent<Hittable>();
                if (hittable)
                {
                    hittable.GetDamage(explosionDamage);
                }
            }
            direction = Random.insideUnitSphere;
        }
    }
}