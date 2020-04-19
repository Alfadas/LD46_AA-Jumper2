using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        Building building = other.gameObject.GetComponent<Building>();
        if (building != null)
        {
            building.GetDamage(damage);
            DestroyBomb();
        }
    }

    void DestroyBomb()
    {
        Object.Destroy(gameObject, 0.2f);
    }
}
