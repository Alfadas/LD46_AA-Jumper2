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

    // Code Duplication with Airship, maybe make a Destroyable-Class
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
        // If-Check unnecessary like in Airship
        if (!destroyed)
        {
            destroyed = true;
            // No GameObject.Destroy()?
        }
        else
        {
            return;
        }
    }
}