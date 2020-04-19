using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] int health;

    private void Start()
    {
        health = maxHealth;
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            DestroyBuilding();
        }
    }

    protected virtual void DestroyBuilding()
    {
        Debug.Log("Building Destroyed");
    }
}
