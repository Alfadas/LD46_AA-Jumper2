using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship : MonoBehaviour
{
    [SerializeField] int speed = 1;
    [SerializeField] int maxHealth = 100;
    [SerializeField] int killPoint = -500;
    int health;
    private int notWaiting = 1;

    private void Start()
    {
        health = maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        Move();
        if (transform.position.z < killPoint)
        {
            DestroyShip();
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
            DestroyShip();
        }
    }

    private void DestroyShip()
    {
        Object.Destroy(gameObject, 0.2f);
    }
}
