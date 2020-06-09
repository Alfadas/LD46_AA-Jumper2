using System.Collections.Generic;
using UnityEngine;

public class Airship : MonoBehaviour
{
    [Tooltip("Maximum m/s")]
    [SerializeField] int maxSpeed = 1;
    [SerializeField] int force = 5;
    [Tooltip("Z Point to automaticly dissolve the ship at the end of the lane")]
    [SerializeField] int killPoint = -500;
    [Tooltip("Metal/Score rewarded for killing the ship")]
    [SerializeField] int metal = 5;
    [Header("Tutorial Refs")]
    [Tooltip("MetalManager ref to add kill reward")]
    [SerializeField] MetalManager metallManager = null;
    [Tooltip("EnemyList ref to delete ship on destruction")]
    [SerializeField] EnemyList enemyList = null;
    bool destroyed = false; //bool to secure one time destruction

    Rigidbody airshipRigidbody = null;
    List<CollisionAvoider> collisionAvoiders = new List<CollisionAvoider>(); // list of collision Avoiders blocked by this Airship

    float forceModifiyer = 1f;
    float forceReduction = 1f;
    int maxForce = 0;

    public float ForceModifyer
    {
        get
        {
            return forceModifiyer;
        }
        set
        {
            if (value >= 1)
            {
                forceModifiyer = 1;
            }
            else
            {
                forceModifiyer = value;
            }
        }
    }

    public Vector3 Velocity //speed as Vector 3
    {
        get
        {
            return airshipRigidbody.velocity;
        }
    }

    public int MaxSpeedModified // get for max speed
    {
        get
        {
            return Mathf.CeilToInt(maxSpeed * forceReduction);
        }
    }

    public void AddCollisionAvoider(CollisionAvoider collisionAvoider)
    {
        collisionAvoiders.Add(collisionAvoider);
    }

    public void AddMass(int value)
    {
        airshipRigidbody.mass += value;
    }

    public void AddForce(int value)
    {
        force += value;
        maxForce += value;
    }

    public void RemoveForce(int value)
    {
        force -= value;
        forceReduction = force / maxForce;
    }

    public void BreakFollowing(Airship newCollidingAirship)
    {
        if (newCollidingAirship == this) return;
        foreach (CollisionAvoider collisionAvoider in collisionAvoiders)
        {
            collisionAvoider.Break(newCollidingAirship);
        }
    }

    void Awake()
    {
        maxForce = force;
        airshipRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        if (transform.position.z < killPoint)
        {
            Dissolve();
        }
    }

    public void SetManagingObjects(MetalManager metallManager, EnemyList enemyList) // called after spwan to set refs
    {
        this.metallManager = metallManager;
        this.enemyList = enemyList;
    }

    void Move()
    {
        //Debug.Log(name + " " + airshipRigidbody.velocity);
        airshipRigidbody.AddRelativeForce(Vector3.back * force * ForceModifyer * Time.deltaTime * 100);
    }

    public void Dissolve() //destroy object without kill reward
    {
        enemyList.RemoveEnemy(this);
        Object.Destroy(gameObject, 0.1f);
    }

    public void DestroyAirship()
    {
        if (destroyed) return;
        destroyed = true;
        //give kill reward
        metallManager.AddMetalAndScore(metal);
        enemyList.RemoveEnemy(this);
        Object.Destroy(gameObject, 0.1f);
    }
}