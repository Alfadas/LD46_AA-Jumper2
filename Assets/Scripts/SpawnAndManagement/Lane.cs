using System.Collections;
using UnityEngine;

public class Lane : MonoBehaviour
{
    [Tooltip("Minimal distance, the ship needs to travel, until lane is free again")]
    [SerializeField] int minDistance;
    Airship newAirship;

    public bool HasAirship
    {
        get
        {
            return newAirship != null;
        }
    }
    public void SetAirship(Airship airship)
    {
        newAirship = airship;
    }

    private void Start()
    {
        newAirship = null;
        StartCoroutine(CheckIfShipLeft());
    }

    IEnumerator CheckIfShipLeft() //checks every sec if spwaned ship is far enough away to spawn a new one
    {
        while (true)
        {
            if (HasAirship)
            {
                if(transform.position.z - newAirship.transform.position.z > minDistance)
                {
                    newAirship = null;
                    yield return new WaitUntil(() => HasAirship == true);
                }
                else
                {
                    yield return new WaitForSeconds(1);
                }
                
            }
            else
            {
                yield return new WaitUntil(() => HasAirship == true);
            }
        }
    }
}