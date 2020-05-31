using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipBalloon : AirshipPart
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void PreDestroyHittable()
    {
        Rigidbody mainRigidbody = gameObject.GetComponentInParent<Rigidbody>();
        mainRigidbody.useGravity = true;
        base.PreDestroyHittable();
    }
}
