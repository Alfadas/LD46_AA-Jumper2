using UnityEngine;

public class Airship : MonoBehaviour
{
    [Tooltip("Maximum m/s")]
    [SerializeField] int maxSpeed = 1;
    [Tooltip("Maximum Airship Health")]
    [SerializeField] int maxHealth = 100;
    [Tooltip("Z Point to automaticly dissolve the ship at the end of the lane")]
    [SerializeField] int killPoint = -500;
    [Tooltip("Metal/Score rewarded for killing the ship")]
    [SerializeField] int metal = 5;
    [Header("Tutorial Refs")]
    [Tooltip("MetalManager ref to add kill reward")]
    [SerializeField] MetalManager metallManager; 
    [Tooltip("EnemyList ref to delete ship on destruction")]
    [SerializeField] EnemyList enemyList; 
    int health; //current health
    bool destroyed = false; //bool to secure one time destruction

    public int Speed { get; set; } // current speed

    // Velocity == SpeedVector, Speed == float, Velocity == Vector
    public Vector3 SpeedVector //speed as Vector 3
    {
        get
        {
            return new Vector3(0, 0, -Speed);
        }
    }

    public int MaxSpeed // get for max speed
    {
        get
        {
            return maxSpeed;
        }
    }

    private void Start()
    {
        //set speed and health to max
        Speed = maxSpeed;
        health = maxHealth;
    }

    void Update()
    {
        Move();
        if (transform.position.z < killPoint)
        {
            DestroyShip(false);
        }
    }

    public void SetManagingObjects(MetalManager metallManager, EnemyList enemyList) // called after spwan to set refs
    {
        this.metallManager = metallManager;
        this.enemyList = enemyList;
    }

    void Move()
    {
        transform.position = transform.position + (SpeedVector * Time.deltaTime); //move along speedvector, *deltaTime for framerate independence
    }

    public void GetDamage(int damage) //methode to calc damage to the ship itself
    {
        health -= damage;
        if (health <= 0)
        {
            Dissolve(); // Why no Kill Reward, when being destroyed by Gunfire?
        }
    }

    // Why this extra method?
    public void Dissolve() //destroy object without kill reward
    {
        DestroyShip(false);
    }

    private void DestroyShip(bool killReward) // destroys ship with or without kill reward
    {
        if (!destroyed) // Can simply override destroyed, if check is unnecessary
        {
            destroyed = true;
            if (killReward)
            {
                //give kill reward
                metallManager.AddMetalAndScore(metal);
            }
            enemyList.RemoveEnemy(this);
            Object.Destroy(gameObject, 0.1f);
        }
    }
}