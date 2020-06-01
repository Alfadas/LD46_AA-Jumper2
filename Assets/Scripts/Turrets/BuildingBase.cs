using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    Turret turret = null; //turret on the BuildingBase

    public bool IsFilled // returns if the BuildingBase has a turret
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