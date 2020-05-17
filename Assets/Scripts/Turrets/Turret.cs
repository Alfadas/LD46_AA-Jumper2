using UnityEngine;

public class Turret : Hittable
{
    const int maxTotalRotationDiff = 10;
    const int smallShipHitArea = 50; // 4^2*pi  using smalest ship as reference

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
    [Tooltip("Lead target with TurretTargetHelper")]
    [SerializeField] bool useTargetHelper = true;

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
    TurretTargetHelper targetHelper;

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

    public int MagazineCapacity
    {
        get
        {
            return weaponController.MagazineCapacity;
        }
    }

    public int GetAccuracyOnDistance(int distance, bool onAutoFire) //distance in m
    {
        float hittingArea = 1;
        if (onAutoFire)
        {
            hittingArea = Mathf.Pow(distance * 0.01f * (weaponController.Spread * autoSpreadMulti * 0.01f), 2) * Mathf.PI;
        }
        else
        {
            hittingArea = Mathf.Pow(distance * 0.01f * (weaponController.Spread * 0.01f), 2) * Mathf.PI;
        }
        int accuracy = 100 - Mathf.FloorToInt(hittingArea / smallShipHitArea );
        if(accuracy > 100)
        {
            accuracy = 100;
        }
        return accuracy;
    }

    public WeaponController WeaponController
    {
        get
        {
            return weaponController;
        }
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
        if (useTargetHelper)
        {
            targetHelper = gameObject.GetComponent<TurretTargetHelper>();
            if (targetHelper == null)
            {
                Debug.LogWarning("missing turret target helper component");
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
                if (useTargetHelper)
                {
                    lookRotation = Quaternion.LookRotation(targetHelper.CalcTargetLeadPoint());
                }
                else
                {
                    lookRotation = Quaternion.LookRotation(Target.transform.position - TurretAimCenter);
                }
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

    public override void DestroyHittable()
    {
        base.DestroyHittable();
        Object.Destroy(gameObject);
    }
}