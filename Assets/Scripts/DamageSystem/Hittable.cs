using System.Collections;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    [Header("Hittable General")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float preDestructionHealthPerc = 0.25f;
    [SerializeField] protected float damageMulti = 1;
    [SerializeField] protected bool isEnemy = true;
    [SerializeField] protected int preDestructionDamagePerSec = 5;

    protected int health = 0; //currentHealth
    protected bool preDestroyed = false;
    protected bool destroyed = false; //bool to secure one time destruction

    public int MaxHealth => maxHealth;

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
        Debug.Log(this.GetType() + "got dmg:" + Mathf.CeilToInt(damage * damageMulti));
        if(health <= 0)
        {
            TryPreDestroyHittable();
            if (health <= -maxHealth * preDestructionHealthPerc)
            {
                TryDestroyHittable();
            }
        }
    }

    public virtual void TryPreDestroyHittable()
    {
        if (preDestroyed) return;
        preDestroyed = true;
        ApplyDamagePerSecond(preDestructionDamagePerSec);
        PreDestroyHittable();
    }

    public virtual void TryDestroyHittable()
    {
        if (destroyed) return;
        destroyed = true;
        DestroyHittable();
    }

    protected virtual void PreDestroyHittable()
    {
        preDestroyed = true;
    }

    protected virtual void DestroyHittable()
    {
        destroyed = true;
    }

    IEnumerator ApplyDamagePerSecond(int damage)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            GetDamage(damage);
        }
    }
}