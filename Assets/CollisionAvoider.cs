using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAvoider : MonoBehaviour
{
    Airship airship;
    Airship collidingAirship;
    // Start is called before the first frame update
    void Start()
    {
        airship = GetComponentInParent<Airship>();
    }

    IEnumerator CheckSpeed()
    {
        while (true)
        {
            if (collidingAirship != null)
            {
                CorrectSpeed();
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void CorrectSpeed()
    {
        if (collidingAirship.speed <= airship.maxSpeed)
        {
            airship.speed = collidingAirship.speed;
        }
        else
        {
            airship.speed = airship.maxSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Airship newCollidingAirship = other.gameObject.GetComponentInParent<Airship>();
        if(collidingAirship != null)
        {
            collidingAirship = newCollidingAirship;
            CorrectSpeed();
        }
    }
}
