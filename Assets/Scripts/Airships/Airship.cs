using UnityEngine;

public class Airship : MonoBehaviour
{
    [Tooltip("Maximum m/s")]
    [SerializeField] int maxSpeed = 1;
    [SerializeField] bool useForce = false;
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

    float maxSpeedModifier = 1f;

    public int Speed { get; set; } // current speed

    public Vector3 Velocity //speed as Vector 3
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

    public int MaxSpeedModified // get for max speed
    {
        get
        {
            return Mathf.CeilToInt(maxSpeed * maxSpeedModifier);
        }
    }

    void Start()
    {
        Speed = maxSpeed;
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
        if (useForce)
        {
            airshipRigidbody.AddRelativeForce(Vector3.back * force * Time.deltaTime * 100);
        }
        else
        {
            transform.position = transform.position + (Velocity * Time.deltaTime); //move along speedvector, *deltaTime for framerate independence
        }
    }

    public void ChangeMaxSpeedModifier(float addition) // Add
    {
        maxSpeedModifier += addition;
        if (Speed > MaxSpeedModified) //update speed
        {
            Speed = MaxSpeedModified;
        }
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