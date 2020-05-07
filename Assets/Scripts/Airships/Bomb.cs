using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Tooltip("Damage to buildings on impact")]
    [SerializeField] int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        Building building = other.gameObject.GetComponent<Building>();//try getting Building component
        if (building != null) //if the collider has a building component attached
        {
            building.GetDamage(damage); //Do damage to the building
            DestroyBomb();
        }
    }

    void DestroyBomb()
    {
        Object.Destroy(gameObject, 0.2f);
    }
}