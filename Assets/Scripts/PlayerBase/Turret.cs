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
            Vector3 targetRelativePosition = target.transform.position - turretGuns.position;
            float t = FirstOrderInterceptTime(muzzleVelocity, targetRelativePosition, target.SpeedVector);
            Vector3 targetLead = target.transform.position + target.SpeedVector * t;
            Vector3 dir = targetLead - turretGuns.position;

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
                float distanceToEnemy = Vector3.Distance(turretGuns.position, target.transform.position);
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
        float shortestDistance = range;
        Airship nearestEnemy = null;
        foreach (Airship enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(turretGuns.position, enemy.transform.position);
            if (distanceToEnemy <= range && distanceToEnemy < shortestDistance && turretGuns.position.y <= enemy.transform.position.y)
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

    //first-order intercept using relative target position
    public static float FirstOrderInterceptTime
    (
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity
    )
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }
}
