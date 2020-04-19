using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building
{
    [SerializeField] string turretName;
    [SerializeField] int fireRate;
    [SerializeField] int size;
    [SerializeField] int damage;
    [SerializeField] int range;
    [SerializeField] int cost;

    public string GetName()
    {
        return name;
    }

    public int GetFireRate()
    {
        return fireRate;
    }

    public int GetSize()
    {
        return size;
    }

    public int GetDamage()
    {
        return damage;
    }

    public int GetRange()
    {
        return range;
    }

    public int GetCost()
    {
        return cost;
    }
}
