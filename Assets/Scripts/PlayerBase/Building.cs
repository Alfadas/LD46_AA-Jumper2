using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Building General")]
    [SerializeField] int maxHealth = 100;
    protected int health; // current health
    bool destroyed = false; //bool to secure one time destruction

    private void Start()
    {
        health = maxHealth; //set health to maxHealth
    }

    public void GetDamage(int damage) //methode to calc damage to the Building itself
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
    }
}