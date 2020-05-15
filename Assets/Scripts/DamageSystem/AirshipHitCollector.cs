using UnityEngine;

public class AirshipHitCollector : Hittable
{
    Airship airship; //airship this is attached to

    protected override void Start()
    {
        base.Start();
        airship = GetComponentInParent<Airship>(); //get attached airship
    }

    public override void GetDamage(int damage) //Methode if this Part is damaged
    {
        base.GetDamage(damage);
        airship.GetDamage(Mathf.CeilToInt(damage * damageMulti)); // airship gets damage * damage multi
    }
}