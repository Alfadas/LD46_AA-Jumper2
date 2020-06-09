using UnityEngine;

public class AirshipPropeller : AirshipPart
{
    [Header("Engine")]
    [Tooltip("force applied by the engine")]
    [SerializeField] int force = 3000;

    protected override void Start()
    {
        base.Start();
        airship.AddForce(force);
    }

    protected override void PreDestroyHittable()
    {
        airship.RemoveForce(Mathf.FloorToInt(force * 0.5f));
        base.PreDestroyHittable();
    }

    protected override void DestroyHittable()
    {
        airship.RemoveForce(Mathf.CeilToInt(force * 0.5f));
        base.DestroyHittable();
    }
}