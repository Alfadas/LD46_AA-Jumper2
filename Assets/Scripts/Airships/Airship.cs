using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship : MonoBehaviour
{
    public int maxSpeed = 1;
    [HideInInspector] public int speed;
    [SerializeField] int maxHealth = 100;
    [SerializeField] int killPoint = -500;
    [SerializeField] int metal = 5;
    int health;
    private int notWaiting = 1;
    MetalManager metallManager;
    EnemyList enemyList;

    private void Start()
    {
        speed = maxSpeed;
        health = maxHealth;
    }

    public void SetManagingObjects(MetalManager metallManager, EnemyList enemyList)
    {
        this.metallManager = metallManager;
        this.enemyList = enemyList;
    }
    // Update is called once per frame
    void Update()
    {
        Move();
        if (transform.position.z < killPoint)
        {
            DestroyShip(false);
        }
    }

    void Move()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (-speed * Time.deltaTime * notWaiting));
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            DestroyShip(true);
        }
    }

    private void DestroyShip(bool kill)
    {
        if (kill)
        {
            metallManager.AddMetalAndScore(metal);
        }
        enemyList.RemoveEnemy(this);
        Object.Destroy(gameObject, 0.1f);
    }
}
