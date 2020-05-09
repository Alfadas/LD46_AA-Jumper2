using UnityEngine;

// Maybe rename to BuildingBaseplate or something like that, Base could mean the Baseplate, the Playerbase or that this is a Base-Class for all Buildings
public class BuildingBase : MonoBehaviour
{
    Turret turret; //turret on the BuildingBase

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