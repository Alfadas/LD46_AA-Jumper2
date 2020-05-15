﻿using UnityEngine;

public class Hittable : MonoBehaviour
{
    [Header("Hittable General")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float damageMulti = 1;
    protected int health; //currentHealth
    bool destroyed = false; //bool to secure one time destruction

    protected void Start()
    {
        health = maxHealth; //set health to maxHealth
    }
    public virtual void GetDamage(int damage) //Methode if this Part is damaged
    {
        health -= (Mathf.CeilToInt(damage * damageMulti)); // airship gets damage * damage multi
        if(health <= 0)
        {
            DestroyHittable();
        }
    }
    public virtual void DestroyHittable()
    {
        if (!destroyed)
        {
            destroyed = true;
        }
        else
        {
            return;
        }
    }
}