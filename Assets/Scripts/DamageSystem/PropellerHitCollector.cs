using UnityEngine;

public class PropellerHitCollector : AirshipHitCollector
{
    [Tooltip("percent of Max speed, reduced, if the propeller is destroyed")]
    [Range(0,1)][SerializeField] float speedReductionPerc = 0.5f;
    [SerializeField] GameObject explosionVfx;
    protected override void Start()
    {
        base.Start();
    }

    public override void DestroyHittable()
    {
        base.DestroyHittable();
        airship.ChangeMaxSpeedModifier(-speedReductionPerc);
        Instantiate(explosionVfx, transform);
    }
}