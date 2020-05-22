using System.Collections;
using UnityEngine;

public class CollisionAvoider : MonoBehaviour
{
    Airship airship; //Airship this is attached to
    Airship collidingAirship; //Airship the attached Airship would collide with (currently colliding with the Avoider)

    void Start()
    {
        airship = GetComponentInParent<Airship>(); // get attached airship
        StartCoroutine(CheckSpeed());
    }

    IEnumerator CheckSpeed() //check for speed changes every 2 seconds
    {
        while (true)
        {
            if (collidingAirship != null) // if there is a colliding airship
            {
                CorrectSpeed();
            }
            else 
            {
                airship.Speed = airship.MaxSpeedModified; //set speed back to max of attached
            }
            yield return new WaitForSeconds(2);
        }
    }

    private void CorrectSpeed() // allign speed with colliding or, if colliding is faster than max speed, reset to max speed
    {
        if (collidingAirship.Speed <= airship.MaxSpeedModified)
        {
            airship.Speed = collidingAirship.Speed;
        }
        else
        {
            airship.Speed = airship.MaxSpeedModified;
        }
    }

    private void OnTriggerEnter(Collider other) //colliding ship is set only on enter and never removed, new ones just override
    {
        Airship newCollidingAirship = other.gameObject.GetComponentInParent<Airship>();
        if (newCollidingAirship != null) //if colliding object is a Airship
        {
            //set as new colliding and change speed
            collidingAirship = newCollidingAirship;
            CorrectSpeed();
        }
    }
}