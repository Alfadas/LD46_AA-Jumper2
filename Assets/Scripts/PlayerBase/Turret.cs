using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building
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
    [SerializeField] int turnSpeed;

    [Header("Shooting")]
    [Tooltip("Fire rate per second")]
    [SerializeField] float fireRate;
    [Tooltip("Only displayed damage per second, no real damage property")]
    [SerializeField] int damage;
    [SerializeField] float muzzleVelocity;
    [SerializeField] int range;
    [Tooltip("Maximum Deviation from Point of Aim in cm at a Target Distance of 100m")]
    [SerializeField] int spread;
    [Tooltip("Spread multiplicator if turret fires automaticaly")]
    [SerializeField] int autoSpreadMulti = 4;
    [SerializeField] GameObject shell;

    [Header("Parts")]
    [Tooltip("Part to rotate on y-axis (parent of gun)")]
    [SerializeField] Transform turretBase;
    [Tooltip("Part to rotate on local x-axis")]
    [SerializeField] Transform turretGuns;
    [Tooltip("Bullet start point")]
    [SerializeField] Transform firePoint;

    float fireCountdown = 0f;
    bool targetHidden = false;
    EnemyList enemyList;
    Airship target;
    Quaternion qRotation;

    public string GetName()
    {
        return turretName;
    }

    public float GetFireRate()
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

    public float GetMeterAutoSpread()
    {
        return spread * 0.01f * autoSpreadMulti;
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
            Quaternion lookRotation = Quaternion.LookRotation(CalcTargetLeadPoint());
            RotateTurret(lookRotation);

            if (fireCountdown <= 0f)
            {
                Vector3 realLookRotation = lookRotation.eulerAngles;
                float rotationDiff = Quaternion.Angle(turretGuns.localRotation, Quaternion.Euler(realLookRotation.x, 0f, 0f)) +
                                     Quaternion.Angle(turretBase.rotation, Quaternion.Euler(0f, realLookRotation.y, 0f));

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
    private void RotateTurret(Quaternion lookRotation)
    {
        qRotation = Quaternion.Lerp(qRotation, lookRotation, Time.deltaTime * turnSpeed);
        Vector3 realRotation = qRotation.eulerAngles;
        turretBase.rotation = Quaternion.Euler(0f, realRotation.y, 0f);
        turretGuns.localRotation = Quaternion.Euler(realRotation.x, 0f, 0f);
    }

    private Vector3 CalcTargetLeadPoint()
    {
        Vector3 targetRelativePosition = target.transform.position - turretGuns.position;
        float t = FirstOrderInterceptTime(muzzleVelocity, targetRelativePosition, target.SpeedVector);
        float timeGravity = FirstOrderInterceptTime(muzzleVelocity, targetRelativePosition, gravity);
        Vector3 targetLead = target.transform.position + target.SpeedVector * t + 0.5f * gravity * Mathf.Pow(timeGravity, 2);
        Vector3 dir = targetLead - turretGuns.position;
        return dir;
    }

    private bool CheckShootingPath()
    {
        float targetDistance = Vector3.Distance(target.transform.position, firePoint.position);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(firePoint.position, firePoint.transform.forward, out hit, targetDistance);
        if (!hasHit || (hasHit && hit.collider.gameObject.GetComponent<HitCollector>() != null))
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
}