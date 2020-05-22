using UnityEngine;

public class Airship : Hittable
{
    [Tooltip("Maximum m/s")]
    [SerializeField] int maxSpeed = 1;
    [Tooltip("Z Point to automaticly dissolve the ship at the end of the lane")]
    [SerializeField] int killPoint = -500;
    [Tooltip("Metal/Score rewarded for killing the ship")]
    [SerializeField] int metal = 5;
    [Header("Tutorial Refs")]
    [Tooltip("MetalManager ref to add kill reward")]
    [SerializeField] MetalManager metallManager; 
    [Tooltip("EnemyList ref to delete ship on destruction")]
    [SerializeField] EnemyList enemyList;

    float maxSpeedModifier;

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

    protected override void Start()
    {
        base.Start();
        //set speed to max
        Speed = maxSpeed;
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
        transform.position = transform.position + (Velocity * Time.deltaTime); //move along speedvector, *deltaTime for framerate independence
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

    public override void DestroyHittable()
    {
        base.DestroyHittable();
        //give kill reward
        metallManager.AddMetalAndScore(metal);
        enemyList.RemoveEnemy(this);
        Object.Destroy(gameObject, 0.1f);
    }
}