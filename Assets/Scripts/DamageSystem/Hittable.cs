using UnityEngine;

public class Hittable : MonoBehaviour
{
    [Header("Hittable General")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float damageMulti = 1;
    [SerializeField] protected bool isEnemy = true;

    protected int health; //currentHealth
    protected bool destroyed = false; //bool to secure one time destruction

    public bool IsEnemy
    {
        get
        {
            return isEnemy;
        }
    }

    protected virtual void Start()
    {
        health = maxHealth; //set health to maxHealth
    }
    public virtual void GetDamage(int damage) //Methode if this Part is damaged
    {
        health -= (Mathf.CeilToInt(damage * damageMulti)); // airship gets damage * damage multi
        if(health <= 0)
        {
            TryDestroyHittable();
        }
    }
    public virtual void TryDestroyHittable()
    {
        if (destroyed) return;
        destroyed = true;
        DestroyHittable();
    }

    protected virtual void DestroyHittable()
    {
        health = 0;
    }
}