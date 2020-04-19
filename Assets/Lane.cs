using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    [SerializeField] MeshRenderer newAirship;
    [SerializeField] int minDistance;

    public bool HasAirship
    {
        get
        {
            return newAirship != null;
        }
    }
    public void SetAirship(MeshRenderer airship)
    {
        newAirship = airship;
    }

    private void Start()
    {
        newAirship = null;
        StartCoroutine(CheckIfShipLeft());
    }

    IEnumerator CheckIfShipLeft()
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