using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAvoider : MonoBehaviour
{
    [SerializeField] Airship airship;
    [SerializeField] Airship collidingAirship;
    // Start is called before the first frame update
    void Start()
    {
        airship = GetComponentInParent<Airship>();
        StartCoroutine(CheckSpeed());
    }

    IEnumerator CheckSpeed()
    {
        while (true)
        {
            if (collidingAirship != null)
            {
                CorrectSpeed();
            }
            else
            {
                airship.speed = airship.maxSpeed;
            }
            yield return new WaitForSeconds(2);
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
        if (newCollidingAirship != null)
        {
            collidingAirship = newCollidingAirship;
            CorrectSpeed();
        }
    }
}
