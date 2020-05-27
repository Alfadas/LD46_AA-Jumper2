using UnityEngine;

public class AirshipPart : Hittable
{
    [SerializeField] bool essential;
    [SerializeField] GameObject explosionVfx;
    [SerializeField] float explosionSize;
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
            GameObject explosion = Instantiate(explosionVfx, transform);
            float scale = explosion.transform.localScale.x * explosionSize;
            explosion.transform.localScale = new Vector3(scale, scale, scale);
        }
        if (essential)
        {
            airship.DestroyAirship();
        }
    }
}