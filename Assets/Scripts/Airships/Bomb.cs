using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Tooltip("Damage to buildings on impact")]
    [SerializeField] int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        Hittable hittable = other.gameObject.GetComponent<Hittable>();//try getting Building component
        if (hittable != null && !hittable.IsEnemy) //if the collider has a building component attached
        {
            hittable.GetDamage(damage); //Do damage to the building
            DestroyBomb();
        }
    }

    void DestroyBomb()
    {
        Object.Destroy(gameObject, 0.2f);
    }
}