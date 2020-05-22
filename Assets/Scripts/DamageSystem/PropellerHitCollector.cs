using UnityEngine;

public class PropellerHitCollector : AirshipHitCollector
{
    [Tooltip("percent of Max speed, reduced, if the propeller is destroyed")]
    [Range(0,1)][SerializeField] float speedReductionPerc = 0.5f;
    protected override void Start()
    {
        base.Start();
    }

    public override void DestroyHittable()
    {
        airship.ChangeMaxSpeedModifier(airship.MaxSpeed * speedReductionPerc); //Floor to never set MaxSpeed to 0
        base.DestroyHittable();
    }
}