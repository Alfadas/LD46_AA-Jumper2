﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Building General")]
    [SerializeField] int maxHealth = 100;
    protected int health;
    bool destroyed = false;

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
        if (!destroyed)
        {
            destroyed = true;
        }
        else
        {
            return;
        }
        Debug.Log("Building Destroyed");
    }
}
