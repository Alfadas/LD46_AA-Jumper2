using System.Collections;
using UnityEngine;

public class AirshipPart : Hittable
{
    [Tooltip("Destruction also destroyes ariship")]
    [SerializeField] bool essential = false;
    [Tooltip("Mass of the whole part")]
    [SerializeField] int partMass = 100;
    [Header("Explosion")]
    [Tooltip("Explosion Vfx Prefab")]
    [SerializeField] GameObject explosionVfx = null;
    [Tooltip("Size in Unity scale the explosion will get scaled to")]
    [SerializeField] float explosionSize = 1;
    [Tooltip("Damage per explosion fragment")]
    [SerializeField] int explosionDamage = 15;
    [Tooltip("Count of fragments generated on exploding")]
    [SerializeField] int fragmentCount = 25;
    protected Airship airship = null; //airship this is attached to

    protected override void Start()
    {
        base.Start();
        airship = GetComponentInParent<Airship>(); //get attached airship
        airship.AddMass(partMass);
    }

    protected override void DestroyHittable()
    {
        base.DestroyHittable();
        if(explosionVfx && explosionSize > 0)
        {
            GameObject explosion = Instantiate(explosionVfx, transform.position, Quaternion.identity);
            float scale = explosion.transform.localScale.x * explosionSize;
            explosion.transform.localScale = new Vector3(scale, scale, scale);
            //change gravity to scale appropriately
            ParticleSystem.MainModule particleSystem = explosion.GetComponent<ParticleSystem>().main; //need to get it first to work
            particleSystem.gravityModifierMultiplier = scale;

            //disable cxolliders on Part and hitCollectors
            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            foreach(Collider collider in colliders)
            {
                collider.enabled = false;
            }

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
            if (Physics.Raycast(transform.position, direction, out hit, explosionSize))
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

    private void OnCollisionEnter(Collision collision)
    {
        AirshipCollisionHandler.HandleCollisionEnter(this, collision.gameObject);
    }
}