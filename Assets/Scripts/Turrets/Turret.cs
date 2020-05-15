﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Hittable
{
    readonly Vector3 gravity = new Vector3(0, 10f, 0);
    const int maxTotalRotationDiff = 10;
    const int changeTargetIfHiddenTime = 3;

    [Header("General")]
    [Tooltip("Displayed name")]
    [SerializeField] string turretName;
    [Tooltip("Purchase metal cost")]
    [SerializeField] int cost;
    [Tooltip("Size in m")]
    [SerializeField] int size;
    [Tooltip("Turret max rotation speed")]
    [SerializeField] int turnSpeed;
    [Tooltip("If turret can rotate, needs TurretRotator")]
    [SerializeField] bool rotatable = true;

    [Header("Shooting")]
    [Tooltip("Fire rate per second")]
    [SerializeField] float fireRate;
    [Tooltip("Only displayed damage per second, no real damage property")]
    [SerializeField] int damage;
    [Tooltip("Bullet velocity at the muzzle")]
    [SerializeField] float muzzleVelocity;
    [Tooltip("Max auto target range")]
    [SerializeField] int range;
    [Tooltip("Maximum Deviation from Point of Aim in cm at a Target Distance of 100m")]
    [SerializeField] int spread;
    [Tooltip("Spread multiplicator if turret fires automaticaly")]
    [SerializeField] int autoSpreadMulti = 4;
    [Tooltip("Shell prefab")]
    [SerializeField] GameObject shell;

    [Header("Parts")]
    [Tooltip("Bullet start point")]
    [SerializeField] Transform firePoint;
    [Tooltip("Part to rotate on local x-axis")]
    [SerializeField] Transform turretGuns;

    float fireCountdown = 0f;
    bool targetHidden = false;
    TurretRotator turretRotator; //rotator comnponent
    EnemyList enemyList;
    Airship target;


    public string Name
    {
        get
        {
            return turretName;
        }
    }

    public float FireRate
    {
        get
        {
            return fireRate;
        }
    }

    public int Size
    {
        get
        {
            return size;
        }
    }

    public int Damage
    {
        get
        {
            return damage;
        }
    }

    public int Range
    {
        get
        {
            return range;
        }
    }

    public int Cost
    {
        get
        {
            return cost;
        }
    }

    public int TurnSpeed
    {
        get
        {
            return turnSpeed;
        }
    }

    public float GetMeterAutoSpread()
    {
        return spread * 0.01f * autoSpreadMulti;
    }

    protected override void Start()
    {
        base.Start();
        if (rotatable)
        {
            turretRotator = gameObject.GetComponent<TurretRotator>();
            if (turretRotator == null)
            {
                Debug.LogWarning("missing turret rotator component");
            }
        }
        enemyList = FindObjectOfType<EnemyList>();
        StartCoroutine(UpdateTarget());
    }

    void Update()
    {
        fireCountdown -= Time.deltaTime;

        if (target != null)
        {
            if (rotatable)
            {
                Quaternion lookRotation = Quaternion.LookRotation(CalcTargetLeadPoint());
                turretRotator.RotateTurret(lookRotation);

                if (fireCountdown <= 0f)
                {
                    float rotationDiff = turretRotator.GetRotationDiff(lookRotation);

                    if (rotationDiff <= maxTotalRotationDiff) // if it is near the ideal fire alignment
                    {
                        if (CheckShootingPath())
                        {
                            targetHidden = false;
                            Shoot();
                        }
                        else
                        {
                            if (rotationDiff <= 0.1f) // if it is at the ideal fire alignment
                            {
                                if (!targetHidden)
                                {
                                    targetHidden = true;
                                    StartCoroutine(ChangeTarget(target));
                                }
                            }
                            else
                            {
                                targetHidden = false;
                            }
                        }
                    }
                }
            }
            else
            {
                Shoot();
            }
        }
    }

    IEnumerator ChangeTarget(Airship oldTarget)
    {
        yield return new WaitForSeconds(changeTargetIfHiddenTime);
        if (target == oldTarget && targetHidden)
        {
            List<Airship> enemies = enemyList.GetEnemies().GetRange(0, enemyList.GetEnemies().Count);
            enemies.Remove(oldTarget);
            targetHidden = false;
            SearchNearestTarget(enemies.ToArray());
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

    private Vector3 CalcTargetLeadPoint()
    {
        Vector3 targetRelativePosition = target.transform.position - turretGuns.position;
        float t = FirstOrderInterceptTime(muzzleVelocity, targetRelativePosition, target.Velocity);
        float timeGravity = FirstOrderInterceptTime(muzzleVelocity, targetRelativePosition, gravity);
        Vector3 targetLead = target.transform.position + target.Velocity * t + 0.5f * gravity * Mathf.Pow(timeGravity, 2);
        Vector3 dir = targetLead - turretGuns.position;
        return dir;
    }

    private bool CheckShootingPath()
    {
        float targetDistance = Vector3.Distance(target.transform.position, firePoint.position);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(firePoint.position, firePoint.transform.forward, out hit, targetDistance);
        if (!hasHit || (hasHit && hit.collider.gameObject.GetComponentInParent<Airship>() != null))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(shell, firePoint.position, firePoint.rotation);
        Vector3 deviation = (Random.insideUnitSphere * spread * autoSpreadMulti) / 10000.0f;
        bullet.GetComponent<Rigidbody>().AddForce((bullet.transform.forward + deviation) * muzzleVelocity, ForceMode.VelocityChange);
        fireCountdown = 1f / fireRate;
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

    public override void DestroyHittable()
    {
        base.DestroyHittable();
        Object.Destroy(gameObject);
    }
}