using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] Turret turret;

    public bool IsFilled
    {
        get
        {
            return turret != null;
        }
    }

    public void PlaceTurret(Turret newTurret)
    {
        turret = Instantiate(newTurret, transform.position, Quaternion.identity, transform);
    }
}
