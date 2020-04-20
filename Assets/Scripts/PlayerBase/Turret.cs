using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building
{
    [SerializeField] string turretName;
    [SerializeField] int fireRate;
    [SerializeField] int size;
    [SerializeField] int damage;
    [SerializeField] float muzzleVelocity;
    [SerializeField] int range;
    [SerializeField] int cost;
    [SerializeField] int turnSpeed;
    [Tooltip("Maximum Deviation from Point of Aim in cm at a Target Distance of 100m")]
    [SerializeField] int spread;
    [SerializeField] Transform turretBase;
    [SerializeField] Transform turretGuns;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject shell;
    EnemyList enemyList;
    Airship target;
    Quaternion qRotation;
    float fireCountdown = 0f;

    public string GetName()
    {
        return name;
    }

    public int GetFireRate()
    {
        return fireRate;
    }

    public int GetSize()
    {
        return size;
    }

    public int GetDamage()
    {
        return damage;
    }

    public int GetRange()
    {
        return range;
    }

    public int GetCost()
    {
        return cost;
    }

    public float GetMeterSpread()
    {
        return spread * 0.01f;
    }

    private void Start()
    {
        enemyList = FindObjectOfType<EnemyList>();
        StartCoroutine(UpdateTarget());
    }

    void Update()
    {
        fireCountdown -= Time.deltaTime;
        
        if (target != null)
        {
            float targetingLead = (-Vector3.Distance(target.transform.position, transform.position) * target.speed) / (target.speed - muzzleVelocity);


            Vector3 dir = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z - targetingLead) - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            //qRotation = Quaternion.Euler(0f, turretBase.rotation.y + 90, turretGuns.localRotation.z);
            qRotation = Quaternion.Lerp(qRotation, lookRotation, Time.deltaTime * turnSpeed);
            Vector3 realRotation = qRotation.eulerAngles;
            turretBase.rotation = Quaternion.Euler(0f, realRotation.y, 0f);
            turretGuns.localRotation = Quaternion.Euler(realRotation.x, 0f, 0f);

            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
        }
    }

    IEnumerator UpdateTarget()
    {
        while (true)
        {
            Airship[] enemies = enemyList.GetEnemies().ToArray();
            if (target == null)
            {
                SearchNearestTarget(enemies);
            }
            else
            {
                float distanceToEnemy = Vector3.Distance(transform.position, target.transform.position);
                if (distanceToEnemy > range)
                {
                    SearchNearestTarget(enemies);
                }
            }
            yield return new WaitForSeconds(1);
        }

    }
    void SearchNearestTarget(Airship[] enemies)
    {
        float shortestDistance = Mathf.Infinity;
        Airship nearestEnemy = null;
        foreach (Airship enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= range && distanceToEnemy < shortestDistance && transform.position.y <= enemy.transform.position.y)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        if (nearestEnemy != null)
        {
            target = nearestEnemy;
        }
        else
        {
            target = null;
        }
    }
    void Shoot()
    {
        GameObject bullet = Instantiate(shell, firePoint.position, firePoint.rotation);
        Vector3 deviation = (Random.insideUnitSphere * spread) / 10000.0f;
        bullet.GetComponent<Rigidbody>().AddForce((bullet.transform.forward + deviation) * muzzleVelocity, ForceMode.VelocityChange);
    }

}
