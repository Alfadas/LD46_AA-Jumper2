using System.Collections;
using UnityEngine;

public class CollisionAvoider : MonoBehaviour
{
    Airship airship = null; //Airship this is attached to
    Airship collidingAirship = null; //Airship the attached Airship would collide with (currently colliding with the Avoider)
    bool speedBalanced = false;

    void Start()
    {
        airship = GetComponentInParent<Airship>(); // get attached airship
    }

    IEnumerator CheckSpeed() //check for speed changes every 2 seconds
    {
        yield return new WaitUntil(() => speedBalanced == true);
        while (collidingAirship != null)
        {
            //ToDo: break if the colliding airship has to break
            airship.ForceModifyer = collidingAirship.Velocity.z / -airship.MaxSpeedModified;
            yield return new WaitForSeconds(2);
        }
        airship.ForceModifyer = 1;
    }

    IEnumerator BalanceSpeed(Airship referenceShip)
    {
        while (!speedBalanced)
        {
            if (referenceShip.Velocity.z <= airship.Velocity.z)
            {
                speedBalanced = true;
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void OnTriggerEnter(Collider other) //colliding ship is set only on enter and never removed, new ones just override
    {
        Airship newCollidingAirship = other.gameObject.GetComponentInParent<Airship>();
        if (newCollidingAirship != null) //if colliding object is a Airship
        {
            //set as new colliding and change speed
            collidingAirship = newCollidingAirship;
            collidingAirship.AddCollisionAvoider(this);
            Break(collidingAirship);
        }
    }

    public void Break(Airship referenceShip)
    {
        airship.BreakFollowing(referenceShip);
        airship.ForceModifyer = -1f;
        StopAllCoroutines();
        StartCoroutine(CheckSpeed());
        StartCoroutine(BalanceSpeed(referenceShip));
    }
}