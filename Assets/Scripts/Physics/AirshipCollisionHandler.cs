using UnityEngine;

static class AirshipCollisionHandler
{
    public static void HandleCollisionEnter(Hittable collidingHittable, GameObject otherCollidingObject)
    {
        Airship collidingAirship = collidingHittable.gameObject.GetComponentInParent<Airship>();
        Airship otherCollidingAirship = otherCollidingObject.gameObject.GetComponentInParent<Airship>();
        if (collidingAirship && otherCollidingAirship && collidingAirship == otherCollidingAirship) return; // Airship, don´t hit yourself

        Vector3 relativeVelocity = Vector3.zero;
        int damage = 0;
        if (collidingAirship && otherCollidingAirship)
        {
            relativeVelocity = collidingAirship.Velocity - otherCollidingAirship.Velocity;
            damage = Mathf.FloorToInt(Vector3.Magnitude(relativeVelocity));
        }
        else
        {
            //relativeVelocity = collidingAirship.Velocity;
            damage = collidingHittable.MaxHealth * 2;
        }

        Debug.Log(collidingHittable.name + " " + damage);

        collidingHittable.GetDamage(damage);
    }
}
