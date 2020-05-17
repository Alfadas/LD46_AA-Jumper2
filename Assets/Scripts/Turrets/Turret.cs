using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Hittable
{
    readonly Vector3 gravity = new Vector3(0, 10f, 0);
    const int maxTotalRotationDiff = 10;

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
    [Tooltip("If turret aims at Enemys with TurretTargetEnemy")]
    [SerializeField] bool targetingEnemys = true;
    [Tooltip("Shoothing bullets with WeaponController")]
    [SerializeField] bool shootingBullets = true;

    [Header("Shooting")]
    [Tooltip("Only displayed damage per second, no real damage property")]
    [SerializeField] int damage;
    [Tooltip("Max auto target range")]
    [SerializeField] int range;
    [Tooltip("Spread multiplicator if turret fires automaticaly")] 
    [SerializeField] int autoSpreadMulti = 4;

    [Header("Parts")]
    [Tooltip("Bullet start point")]
    [SerializeField] Transform firePoint;
    [SerializeField] WeaponController weaponController;

    TurretRotator turretRotator; //rotator comnponent
    TurretTargetEnemy turretTargetEnemy;
    

    public bool TargetHidden { get; set; } = false;
    public Airship Target { get; set; }

    public Vector3 TurretAimCenter
    {
        get
        {
            if (rotatable)
            {
                return turretRotator.TurretGun;
            }
            else
            {
                return transform.position;
            }
        }
    }

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
            return weaponController.RoundsPerMinute;
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
        return weaponController.Spread * 0.01f * autoSpreadMulti;
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
        if (targetingEnemys)
        {
            turretTargetEnemy = gameObject.GetComponent<TurretTargetEnemy>();
            if (turretTargetEnemy == null)
            {
                Debug.LogWarning("missing turret rotator component");
            }
        }
        if (shootingBullets)
        {
            weaponController = gameObject.GetComponentInChildren<WeaponController>();
            if (weaponController == null)
            {
                Debug.LogWarning("missing turret rotator component");
            }
            weaponController.SpreadMod = autoSpreadMulti;
        }
        StartCoroutine(turretTargetEnemy.UpdateTarget());
    }

    void Update()
    {
        if (Target != null)
        {
            Quaternion lookRotation = Quaternion.identity;
            if (rotatable)
            {
                lookRotation = Quaternion.LookRotation(CalcTargetLeadPoint());
                turretRotator.RotateTurret(lookRotation);
            }
            if (shootingBullets && weaponController.ReadyToFire)
            {
                if (rotatable)
                {
                    float rotationDiff = turretRotator.GetRotationDiff(lookRotation);

                    if (rotationDiff <= maxTotalRotationDiff) // if it is near the ideal fire alignment
                    {
                        if (!targetingEnemys || targetingEnemys && CheckShootingPathWithAirshipTarget()) // if not targeting enemies skip check
                        {
                            TargetHidden = false;
                            weaponController.pullTrigger();
                        }
                        else
                        {
                            weaponController.releaseTrigger();
                            if (rotationDiff <= 0.1f) // if it is at the ideal fire alignment
                            {
                                if (!TargetHidden && targetingEnemys)
                                {
                                    TargetHidden = true;
                                    StartCoroutine(turretTargetEnemy.ChangeTarget(Target));
                                }
                            }
                            else
                            {
                                TargetHidden = false;
                            }
                        }
                    }
                }
                else
                {
                    weaponController.pullTrigger();
                }
            }
        }
    }

    private Vector3 CalcTargetLeadPoint()
    {
        Vector3 targetRelativePosition = Target.transform.position - TurretAimCenter;
        float t = FirstOrderInterceptTime(weaponController.MuzzleVelocity, targetRelativePosition, Target.Velocity);
        float timeGravity = FirstOrderInterceptTime(weaponController.MuzzleVelocity, targetRelativePosition, gravity);
        Vector3 targetLead = Target.transform.position + Target.Velocity * t + 0.5f * gravity * Mathf.Pow(timeGravity, 2);
        Vector3 dir = targetLead - TurretAimCenter;
        return dir;
    }

    private bool CheckShootingPathWithAirshipTarget()
    {
        float targetDistance = Vector3.Distance(Target.transform.position, firePoint.position);
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