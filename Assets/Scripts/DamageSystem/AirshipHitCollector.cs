using UnityEngine;

public class AirshipHitCollector : Hittable
{
    protected AirshipPart airshipPart = null; //airship this is attached to

    protected override void Start()
    {
        base.Start();
        airshipPart = GetComponentInParent<AirshipPart>(); //get attached airship
    }

    public override void GetDamage(int damage) //Methode if this Part is damaged
    {
        base.GetDamage(damage);
        airshipPart.GetDamage(Mathf.CeilToInt(damage * damageMulti)); // airship gets damage * damage multi
    }

    protected override void DestroyHittable()
    {
        damageMulti = 1;
    }
}